using System;
using System.IO;
using System.Net;
using System.Text;

namespace BackStopApiDemo
{
    class BackStopApiDemo
    {
        static void Main(string[] args)
        {
            var response = Handshark();
            Console.Write(response);
            response = GetFunds();
            Console.Write(response);
            response = GetPeople();
            Console.Write(response);
            Console.Read();
        }
        private static string _Token = null;
        //private static string _AuthUrl = "https://demo01.backstopsolutions.com/backstop/api/login";
        //private static string _BaseUrl = "https://demo01.backstopsolutions.com/backstop/api";
        //private static string _UserName = "shiny.liu";
        //private static string _Password = "b@ckst0p";

        private static string _AuthUrl = "http://192.168.10.235:8080/backstop/api/login";
                private static string _BaseUrl = "http://192.168.10.235:8080/backstop/api";
        private static string _UserName = "bsg4";
        private static string _Password = "rup3rt";

        private static string _ContentTypeHeader = "application/vnd.api+json";
        private static string _AcceptHeader = "application/vnd.api+json";
        public static string Handshark()
        {
            string basicAuthHeader = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_UserName + ":" + _Password));
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            HttpWebRequest request = HttpWebRequest.Create(_AuthUrl) as HttpWebRequest;
            request.Accept = _AcceptHeader;
            request.ContentType = _ContentTypeHeader;
            request.Headers.Add("Authorization: " + basicAuthHeader);
            request.Method = "POST";
            WebResponse response = request.GetResponse();
            if (response != null)
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                _Token = reader.ReadToEnd();
                return "Successful:\r\n Token=" + _Token;
            }
            else
            {
                return "Failed: login response is null.";
            }
        }
        public static string GetFunds()
        {
            string resource = "/funds";
            return GetResponse(resource);
        }
        public static string GetPeople()
        {
            string resource = "/people";
            return GetResponse(resource);
        }
        private static string GetResponse(string resource)
        {
            var basicAuthHeader = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_UserName + ":" + _Token));
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // 3072 = SecurityProtocolType.Tls12; // Enumeration value not available until .NET 4.6
            HttpWebRequest request = HttpWebRequest.Create(_BaseUrl + resource) as HttpWebRequest;
            request.Accept = _AcceptHeader;
            request.ContentType = _ContentTypeHeader;
            request.Headers.Add("Authorization: " + basicAuthHeader);
            request.Headers.Add("token:true");
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            if (response != null)
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseText = reader.ReadToEnd();
                return "Successful:\r\n" + responseText;
            }
            else
            {
                return "Failed: request response is null.";
            }
        }
    }
}