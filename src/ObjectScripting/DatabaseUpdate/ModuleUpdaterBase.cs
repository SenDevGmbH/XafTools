using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using SenDev.XafTools.Attributes;

namespace SenDev.XafTools.DatabaseUpdate
{
    public abstract class ModuleUpdaterBase : ModuleUpdater
    {
        protected ModuleUpdaterBase(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
        {
        }


        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            ExecuteOneTimeMethods();
        }
        private void ExecuteOneTimeMethods()
        {
            var executedMethods = ObjectSpace.GetObjects<OneTimeMethodExecuteInfo>();

            var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<OneTimeMethodAttribute>() != null && !executedMethods.Any(em => em.MethodName == m.Name))
                .ToArray();

            foreach (var method in methods)
            {
                method.Invoke(this, null);
                var info = ObjectSpace.CreateObject<OneTimeMethodExecuteInfo>();
                info.MethodName = method.Name;
                info.ExecuteTime = DateTime.Now;
                info.ExecutedOnVersion = CurrentDBVersion.ToString();
                ObjectSpace.CommitChanges();
            }
        }
    }
}
