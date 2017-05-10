using System;

namespace NetCoreStack.Hisar.WebCli.Tools.Models
{
    public class Page
    {
        public int Id { get; set; }
        public string ComponentId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public PageType PageType { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
