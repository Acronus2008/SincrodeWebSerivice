using System;
using System.IO;
using System.ServiceProcess;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;

namespace SINCRODEService
{
    class Program
    {
        private readonly ILog log;

        static void Main(string[] args)
        {
            //var builder = new ConfigurationBuilder()
            //                .SetBasePath(Directory.GetCurrentDirectory())
            //                .AddJsonFile("appsettings.json");

            //IConfiguration config = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", true, true)
            //    .Build();

            //Configure(IApliccaionBuilder app,
            //    IHostingEnviroment env,
            //    ILoggerFactory loggerFactory)
            //{
            //    loggerFactory.AddLog4Net();
            //}

            ServiceBase.Run(new SincroDEService());
        }
    }
}
