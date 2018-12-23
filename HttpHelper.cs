using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomDataProvider_Demo
{
  
    public class HttpHelper
    {
        public static HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();          
           // client.DefaultRequestHeaders.Add("Connection", "close");
            return client;
        }

    
        public static async Task<OData> GetAPIdata(string externalwebapiurl)

        {
            using (HttpClient client = HttpHelper.GetHttpClient())
            {

                // string url = "http://winstonodata4.azurewebsites.net/Students";  
               // string url = string.Format(externalwebapiurl + "/" + "Students");
                                
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(externalwebapiurl));
                HttpResponseMessage response = await client.SendAsync(request);                

                if (!response.IsSuccessStatusCode)
                    throw new Exception("My API stopped this from happening");

                string json = response.Content.ReadAsStringAsync().Result;

                OData odata = JsonConvert.DeserializeObject<OData>(json);
                return odata;
            }
        }

        public static async Task<QueryResult> GetAPIdatabyID(string externalwebapiurl)

        {
            using (HttpClient client = HttpHelper.GetHttpClient())
            {

                // string url = "http://winstonodata4.azurewebsites.net/Students/("+objectId+")";  
             //   string url = string.Format(externalwebapiurl + "/" + "Students");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(externalwebapiurl));
                HttpResponseMessage response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("My API stopped this from happening");

                string json = response.Content.ReadAsStringAsync().Result;

                JObject jobject = JObject.Parse(json);
                QueryResult odata = new QueryResult();
                odata.StudentId = (Guid)jobject["StudentId"];
                odata.name= (string)jobject["name"];
                odata.age = (Int32)jobject["age"];
                odata.familyaddress = (string)jobject["familyaddress"];
                odata.grade = (Int32)jobject["grade"];
                odata.phoneno = (string)jobject["phoneno"];
                return odata;
            }
        }
    }
}
