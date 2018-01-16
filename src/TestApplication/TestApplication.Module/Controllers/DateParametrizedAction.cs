using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace TestApplication.Module.Controllers
{
    public class DateParametrizedActionController : ViewController
    {

        public DateParametrizedActionController()
        {
            DateActtion = new ParametrizedAction(this, "DateAction",
                DevExpress.Persistent.Base.PredefinedCategory.Edit, typeof(DateTime));

            DateActtion.Execute += DateActtion_Execute;
            DateActtion.CustomizeControl += DateActtion_CustomizeControl;
        }

        private void DateActtion_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
        }

        private void DateActtion_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            Console.WriteLine(e.ParameterCurrentValue);    
        }

        public ParametrizedAction DateActtion { get; }
    }
}
