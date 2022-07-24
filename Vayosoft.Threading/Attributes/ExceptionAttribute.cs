using System;
using System.Diagnostics;

namespace Vayosoft.Threading.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ExceptionAttribute : Attribute
    {
        public virtual void OnException(Exception e)
        {
            Trace.TraceError($"{this.GetType().Name}| {e.GetType().Name}| {e.Message}{Environment.NewLine}{e.StackTrace}");
        }
    }
}
