using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace RestClient
{
    public class BaseRestApiClient
    {        
        private static readonly JsonNetSerializer _serializer = new JsonNetSerializer();
        private static readonly string _url = ConfigurationManager.AppSettings["serverUrl"];

        public async Task<T> SendRequest<T>(string path, Method method = Method.GET, string token = null, object body = null) where T: new()
        {
            var client = new RestSharp.RestClient { BaseUrl = new Uri(_url) };
            if (token != null)
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            var request = new RestRequest(path, method) { RequestFormat = DataFormat.Json, JsonSerializer = _serializer };
            if (body != null)
                request.AddBody(body);

            var response = await client.ExecuteTaskAsync<T>(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }        
    }
}
