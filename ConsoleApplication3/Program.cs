using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Rest.Azure.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication3
{
	internal class Program
	{
		public static void Main(string[] args)
        {
            string code = "uk";
            // NOTE: Replace this example key with a valid subscription key.
            string key = "a0c7e6538e1442c78ab4339dc4b259b4";
            //host url
            string host = "https://api.cognitive.microsofttranslator.com";
            string path = "/translate?api-version=3.0";
            // Translate to dropdown selected language.
            string params_ = $"&to={code}";

            string uri = host + path + params_;

            string text = "hello";
            
            var resultText = Translate(uri, text, key).Result;
        }
        
        public static async Task<string> Translate(string uri, string text, string key)  
        {  
            System.Object[] body = new System.Object[] { new { Text = text } };  
            var requestBody = JsonConvert.SerializeObject(body);  
                
            using (var client = new HttpClient())  
            using (var request = new HttpRequestMessage())  
            {  
                request.Method = HttpMethod.Post;  
                request.RequestUri = new Uri(uri);  
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");  
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);  
    
                var response = await client.SendAsync(request);  
                var responseBody = await response.Content.ReadAsStringAsync();  
                dynamic result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented);  
                    
                return result;
            }  
        }  
	}
}