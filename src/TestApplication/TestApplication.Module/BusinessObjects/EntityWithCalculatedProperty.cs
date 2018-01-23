using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace TestApplication.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class EntityWithCalculatedProperty : BaseObject
    {
        public EntityWithCalculatedProperty(DevExpress.Xpo.Session session) : base(session)
        {
        }



        private string name;
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [Association, Aggregated]
        public XPCollection<IntValueItem> Values => GetCollection<IntValueItem>(nameof(Values));

        public int Sum => Values.Sum(v => v.Value);
    }
}
