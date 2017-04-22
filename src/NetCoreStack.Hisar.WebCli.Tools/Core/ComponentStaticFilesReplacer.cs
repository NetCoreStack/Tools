using System;
using NetCoreStack.Hisar.WebCli.Tools.Interfaces;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class ComponentStaticFilesReplacer : IComponentFileReplacer
    {
        private readonly string[] _files;

        public ComponentStaticFilesReplacer(params string[] files)
        {
            _files = files;
        }
        
        public void Invoke()
        {
        }
    }
}
