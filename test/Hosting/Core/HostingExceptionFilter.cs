using System;
using Microsoft.AspNetCore.Mvc.Filters;
using NetCoreStack.Hisar;
using NetCoreStack.Mvc.Types;

namespace Hosting.Core
{
    public class HostingExceptionFilter : IHisarExceptionFilter
    {
        public void Invoke(ExceptionContext context, SystemLog systemLog)
        {
            return;
        }
    }
}
