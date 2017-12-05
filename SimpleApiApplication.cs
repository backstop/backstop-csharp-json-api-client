using System;
using System.IO;
using System.Net;
using System.Text;
/**
 * This is a simpler example focusing on concepts demostration and give you a quick start.
 *
 * To get it work, please make sure
 * 1. You have contacted Backstop support team and properly set up API access
 * 2. Replace all parameters in angle brackets, e.g. {HOST_NAME}
 * 3. After the above steps you should be able to run a simple GET example
 * 4. Currently the example for POST hedge-funds is commented out. To try out, please update the corresponding parameters
 *    and uncomment the apiExample.sendApiRequest(API_PATH_HEDGE_FUNDS, METHOD_POST, API_POST_HEDGE_FUND_BODY)
 *
 * Refer to ApiApplication for framework example of full implementation
 */
class SimpleApiApplication
{
    private static string HOST_NAME = "192.168.10.235:8080"; //
    private static string USER_NAME = "{USER_NAME}"; //
    private static string PASS_WORD = "{PASS_WORD}";
    private static string SERVICE_URL = "http://" + HOST_NAME;
    private static string LOGIN_URL = "/backstop/api/login";
    private static string API_PATH_PEOPLE = "/backstop/api/people";
    private static string API_PATH_HEDGE_FUNDS = "/backstop/api/hedge-funds";
    private static String ORGANIZATION_ID = "781301"; //807065 //{ORGANIZATION_ID}
    private static String API_POST_HEDGE_FUND_BODY = "{\n"
            + "  \"data\": {\n"
            + "    \"type\":\"hedge-funds\",\n"
            + "    \"attributes\": {\n"
            + "      \"inceptionDate\": \"2017-01-01\",\n"
            + "      \"name\": \"Test Hedge Fund-hisky\",\n"
            + "      \"quarterlyValuation\": \"false\"\n"
            + "    },\n"
            + "    \"relationships\":{\n"
            + "      \"managementCompany\": {\n"
            + "        \"data\": {\n"
            + "          \"type\":\"organizations\",\n"
            + "          \"id\":\"" + ORGANIZATION_ID + "\"\n"
            + "        }\n"
            + "      }\n"
            + "    }\n"
            + "  }\n"
            + "}";
    private static string CONTENT_TYPE_HEADER = "application/vnd.api+json";
    private static string HTTP_METHOD_GET = "GET";
    private static string HTTP_METHOD_POST = "POST";
    private static string authorizationToken = null;
    public static void Main(string[] args)
    {
        SimpleApiApplication apiExample = new SimpleApiApplication();
        var response = apiExample.login();
        //Console.Write(response);
        //response = apiExample.sendApiRequest(API_PATH_PEOPLE, HTTP_METHOD_GET, null);
        //Console.Write(response);
        response = apiExample.sendApiRequest(API_PATH_HEDGE_FUNDS, HTTP_METHOD_POST, API_POST_HEDGE_FUND_BODY);
        Console.Write(response);
        Console.Read();
    }
    /**
     * Login with basic authentication and obtain/keep the authentication token for future communication
     */
    public string login()
    {
        authorizationToken = sendHttpRequest(LOGIN_URL, HTTP_METHOD_POST, "", false);
        return authorizationToken;
    }
    /**
     * Main method for any JSON API request
     */
    private string sendApiRequest(string apiPath, string httpMethod, string requestBody)
    {
        return sendHttpRequest(apiPath, httpMethod, requestBody, true);
    }
    /**
     * Main method for any Http request
     */
    private static string sendHttpRequest(string requestUrl, string httpMethod, string requestBody, Boolean useToken)
    {
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        HttpWebRequest request = HttpWebRequest.Create(SERVICE_URL + requestUrl) as HttpWebRequest;
        request.Accept = CONTENT_TYPE_HEADER;
        request.ContentType = CONTENT_TYPE_HEADER;
        string token = "";
        if (useToken)
        {
            token = authorizationToken;
            request.Headers.Add("token:true");
        }
        else
        {
            token = PASS_WORD;
        }
        request.Headers.Add("Authorization: " + "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(USER_NAME + ":" + token)));
        request.Method = httpMethod;
        if (!HTTP_METHOD_GET.Equals(httpMethod))
        {
            request.ContentLength = requestBody.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(requestBody);
            writer.Flush();
        }
        WebResponse response = request.GetResponse();
        if (response != null)
        {
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        else
        {
            return "Failed: request response is null.";
        }
    }
}