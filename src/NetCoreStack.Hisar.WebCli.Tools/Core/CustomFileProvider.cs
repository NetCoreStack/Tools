using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class CustomFileProvider : IFileProvider
    {
        private readonly EnvironmentContext _context;
        public CustomFileProvider(EnvironmentContext context)
        {
            _context = context;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new EnumerableDirectoryContents(new List<IFileInfo>());
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var name = Path.GetFileName(subpath);
            if (name == "config.js")
            {
                var savePath = "/api/editor/savepagecontent";
                var fetchPath = "/api/editor/getlayoutpage";
                // if no appdir specified work with default Layout.cshtml
                if (string.IsNullOrEmpty(_context.MainAppDirectoryWebRoot))
                {
                    savePath = "/api/page/savepage";
                    fetchPath = "/api/page/getpage?componentId=" + _context.ComponentInfo?.ComponentId;
                }

                var fileContent = "define('config', { savePath: '"+savePath+"', fetchPath: '"+fetchPath+"', componentId: '" + _context.ComponentInfo?.ComponentId + "'});";
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
                {
                    return new InMemoryFileInfo(name, name, memoryStream.ToArray(), DateTime.UtcNow);
                }
            }
            
            return new NotFoundFileInfo(name);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}
