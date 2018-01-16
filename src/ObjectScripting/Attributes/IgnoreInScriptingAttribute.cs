using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenDev.XafTools.Attributes
{
    [AttributeUsageAttribute(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreInScriptingAttribute : Attribute
    {
    }
}
