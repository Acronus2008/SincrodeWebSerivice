using Microsoft.Extensions.Configuration;
using SINCRODEService.Config;
using System;
using System.Globalization;
using System.IO;

namespace SINCRODEService
{
    class Intervalo
    {
        public static int nextTimeToExecute;

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
                nextTimeToExecute = 1;
            }
            else
            {
                if (DateTime.Now < time2)
                {
                    intervalo = time2 - DateTime.Now;
                    nextTimeToExecute = 2;
                }
                else
                {
                    if (DateTime.Now < time3)
                    {
                        intervalo = time3 - DateTime.Now;
                        nextTimeToExecute = 3;
                    }
                    else
                    {
                        intervalo = time1.AddDays(1) - DateTime.Now;
                        nextTimeToExecute = 1;
                    }
                }
            }
            return (double) intervalo.TotalMilliseconds;
        }
    }
}
