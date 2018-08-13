using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace WPFRxNet.API
{
    public class RemoteApi
    {
        private const string BaseUrl = "https://api.github.com/";

        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            var client = new RestClient(BaseUrl);

            var response = await client.ExecuteTaskAsync<T>(request);
            
            if (response.ErrorException != null)
            {
                throw new Exception("Error in retriving the response", response.ErrorException);
            }

            var data = JsonConvert.DeserializeObject<T>(response.Content);

            return data;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient(BaseUrl);

            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                throw new Exception("Error in retriving the response", response.ErrorException);
            }


            return response.Data;
        }
    }
}
