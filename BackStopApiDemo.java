import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.InputStream;
import java.io.PrintWriter;
import java.net.URL;
import java.net.URLConnection;
import java.util.Base64;
import java.util.Collection;
import java.util.List;
import java.util.Map;

/***********************************************
 *** Created by william on 12/1/17.
 ***********************************************/
public class BackStopApiDemo {
    private static final String HOST_NAME = "{HOST_NAME}"; //{HOST_NAME} // 192.168.10.235:8080
    private static final String USER_NAME = "{USER_NAME}"; //{USER_NAME} // bsg4
    private static final String PASSWORD = "{PASSWORD}";   //{PASSWORD}  // rup3rt
    private static final String SERVICE_URL = "http://" + HOST_NAME;
    private static final String LOGIN_PATH = "/backstop/api/login";
    private static final String API_PATH_PEOPLE = "/backstop/api/people";
    private static final String API_PATH_HEDGE_FUNDS = "/backstop/api/hedge-funds/";
    private static final String METHOD_GET = "GET";
    private static final String METHOD_POST = "POST";
    private String SYSTEM_API_TOKEN="";

    private static final String ORGANIZATION_ID = "6792"; // You need to update {ORGANIZATION_ID} before you try POST hedge fund (currently commented out)
    private static final String API_POST_HEDGE_FUND_BODY = "{\n"
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
    private void login() throws Exception {
        this.SYSTEM_API_TOKEN = sendRequest(LOGIN_PATH, METHOD_POST, null, false); // POST http request
    }
    private String sendApiRequest(String apiPath, String httpMethod, String requestBody) throws Exception {
        return this.sendRequest(apiPath, httpMethod, requestBody, true);
    }
    private String sendRequest(String requestPath, String httpMethod, String requestBody, boolean useAuthenticationToken) throws Exception {
        URL url = new URL(this.SERVICE_URL + requestPath);
        URLConnection connection = url.openConnection();
        String argAuthorization="";
        if(useAuthenticationToken) {
            argAuthorization=this.SYSTEM_API_TOKEN;
            connection.setRequestProperty("token","true");
        }else{
            argAuthorization=this.PASSWORD;
        }
        connection.setRequestProperty("Authorization","Basic "+new String(Base64.getEncoder().encode((this.USER_NAME+":"+argAuthorization).getBytes())));
        return sendHttpRequest(connection,httpMethod,requestBody);
    }
    private String sendHttpRequest(URLConnection connection, String httpMethod, String requestBody) throws Exception {
        final String JSON_API_CONTENT_TYPE = "application/vnd.api+json";
        connection.setRequestProperty("Accept", JSON_API_CONTENT_TYPE);
        boolean isGetMethod = METHOD_GET.equals(httpMethod);
        if (!isGetMethod) {
            connection.setRequestProperty("Content-type", JSON_API_CONTENT_TYPE);
            connection.setDoInput(true);
            connection.setDoOutput(true);
            PrintWriter out = new PrintWriter(connection.getOutputStream());
            out.flush();
        }
        connection.connect();
        InputStream in = connection.getInputStream();
        StringBuffer result=new StringBuffer();
        String line = null;
        while ((line = new BufferedReader(new InputStreamReader(in)).readLine())!= null)  {
            result.append(line);
        }
        in.close();
        System.out.println("------------------------------------------------");
        System.out.println("request url    ===>>>>>> "+connection.getURL());
        System.out.println("request result ===>>>>>> "+result);
        System.out.println("------------------------------------------------");
        return result.toString();
    }
    public static void main(String[] args) throws Exception {
        BackStopApiDemo apiDemo = new BackStopApiDemo();
        apiDemo.login();
        apiDemo.sendApiRequest(API_PATH_PEOPLE, METHOD_GET, null); // GET People
        //apiDemo.sendApiRequest(API_PATH_HEDGE_FUNDS,METHOD_POST,API_POST_HEDGE_FUND_BODY); // POST (Create) a hedge fund. Please make sure you update the corresponding parameters before trying this out.
    }
}
