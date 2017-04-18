using NetCoreStack.Hisar.WebCli.Tools.Context;
using NetCoreStack.Hisar.WebCli.Tools.Models;
using NetCoreStack.Hisar.WebCli.Tools.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NetCoreStack.WebSockets;
using System.Linq;
using System.Threading.Tasks;
using System;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace NetCoreStack.Hisar.WebCli.Tools.Controllers
{
    public class PageController : Controller
    {
        private readonly HisarCliContext _context;
        private readonly IConnectionManager _connectionManager;

        public PageController(HisarCliContext context, IConnectionManager connectionManager)
        {
            _context = context;
            _connectionManager = connectionManager;
        }

        [HttpGet]
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

        [HttpPost]
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

            await _connectionManager.BroadcastBinaryAsync(Encoding.UTF8.GetBytes(page.Content), 
                new RouteValueDictionary(new { fileupdated = model.Name }));
            
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

            var cliEnvironment = HttpContext.RequestServices.GetService<CliEnvironment>();
            var path = Path.Combine(cliEnvironment.MainAppDirectoryWebRoot, fullname);
            if (System.IO.File.Exists(path))
            {
                var name = Path.GetFileName(path);
                var stream = new FileStream(path, FileMode.Open);
                return new FileStreamResult(stream, new MediaTypeHeaderValue("text/plain"))
                {
                    FileDownloadName = name
                };
            }

            return NotFound();
        }
    }
}
