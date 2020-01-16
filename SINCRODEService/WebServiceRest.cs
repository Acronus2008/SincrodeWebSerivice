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

        //public static string PutRequestJson(string NifDni, string endpoint, string json, string method = "PUT")
        //{
        //    // Create string to hold JSON response
        //    string jsonResponse = "200 OK";
        //    //log.Info("LLamada " + method + " a la uri: " + endpoint);
        //    using (var client = new WebClient())
        //    {
        //        //client.UseDefaultCredentials = true;
        //        client.Headers.Add("Content-Type:application/json");
        //        client.Headers.Add("Accept:application/json");
        //        var uri = new Uri(endpoint+ NifDni);
        //        //Log("LLamada " + method + " a la uri: " + uri.ToString());
        //        //Log("Json: " + json);
        //        jsonResponse = client.UploadString(uri, method, json);

        //        if (jsonResponse.Length==0)
        //        {
        //            jsonResponse = "000 Respuesta vacía";
        //        }
        //    }
        //    return jsonResponse;
        //}

        public static HttpWebResponse PutPostRequest(string endpoint, string username, string password, string json, string method = "POST", string NifDni = "")
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(json);
            HttpWebRequest request;

            string fullendpoint = endpoint;
            if (method == "PUT")
            {
                fullendpoint += NifDni;
            }

            request = WebRequest.Create(fullendpoint) as HttpWebRequest;
            request.Timeout = 10 * 1000;
            request.Method = method;
            request.ContentLength = data.Length;
            request.ContentType = "application/json; charset= utf8-8";
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            string credenttials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("usuario:clave"));
            request.Headers.Add("Authorization", "Basic" + credenttials);

            Stream postStreams = request.GetRequestStream();
            postStreams.Write(data, 0, data.Length);

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string body = reader.ReadToEnd();
            
            return response;
        }
    }
}
