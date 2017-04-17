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
    }
}
