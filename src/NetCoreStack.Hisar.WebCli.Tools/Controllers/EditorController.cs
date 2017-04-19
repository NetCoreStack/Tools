using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using NetCoreStack.Hisar.WebCli.Tools.Models;
using NetCoreStack.Hisar.WebCli.Tools.ViewModels;
using NetCoreStack.WebSockets;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreStack.Hisar.WebCli.Tools.Controllers
{
    [Route("api/[controller]")]
    public class EditorController : Controller
    {
        private readonly CliEnvironment _cliEnv;
        public EditorController(CliEnvironment cliEnv)
        {
            _cliEnv = cliEnv;
        }

        [HttpGet(nameof(GetLayoutPage))]
        public IActionResult GetLayoutPage()
        {
            var fullname = HostingConstants.LayoutPageFullName;
            var pageContent = Properties.Resource.DefaultPageContent;
            if (!string.IsNullOrEmpty(HostingHelper.MainAppDirectory))
            {
                fullname = PathUtility.GetLayoutPagePath(HostingHelper.MainAppDirectory);
                pageContent = System.IO.File.ReadAllText(fullname);
                var name = Path.GetFileName(fullname);
            }

            return Json(new PageContentModel
            {
                Content = pageContent,
                Fullname = fullname,
            });
        }

        [HttpGet(nameof(GetDirectories))]
        public IActionResult GetDirectories()
        {
            if (!string.IsNullOrEmpty(HostingHelper.MainAppDirectory))
            {
                var directory = new DirectoryInfo(HostingHelper.MainAppDirectory);
                if (directory.Exists)
                {
                    var tree = new JsTreeDataModel
                    {
                        Text = directory.Name,
                        Id = directory.FullName,
                        Opened = "true",
                        Type = "root"
                    };

                    PathUtility.TreeList = new List<JsTreeDataModel> { tree };
                    return Json(PathUtility.WalkDirectoryTree(directory, tree));
                }
            }

            var layoutRoot = new List<JsTreeDataModel> {
              new JsTreeDataModel
                {
                    Text = HostingConstants.LayoutPageFullName,
                    Id = HostingConstants.LayoutPageFullName,
                    Opened = "true",
                    Type = "root"
                }
            };

            return Json(layoutRoot);
        }

        [HttpPost(nameof(SavePageContent))]
        public async Task<IActionResult> SavePageContent([FromBody]PageContentModel model)
        {
            var connectionManager = HttpContext.RequestServices.GetService<IConnectionManager>();
            await connectionManager.BroadcastAsyncFileChanged(model.Content, model.Fullname);

            if (System.IO.File.Exists(model.Fullname))
            {
                System.IO.File.WriteAllText(model.Fullname, model.Content, Encoding.UTF8);
            }

            return Json(new WebResult());
        }

        [HttpGet(nameof(GetFileContent))]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult GetFileContent([FromQuery]string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                return BadRequest($"{nameof(fullname)} not speficied!");
            }

            if (System.IO.File.Exists(fullname))
            {
                var name = Path.GetFileName(fullname);
                var stream = new FileStream(fullname, FileMode.Open);
                return new FileStreamResult(stream, new MediaTypeHeaderValue("text/plain"))
                {
                    FileDownloadName = name
                };
            }

            return NotFound();
        }
    }
}
