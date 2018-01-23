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

    public class IntValueItem : BaseObject
    {
        public IntValueItem(Session session) : base(session)
        {
        }



        private EntityWithCalculatedProperty owner;
        [Association]
        public EntityWithCalculatedProperty Owner
        {
            get => owner;
            set => SetPropertyValue(nameof(Owner), ref owner, value);
        }


        private int value;
        public int Value
        {
            get => value;
            set => SetPropertyValue(nameof(Value), ref value, value);
        }
    }
}
