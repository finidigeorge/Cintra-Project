using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;

namespace RestApi
{
    public class BaseRestApiClient
    {
        private readonly RestClient _client;
        private readonly JsonNetSerializer _serializer = new JsonNetSerializer();
        private readonly string _url = ConfigurationManager.AppSettings["serverUrl"];

        public BaseRestApiClient()
        {
            _client = new RestClient { BaseUrl = new Uri(_url) };            
        }

        public async Task<T> SendRequest<T>(string path, Method method = Method.GET) where T: new()
        {
            var request = new RestRequest(path, method) { RequestFormat = DataFormat.Json, JsonSerializer = _serializer };

            var response = await _client.ExecuteTaskAsync<T>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }
}
