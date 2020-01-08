﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SINCRODEService.Config
{
    public static class ConfigHelper
    {
        private static IConfiguration _config;

        static ConfigHelper()
        {
            SetConfig();
        }

        public static IConfiguration GetConfiguration()
        {
            return _config;
        }

        private static void SetConfig()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
