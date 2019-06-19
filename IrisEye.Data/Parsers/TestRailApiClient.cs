using IrisEye.Core.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace IrisEye.Data.Parsers
{
    public class TestRailApiClient
    {
        private readonly RestClient _restClient;    
        private const string BaseUrl = "https://testrail.stage.mozaws.net";

        public TestRailApiClient()
        {
            _restClient = new RestClient(BaseUrl)
            {
                Authenticator = new HttpBasicAuthenticator("", ""), 
                UserAgent = "Iris"
            };
            _restClient.AddDefaultHeader("Content-Type", "application/json");
        }

        public TestRailTest GetTest(int id)
        {
            var request = new RestRequest("/index.php?/api/v2/get_case/"+id, Method.GET);
            var jsonContent = _restClient.Execute(request).Content;
            return JsonConvert.DeserializeObject<TestRailTest>(jsonContent);
        }
        
        
        
        

    }
}