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
using SenDev.XafTools.Attributes;
using System.ComponentModel;

namespace SenDev.XafTools.Controllers
{
    public class ScriptObjectController : ViewController
    {
        public ScriptObjectController()
        {
            ScriptObjectAction = new SimpleAction(this, "ScriptObjectAction", PredefinedCategory.Tools);
            ScriptObjectAction.Caption = "Generate Script";
            ScriptObjectAction.Execute += scriptObjectAction_Execute;
            ScriptObjectAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
        }

        public SimpleAction ScriptObjectAction { get; }

        void scriptObjectAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ObjectScriptGenerator creator = new ObjectScriptGenerator();
            var result = new ScriptingResult() { Script = creator.GenerateScript(e.SelectedObjects.Cast<object>().ToList()) };
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(Application.CreateObjectSpace(result.GetType()), result);
        }
    }
}
