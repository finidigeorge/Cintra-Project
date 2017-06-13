using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using Shared.Dto;

namespace RestClient
{
    public class BaseRestApiClient
    {        
        private static readonly JsonNetSerializer _serializer = new JsonNetSerializer();
        private static readonly string _url = ConfigurationManager.AppSettings["serverUrl"];

        public async Task<T> SendRequest<T>(string path, Method method = Method.GET, object body = null) where T: new()
        {
            var client = new RestSharp.RestClient { BaseUrl = new Uri(_url) };

            var token = AuthProvider.GetToken()?.AccessToken;
            if (token != null)
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            var request = new RestRequest(path, method) { RequestFormat = DataFormat.Json, JsonSerializer = _serializer };
            if (body != null)
                request.AddBody(body);

            var response = await client.ExecuteTaskAsync<T>(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"Requested path {path} not found");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = JsonConvert.DeserializeObject<ErrorMessageDto>(response.Content);
                throw new Exception(error.Error);
            }

            return response.Data;
        }        
    }
}
