using System.Collections.Generic;

namespace NetCoreStack.Hisar.WebCli.Tools.Models
{
    public class JsTreeDataModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public string Opened { get; set; }
        public List<JsTreeDataModel> Children { get; set; }

        public JsTreeDataModel()
        {
            Children = new List<JsTreeDataModel>();
        }
    }
}
