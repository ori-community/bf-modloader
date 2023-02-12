using System;

namespace BFModLoader.ModLoader
{
    public enum LogLevel
    {
        Error, Warn, Info, Debug, Trace
    }

    public static class Logger
    {
        private const LogLevel Level = LogLevel.Trace;

        //TODO: Use a real logging framework
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if(level <= Level)
                Console.WriteLine("[" + level + "]" + message);
        }

        public static void Log(Exception e, LogLevel level = LogLevel.Error, bool fullLog = false)
        {
            if (level > Level)
                return;

            if (!fullLog)
            {
                Log("[" + level + "]" + e.Message, LogLevel.Error);
                return;
            }
            var exception = e;
            while(exception != null){
                Console.WriteLine("[" + level + "] " + e.GetType().Name + " - " + e.Message);
                Console.WriteLine(e.StackTrace);

                if (e.InnerException == exception)
                    break;
                
                exception = e.InnerException;
            }
        }
    }
}
