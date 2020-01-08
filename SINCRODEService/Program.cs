using System;
using System.IO;
using System.ServiceProcess;
using log4net;

namespace SINCRODEService
{
    class Program
    {
        private readonly ILog log;
        private const string _logFileLocation = @"C:\temp\servicelog.txt";

        private static void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFileLocation));
            File.AppendAllText(_logFileLocation, DateTime.Now.ToString() + " : " + logMessage + Environment.NewLine);
        }

        static void Main(string[] args)
        {
            try
            {
                Log("Entrada del sistema: ServiceBase.Run(new SincroDEService())");
                ServiceBase.Run(new SincroDEService());
            }
            catch (Exception ex)
            {
                Log("Error iniciando el servicio: " + ex.ToString());
            }
        }
    }
}
