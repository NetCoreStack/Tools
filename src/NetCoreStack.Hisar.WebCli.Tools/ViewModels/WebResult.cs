namespace NetCoreStack.Hisar.WebCli.Tools.ViewModels
{
    public class WebResult
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class WebResult<TModel> : WebResult
    {
        public TModel Result { get; set; }

        public WebResult(TModel model)
        {
            Result = model;
        }
    }
}
