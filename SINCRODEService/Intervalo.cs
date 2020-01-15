using Microsoft.Extensions.Configuration;
using SINCRODEService.Config;
using System;
using System.Globalization;
using System.IO;

namespace SINCRODEService
{
    class Intervalo
    {
        private const string _logFileLocation = @"C:\temp\servicelog.txt";

        private static void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFileLocation));
            File.AppendAllText(_logFileLocation, DateTime.Now.ToString() + " : " + logMessage + Environment.NewLine);
        }

        public static double SetNextIntervalo()
        {
            IConfiguration config = ConfigHelper.GetConfiguration();

            DateTime time1 = DateTime.ParseExact(config["ExcetuteTime1"], "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime time2 = DateTime.ParseExact(config["ExcetuteTime2"], "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime time3 = DateTime.ParseExact(config["ExcetuteTime3"], "HH:mm:ss", CultureInfo.InvariantCulture);

            TimeSpan intervalo;
            if (DateTime.Now < time1)
            {
                intervalo = time1 - DateTime.Now;
                //Log("DateTime.Now < time1 : "+ DateTime.Now +" < "+ time1);
            }
            else
            {
                if (DateTime.Now < time2)
                {
                    intervalo = time2 - DateTime.Now;
                    //Log("DateTime.Now < time2 : " + DateTime.Now + " < " + time2);
                }
                else
                {
                    if (DateTime.Now < time3)
                    {
                        intervalo = time3 - DateTime.Now;
                        //Log("DateTime.Now < time3 : " + DateTime.Now + " < " + time3);
                    }
                    else
                    {
                        intervalo = time1.AddDays(1) - DateTime.Now;
                        //Log("DateTime.Now > que los 3 timer : " + DateTime.Now + " > " + time3 + " > " + time2 + " > " + time1);
                    }
                }
            }
            return (double) intervalo.TotalMilliseconds;
        }
    }
}
