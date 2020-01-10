using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SINCRODEWebApp.DAHelper
{
    public static class ADHelper
    {
        private static IConfigurationRoot _configuration;

        static ADHelper()
        {
            _configuration = GetConfiguration();
        }

        public static bool ActiveDirectoryLogin(string username, string password)
        {
            string _server = GetActiveDirectoryServer();
            string _user = GetActiveDirectoryUser(username);

            //string ldapServer = "LDAP://ldap.forumsys.com:389/dc=example,dc=com";

            DirectoryEntry validator = new DirectoryEntry(_server, _user, password, AuthenticationTypes.ServerBind);

            try
            {
                if (validator.NativeObject.Equals(null))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetActiveDirectoryUser(string username)
        {
            return string.Format("uid={0},{1}", username, GetActiveDirectoryDomainController());
        }

        private static string GetActiveDirectoryServer()
        {
            return string.Format("LDAP://{0}:{1}/{2}", GetActiveDirectoryServerHost(), GetActiveDirectoryServerPort(), GetActiveDirectoryDomainController());
        }

        private static string GetActiveDirectoryDomainController()
        {
            var _domaincontroller = _configuration.GetValue<string>("ActiveDirectory:DomainController");
            var _values = _domaincontroller.Split(".");
            var _controller = "";

            for (int i = 0; i <= _values.Length - 1; i++)
            {
                if (i == 0)
                {
                    _controller += string.Format("dc={0}", _values[0]);
                }
                else if(i >= 1)
                {
                    _controller += string.Format(",dc={0}", _values[i]);
                }                
            }

            return _controller;
        }

        private static string GetActiveDirectoryServerPort()
        {
           return _configuration.GetValue<string>("ActiveDirectory:Port");
        }

        private static string GetActiveDirectoryServerHost()
        {
            return _configuration.GetValue<string>("ActiveDirectory:Server");
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }
    }
}
