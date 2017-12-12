using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Mime;

/**
 * Demonstrate how to upload and download document
 *
 * To get it work, please make sure
 * 1. You have contacted Backstop support team and properly set up API access
 * 2. Replace all parameters in angle brackets, e.g. {HOST_NAME}
 * 3. Runing this application will do the following
 * 3.1 Upload (POST) a document to the person designated by {PERSON_ID}
 * 3.2 Download the uploaded document to a file in the same directory of the file to be uploaded
 *
 */
class DocumentApiApplication
{
    private static string HOST_NAME = "{HOST_NAME}";// e.g. demo01.backstopsolutions.com
    private static String USER_NAME = "{USER_NAME}";// The user name with API access
    private static String PASSWORD = "{PASSWORD}";// The backstop ID of the person you want to upload a document
    private static String PERSON_ID = "{PERSON_ID}";
    private static String UPLOAD_DOCUMENT_NAME = "{UPLOAD_DOCUMENT_NAME}"; // e.g "product.doc";
    private static String UPLOAD_DOCUMENT_PATH = "{UPLOAD_DOCUMENT_PATH}"; // e.g "C:\product.doc";
    private static String SERVICE_URL = "http://" + HOST_NAME;
    /**
     * The backstop ID of the document to be downloaded. Note the URL to
     */
    private static String DOWNLOAD_DOCUMENT_ID = "{DOWNLOAD_DOCUMENT_ID}";
    /**
     * The folowing URL is also avaiable from the documents resource.
     */
    private static String DOWNLOAD_DOCUMENT_PATH = "/backstop/api-dynamic-documents?docId=" + DOWNLOAD_DOCUMENT_ID;
    private static String LOGIN_PATH = "/backstop/api/login";
    private static String API_PATH_DOCUMENTS = "/backstop/api/documents";
    private static String API_POST_DOCUMENT_BODY = "{\n"
            + "  \"data\": {\n"
            + "    \"type\":\"documents\",\n"
            + "    \"attributes\": {\n"
            + "      \"attachedTo\": {\n"
            + "          \"resourceId\": \"" + PERSON_ID + "\",\n"
            + "          \"resourceType\": \"people\"\n"
            + "        },\n"
            + "      \"data\": \"{DATA}\",\n"
            + "      \"description\": \"Test Upload\",\n"
            + "      \"documentName\": \"{DOCUMENT_NAME}\",\n"
            + "      \"effectiveDate\": \"2017-01-01\",\n"
            + "      \"title\": \"Test Upload\"\n"
            + "    }\n"
            + "  }\n"
            + "}";

    private static String METHOD_GET = "GET";
    private static String METHOD_POST = "POST";
    private static string CONTENT_TYPE_HEADER = "application/vnd.api+json";
    private static string HTTP_METHOD_GET = "GET";
    private static string HTTP_METHOD_POST = "POST";
    private static string authorizationToken = null;
    private static HttpClient httpClient = new HttpClient();

    public static void Main(string[] args)
    {
        DocumentApiApplication apiExample = new DocumentApiApplication();

        // Login
        apiExample.login();

        // Upload document
        apiExample.uploadDocument();

        // Download document
        apiExample.downloadDocument();

        Console.Write("done.");
        Console.ReadLine();
    }

    /**
     * Login with basic authentication and obtain/keep the authentication token for future communication
     */
    public void login()
    {
        authorizationToken = sendHttpRequest(LOGIN_PATH, HTTP_METHOD_POST, "", false);
    }
    private string sendApiRequest(string apiPath, string httpMethod, string requestBody)
    {
        return sendHttpRequest(apiPath, httpMethod, requestBody, true);
    }
    private string zipAndEncode(byte[] bytes)
    {
        using (var ms = new MemoryStream())
        using (var gzip = new GZipStream(ms, CompressionMode.Compress))
        {
            gzip.Write(bytes, 0, bytes.Length);
            gzip.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
    }


    /**
     * Upload document to designated person
     */
    public string uploadDocument()
    {
        var fileBytes = File.ReadAllBytes(UPLOAD_DOCUMENT_PATH);
        var requestBody = API_POST_DOCUMENT_BODY.Replace("{DOCUMENT_NAME}", UPLOAD_DOCUMENT_NAME).Replace("{DATA}", zipAndEncode(fileBytes));
        return sendApiRequest(API_PATH_DOCUMENTS, METHOD_POST, requestBody);
    }
    /**
     * Download document and save to the same directory as that for upload
     */
    private void downloadDocument()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(@SERVICE_URL + DOWNLOAD_DOCUMENT_PATH)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(USER_NAME + ":" + authorizationToken)));
        request.Headers.TryAddWithoutValidation("token", "true");
        var response = httpClient.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();
        var fileName = response.Content.Headers.ContentDisposition.FileName.Replace("\"", "");
        File.WriteAllBytes(fileName, response.Content.ReadAsByteArrayAsync().Result);
    }

    private string sendHttpRequest(string requestUrl, string httpMethod, string requestBody, bool useToken)
    {
        var request = new HttpRequestMessage
        {
            Content = new StringContent(requestBody, Encoding.UTF8, CONTENT_TYPE_HEADER),
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
        var response = httpClient.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStringAsync().Result;
    }

}