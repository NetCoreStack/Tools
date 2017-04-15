using System;
using System.IO;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public interface IConsole
    {
        event ConsoleCancelEventHandler CancelKeyPress;
        TextWriter Out { get; }
        TextWriter Error { get; }
        TextReader In { get; }
        bool IsInputRedirected { get; }
        bool IsOutputRedirected { get; }
        bool IsErrorRedirected { get; }
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
    }
}
