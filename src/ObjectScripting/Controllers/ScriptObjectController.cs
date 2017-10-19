using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SenDev.XafTools.BusinessObjects;

namespace SenDev.XafTools.Controllers
{
    public class ScriptObjectController : ViewController
    {
        private readonly SimpleAction scriptObjectAction;

        public ScriptObjectController()
        {
            scriptObjectAction = new SimpleAction(this, "ScriptObjectAction", PredefinedCategory.Tools);
            scriptObjectAction.Caption = "Generate Script";
            scriptObjectAction.Execute += scriptObjectAction_Execute;
            scriptObjectAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            RegisterActions(scriptObjectAction);
        }

        void scriptObjectAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace ns = new CodeNamespace();
            ns.Name = "ScriptedObjects";
            compileUnit.Namespaces.Add(ns);
            CodeTypeDeclaration typeDeclaration = new CodeTypeDeclaration();
            typeDeclaration.Name = "Class1";
            ns.Types.Add(typeDeclaration);
            var method = new CodeMemberMethod();
            method.Name = "CreateObject";

            typeDeclaration.Members.Add(method);

            var rootObject = View.SelectedObjects.OfType<object>().Single();
            var rootObjectType = rootObject.GetType();

            method.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(Session)), "session"));
            method.ReturnType = new CodeTypeReference(rootObjectType.Name);
            ns.Imports.Add(new CodeNamespaceImport(rootObjectType.Namespace));

            string resultName = AddObjectInstance(method, rootObject, new InstanceManager());

            method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(resultName)));
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            var result = new ScriptingResult();
            using (var stringWriter = new StringWriter())
            {
                using (var itw = new IndentedTextWriter(stringWriter))
                {
                    var options = new CodeGeneratorOptions();
                    options.BracingStyle = "C";
                    codeProvider.GenerateCodeFromCompileUnit(compileUnit, itw, options);
                }
                result.Script = stringWriter.ToString();
            }

            e.ShowViewParameters.CreatedView = Application.CreateDetailView(ObjectSpace, result);
        }

        private static string AddObjectInstance(CodeMemberMethod method, object instance, InstanceManager instanceManager)
        {
            Type type = instance.GetType();
            if (instanceManager.ConstainsInstance(instance))
                return instanceManager.GetInstanceName(instance);

            string variableName = instanceManager.GetInstanceName(instance);
            CodeVariableDeclarationStatement objectVariable = new CodeVariableDeclarationStatement(
                type.Name, variableName, new CodeObjectCreateExpression(type.Name,
                new CodeVariableReferenceExpression("session")));

            method.Statements.Add(objectVariable);
            var ti = XafTypesInfo.Instance.FindTypeInfo(type);
            foreach (var member in ti.Members.Where(m => ((m.MemberType.IsEnum || m.MemberType.IsValueType || m.MemberType == typeof(string) || 
                m.MemberType == typeof(Type))
                && m.IsPersistent && m.IsPublic && !m.IsReadOnly)))
            {
                object value = member.GetValue(instance);
                if (value != null)
                {
                    if ((member.MemberType.IsEnum || IsPrimitive(value)))
                    {
                        method.Statements.Add(new CodeAssignStatement(
                            new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(variableName), member.Name),
                            GetMemberValueExpression(member, value)
                            ));
                    }

                    if (member.MemberType == typeof(Type))
                    {
                        method.Statements.Add(new CodeAssignStatement(
                            new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(variableName), member.Name),
                            new CodeTypeOfExpression(new CodeTypeReference((Type)value))
                            ));

                    }
                }

            }

            foreach (var member in ti.Members.Where(m => m.MemberTypeInfo.IsPersistent && m.IsPublic && !m.IsReferenceToOwner && !m.IsReadOnly))
            {
                object value = member.GetValue(instance);

                if (value != null)
                {
                    string valueName = AddObjectInstance(method, value, instanceManager);
                    method.Statements.Add(new CodeAssignStatement(
                        new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(variableName), member.Name),
                        new CodeVariableReferenceExpression(valueName)));
                }
            }

            foreach (var member in ti.Members.Where(m => m.IsAssociation && m.IsList && m.IsPublic))
            {
                var list = member.GetValue(instance) as XPBaseCollection;

                if (list != null && list.IsLoaded && list.Count > 0)
                {
                    foreach (object value in list)
                    {
                        string valueName = AddObjectInstance(method, value, instanceManager);
                        method.Statements.Add(new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                                new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(variableName), member.Name), "Add"),
                                new CodeVariableReferenceExpression(valueName)));
                    }
                }
            }

            return variableName;
        }


        private static bool IsPrimitive(object value)
        {
            //Checking value is a type supported by the CodePrimitiveExpression
            return (value == null ||
                    value is string ||
                    value is char ||
                    value is byte ||
                     value is Int16 ||
                     value is Int32 ||
                     value is Int64 ||
                     value is Single ||
                     value is Double ||
                     value is Decimal ||
                     value is bool);
        }
        private static CodeExpression GetMemberValueExpression(IMemberInfo member, object value)
        {
            if (member.MemberType.IsEnum)
                return new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(member.MemberType.FullName), value.ToString());
            else
            {
                //TODO: Implement Structs
                return new CodePrimitiveExpression(value);
            }
        }
        private class InstanceManager
        {
            private readonly Dictionary<object, string> instanceNamesDict = new Dictionary<object, string>();
            private readonly HashSet<string> names = new HashSet<string>();

            internal string GetInstanceName(object instance)
            {
                string name;
                if (instanceNamesDict.TryGetValue(instance, out name))
                {
                    return name;
                }

                for (int i = 1; names.Contains(name = CreateName(instance, i)); i++) { }
                instanceNamesDict.Add(instance, name);
                names.Add(name);
                return name;
            }

            private string CreateName(object instance, int index)
            {

                StringBuilder sb = new StringBuilder(instance.GetType().Name);
                sb[0] = Char.ToLower(sb[0]);
                sb.Append(index);
                return sb.ToString();
            }

            internal bool ConstainsInstance(object instance)
            {
                return instanceNamesDict.ContainsKey(instance);
            }
        }
    }
}
