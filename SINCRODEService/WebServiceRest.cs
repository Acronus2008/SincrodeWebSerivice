using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using  System.Web;

namespace SINCRODEService
{
    class WebServiceRest
    {
        private const string _logFileLocation = @"C:\temp\servicelog.txt";

        private static void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFileLocation));
            File.AppendAllText(_logFileLocation, DateTime.Now.ToString() + " : " + logMessage + Environment.NewLine);
        }

        public static string GetEmployee(string endpoint, string username, string password, string NifDni)
        {
            string url = endpoint + NifDni;
            //Log("Acceso al Get del WS de Evalos: "+ url);
            var uri = new Uri(url);
            var client = new WebClient();
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            client.Headers.Add("Authorization", "Basic " + encoded);
            var json = client.DownloadString(uri);

            return json;
        }

        public static HttpWebResponse PutPostRequest(string endpoint, string username, string password, string json, string method = "POST")
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(json);
            HttpWebRequest request;

            request = WebRequest.Create(endpoint) as HttpWebRequest;
            request.Timeout = 10 * 1000;
            request.Method = method;
            request.ContentLength = data.Length;
            request.ContentType = "application/json; charset= utf8-8";
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            Stream postStreams = request.GetRequestStream();
            postStreams.Write(data, 0, data.Length);
            postStreams.Close();
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string body = reader.ReadToEnd();
            
            return response;
        }
    }
}
