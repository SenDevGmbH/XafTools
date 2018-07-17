using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SenDev.XafTools.DatabaseUpdate
{
    public class OneTimeMethodExecuteInfo : BaseObject
    {
        public OneTimeMethodExecuteInfo(Session session) : base(session)
        {
        }


        private string methodName;
        public string MethodName
        {
            get { return methodName; }
            set { SetPropertyValue(nameof(MethodName), ref methodName, value); }
        }


        private DateTime executeTime;
        public DateTime ExecuteTime
        {
            get { return executeTime; }
            set { SetPropertyValue(nameof(ExecuteTime), ref executeTime, value); }
        }

        private string executedOnVersion;
        public string ExecutedOnVersion
        {
            get { return executedOnVersion; }
            set { SetPropertyValue(nameof(ExecutedOnVersion), ref executedOnVersion, value); }
        }
    }
}
