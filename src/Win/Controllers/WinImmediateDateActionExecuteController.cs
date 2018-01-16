using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

namespace SenDev.XafTools.Win.Controllers
{
    public class WinImmediateDateActionExecuteController : Controller
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            var siteController = Frame.GetController<ActionControlsSiteController>();
            if (siteController != null)
            {
                siteController.CustomBindActionControlToAction += SiteController_CustomBindActionControlToAction;
                siteController.CustomizeActionControl += SiteController_CustomizeActionControl;
            }
        }

        private void SiteController_CustomizeActionControl(object sender, ActionControlEventArgs e)
        {
            if (e.ActionControl is BarEditItemParametrizedActionControl control)
            {
                if (control.BarItem.Edit is RepositoryItemDateEdit dateEdit)
                {
                    var executeButton = RepositoryItemButtonEditAdapter.FindExecuteButton(dateEdit);
                    if (executeButton != null)
                    {
                        executeButton.Visible = false;
                    }
                }
            }
        }

        private void SiteController_CustomBindActionControlToAction(object sender, DevExpress.ExpressApp.Templates.ActionControls.Binding.CustomBindEventArgs e)
        {
            if (e.ActionControl is BarEditItemParametrizedActionControl control)
            {
                if (control.BarItem.Edit is RepositoryItemDateEdit dateEdit)
                {
                    if (e.Action is ParametrizedAction action)
                        dateEdit.CloseUp += (s, ea) =>
                        {
                            if (ea.CloseMode == PopupCloseMode.Normal)
                                action.DoExecute(((DateEdit)s).GetPopupEditForm().ResultValue);
                        };
                }
            }
        }



    }
}
