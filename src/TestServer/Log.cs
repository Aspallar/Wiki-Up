using System;

namespace TestServer
{
    public static class Logging
    {
        public static ILog Create(bool supressLogging)
            => supressLogging ? new VoidLog() : new ConsoleLog();
    }

    public interface ILog
    {
        void Log(string message);

    }

    public class ConsoleLog : ILog
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class VoidLog : ILog
    {
        public void Log(string message) {  }
    }


}
