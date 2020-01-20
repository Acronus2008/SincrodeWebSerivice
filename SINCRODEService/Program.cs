using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Xml;
using log4net;

namespace SINCRODEService
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static void Log(string logMessage)
        {
            //log4net
            log.Info(logMessage);
        }

        static void Main(string[] args)
        {
            try
            {
                XmlDocument log4netConfig = new XmlDocument();
                log4netConfig.Load(File.OpenRead(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\log4net.config"));
                var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

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
