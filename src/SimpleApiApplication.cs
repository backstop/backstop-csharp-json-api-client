using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
/**
* This is a simple example focusing on concepts and gives you a quick start.
*
* Here are the steps to use this example
* 1. Contact Backstop support team and properly set up user for API access
* 2. Replace all parameters in angle brackets, e.g. {HOST_NAME}
* 3. After the above steps you should be able to run a simple GET example
* 4. Currently the example for POST hedge-funds is commented out. To try it out, please update the corresponding parameters
*    and uncomment the apiExample.sendApiRequest(API_PATH_HEDGE_FUNDS, METHOD_POST, API_POST_HEDGE_FUND_BODY)
*
* Refer to the Java ApiApplication for a framework example of the full implementation
*/
class SimpleApiApplication
{
    private static string HOST_NAME = "{HOST_NAME}";
    private static string USER_NAME = "{USER_NAME}";
    private static string PASS_WORD = "{PASS_WORD}";

    private static string SERVICE_URL = "https://" + HOST_NAME;
    private static string LOGIN_URL = "/backstop/api/login";

    private static string API_PATH_PEOPLE = "/backstop/api/people";
    private static string API_PATH_HEDGE_FUNDS = "/backstop/api/hedge-funds";

    private static String ORGANIZATION_ID = "{ORGANIZATION_ID}";
    private static String API_POST_HEDGE_FUND_BODY = "{\n"
            + "  \"data\": {\n"
            + "    \"type\":\"hedge-funds\",\n"
            + "    \"attributes\": {\n"
            + "      \"inceptionDate\": \"2017-01-01\",\n"
            + "      \"name\": \"Test Hedge Fund\",\n"
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
    private static HttpClient httpClient = new HttpClient();
    private static string authorizationToken = null;
    public static void Main(string[] args)
    {
        SimpleApiApplication apiExample = new SimpleApiApplication();

        // Forces the use of TLS 1.2
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var response = apiExample.login();
        Console.Write(response);

        response = apiExample.sendApiRequest(API_PATH_PEOPLE, HTTP_METHOD_GET, null);
        Console.Write(response);

        //response = apiExample.sendApiRequest(API_PATH_HEDGE_FUNDS, HTTP_METHOD_POST, API_POST_HEDGE_FUND_BODY);
        //Console.Write(response);

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
        var request = new HttpRequestMessage
        {
            Content = string.IsNullOrWhiteSpace(requestBody) ? null : new StringContent(requestBody, Encoding.UTF8, CONTENT_TYPE_HEADER),
            Method = new HttpMethod(httpMethod),
            RequestUri = new Uri(SERVICE_URL + requestUrl)
        };
        string token;
        if (useToken)
        {
            token = authorizationToken;
            request.Headers.TryAddWithoutValidation("token", "true");
        }
        else
        {
            token = PASS_WORD;
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(USER_NAME + ":" + token)));
        request.Headers.TryAddWithoutValidation("Accept", CONTENT_TYPE_HEADER);
        request.Headers.Add("User-Agent", "BackstopAPIClient");

        var response = httpClient.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }

}
