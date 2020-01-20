using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using  System.Web;
using static SINCRODEService.Program;

namespace SINCRODEService
{
    class WebServiceRest
    {

        public static string GetEmployee(string endpoint, string username, string password, string NifDni)
        {
            string url = endpoint + "/" + NifDni;
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
            request.ContentType = "text/plain";
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            Stream postStreams = request.GetRequestStream();
            postStreams.Write(data, 0, data.Length);
            postStreams.Close();
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            
            return response;
        }
    }
}
