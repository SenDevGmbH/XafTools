using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.DC;

namespace SenDev.XafTools.BusinessObjects
{
    [DomainComponent]
    public class ScriptingResult
    {
        [FieldSize(FieldSizeAttribute.Unlimited)]
        public string Script { get; set; }
    }
}
