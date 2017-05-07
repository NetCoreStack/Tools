using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Hisar.WebCli.Tools.Context;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using NetCoreStack.Hisar.WebCli.Tools.Models;
using NetCoreStack.Hisar.WebCli.Tools.ViewModels;
using NetCoreStack.WebSockets;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStack.Hisar.WebCli.Tools.Controllers
{
    [Route("api/[controller]")]
    public class PageController : Controller
    {
        private readonly HisarCliContext _context;
        private readonly IConnectionManager _connectionManager;

        public PageController(HisarCliContext context, IConnectionManager connectionManager)
        {
            _context = context;
            _connectionManager = connectionManager;
        }

        [HttpGet(nameof(GetPage))]
        public IActionResult GetPage()
        {
            var pageViewModel = _context.Pages.Where(x => x.PageType == PageType.Layout)
                .Select(p => new PageViewModel
                {
                    Id = p.Id,
                    Content = p.Content,
                    Name = p.Name,
                    PageType = p.PageType,
                    UpdatedDate = p.UpdatedDate
                }).FirstOrDefault();

            return Json(pageViewModel);
        }

        [HttpPost(nameof(SavePage))]
        public async Task<IActionResult> SavePage([FromBody]PageViewModel model)
        {
            var page = new Page()
            {
                Content = model.Content,
                Id = model.Id,
                Name = model.Name,
                UpdatedDate = DateTime.Now
            };

            if (page.Id > 0)
                _context.Pages.Update(page);
            else
                _context.Pages.Add(page);

            _context.SaveChanges();

            await _connectionManager.BroadcastAsyncFileChanged(model.Content, model.Name);

            if (!string.IsNullOrEmpty(HostingHelper.MainAppDirectory))
            {
                var layoutPagePath = PathUtility.GetLayoutPagePath(HostingHelper.MainAppDirectory);
                System.IO.File.WriteAllText(layoutPagePath, page.Content, Encoding.UTF8);
            }

            return Json(new WebResult<PageViewModel>(model));
        }

        [HttpGet(nameof(GetFile))]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult GetFile([FromQuery]string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                return BadRequest($"{nameof(fullname)} not speficied!");
            }

            if(fullname.StartsWith("/")) 
            {
                fullname = fullname.Substring(1);
            }
            else
            {
                fullname = PathUtility.NormalizeToOSPath(fullname, true);                
            }

            var cliEnvironment = HttpContext.RequestServices.GetService<CliEnvironment>();
            var path = Path.Combine(cliEnvironment.MainAppDirectoryWebRoot, fullname);
            if (System.IO.File.Exists(path))
            {
                var name = Path.GetFileName(path);
                var stream = new FileStream(path, FileMode.Open);

                string contentType;
                var provider = new FileExtensionContentTypeProvider();
                if(provider.TryGetContentType(name, out contentType))
                {
                    return new FileStreamResult(stream, contentType)
                    {
                        FileDownloadName = name
                    };
                }

                return new FileStreamResult(stream, "text/plain")
                {
                    FileDownloadName = name
                };
            }

            return NotFound();
        }
    }
}
