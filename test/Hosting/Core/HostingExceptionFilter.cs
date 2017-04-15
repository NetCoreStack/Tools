using Microsoft.AspNetCore.Mvc.Filters;
using NetCoreStack.Hisar;
using System;

namespace Hosting.Core
{
    public class HostingExceptionFilter : IHisarExceptionFilter
    {
        public HostingExceptionFilter()
        {

        }

        public void Invoke(ExceptionContext context, SystemLog systemLog)
        {

        }
    }
}
