public class SimpleApiApplication
{
	//...

	[SetUp]
	public void Setup(){}


       [Test]
        public void RESTAuthorizationFundsTest()
        {
            // Setup Authentication Test
            string authUrl = "http://fbtest01.co3.backstopsolutions/backstop/api/login";
            string userName = "bsg4";
            string password = "rup3rt";

            // Setup Rest Endpoint Test
            string baseUrl = "http://fbtest01.co3.backstopsolutions.com/backstop/api";
            string resource = "/funds";

            // Required Request Headers
            string contentTypeHeader = "application/vnd.api+json";
            string acceptHeader = "application/vnd.api+json";
            string method = "POST";

            // Prepare Authentication Header format
            string basicAuthHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            string securityToken = null;

            // Set Windows HTTP Security to TLS Version 1.2
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // 3072 = SecurityProtocolType.Tls12; // Enumeration value not available until .NET 4.6

            try
            {
                HttpWebRequest r = HttpWebRequest.Create(authUrl) as HttpWebRequest;

                r.Accept = acceptHeader;
                r.ContentType = contentTypeHeader;
                r.Headers.Add("Authorization:" + basicAuthHeader);
                r.Method = method;

                WebResponse res = r.GetResponse();
                if (res != null)
                {
                    Stream dataStream = res.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    securityToken = reader.ReadToEnd();
                    Console.WriteLine("Successful:\r\n Token=" + securityToken);
                }
                else
                {
                    Console.WriteLine("Failed: login response is null.");
                }
            }
            catch(Exception authex)
            {
                Console.WriteLine("Failed Authorization " + authex.Message + "\r\n" + authex.StackTrace);
            }

            Assert.IsTrue(securityToken != null);


            // Update authentication header to use Security Token Received from Login
            basicAuthHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + securityToken));

            try
            {
                HttpWebRequest r = HttpWebRequest.Create(baseUrl + resource) as HttpWebRequest;

                r.Accept = acceptHeader;
                r.ContentType = contentTypeHeader;
                r.Headers.Add("Authorization:" + basicAuthHeader);
                r.Headers.Add("token:true");
                r.Method = "GET";

                WebResponse res = r.GetResponse();
                if (res != null)
                {
                    Stream dataStream = res.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseText = reader.ReadToEnd();
                    Console.WriteLine("Successful:\r\n" + responseText);
                }
                else
                {
                    Console.WriteLine("Failed: request response is null.");
                }
            }
            catch (Exception reqex)
            {
                Console.WriteLine("Failed request " + reqex.Message + "\r\n" + reqex.StackTrace);
                throw;
            }
        }

	//...
}