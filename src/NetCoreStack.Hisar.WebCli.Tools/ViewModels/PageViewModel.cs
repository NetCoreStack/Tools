using NetCoreStack.Hisar.WebCli.Tools.Models;
using System;

namespace NetCoreStack.Hisar.WebCli.Tools.ViewModels
{
    public class PageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string ComponentId { get; set; }
        public PageType PageType { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class PageContentModel
    {
        public string Fullname { get; set; }
        public string Content { get; set; }
    }
}
