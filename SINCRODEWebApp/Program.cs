using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SINCRODEWebApp
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static void Log(string logMessage)
        {
            //log4net
            log.Info(logMessage);
        }

        public static void Main(string[] args)
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log4net.config"));
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            Log("Entrada del sistema: CreateWebHostBuilder(args).Build().Run()");

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>();
    }
}
