// Hisar Cli auto generated component info class!
using System.Reflection;
using NetCoreStack.Hisar;
using Microsoft.AspNetCore.Mvc;

namespace Hisar.Component.Guideline
{
    public static class Component
    {
        public static string ComponentId { get; }
        private static string _prefixFormat = "~/{0}/";

        static Component()
        {
            ComponentId = typeof(Component).GetTypeInfo().Assembly.GetComponentId();
        }

        public static string Content(IUrlHelper urlHelper, string contentPath)
        {
#if !RELEASE
            var componentHelper = ComponentInfoHelper.GetComponentHelper(urlHelper.ActionContext);
            if (componentHelper != null)
            {
                if (componentHelper.IsExternalComponent)
                {
                    return urlHelper.Content(contentPath);
                }
            }
#endif
            if (contentPath.StartsWith("/"))
            {
                contentPath = contentPath.Substring(1);
            }
            else if (contentPath.StartsWith("~/"))
            {
                contentPath = contentPath.Substring(2);
            }

            contentPath = contentPath.Insert(0, string.Format(_prefixFormat, ComponentId));
            return urlHelper.Content(contentPath);
        }
    }
}