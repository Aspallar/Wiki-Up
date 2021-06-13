using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    internal interface ILog
    {
        void WriteLine(string message);
    }

    internal static class LogFactory
    {
        public static ILog CreateLog(bool isNoLog)
        {
            if (isNoLog)
                return new NullLog();
            else
                return new ConsoleLog();
        }
    }

    internal class ConsoleLog : ILog
    {
        public void WriteLine(string message) => Console.WriteLine(message);
    }

    internal class NullLog : ILog
    {
        public void WriteLine(string message) { }
    }
}
