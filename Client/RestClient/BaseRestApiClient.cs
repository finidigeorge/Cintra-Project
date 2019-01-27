using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using Shared.Dto;
using Shared.Interfaces;

namespace RestClient
{    
    public class BaseRestApiClient<T> : IBaseController<T> where T : class
    {        
        private static readonly JsonNetSerializer _serializer = new JsonNetSerializer();
        private static readonly string _url = ConfigurationManager.AppSettings["serverUrl"];

        protected readonly string ControllerName;

        public BaseRestApiClient(string controllerName)
        {
            ControllerName = controllerName;
        }

        protected async Task<T1> SendRequest<T1>(string path, Method method = Method.GET, object body = null)
        {
            var client = new RestSharp.RestClient { BaseUrl = new Uri(_url) };

            var token = AuthProvider.GetToken()?.AccessToken;
            if (token != null)
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            var request = new RestRequest(path, method) { RequestFormat = DataFormat.Json, JsonSerializer = _serializer };
            if (body != null)
                request.AddJsonBody(body);

            var response = await client.ExecuteTaskAsync<T1>(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"Requested path {path} not found");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {                
                throw new Exception($"{response.ErrorMessage} {response.Content}");
            }

            return response.Data;
        }

        public async Task Delete(long id)
        {
            await SendRequest<T>($"api/{ControllerName}/values/{id}", Method.DELETE, id);
        }

        public async Task<List<T>> GetAll()
        {
            return await SendRequest<List<T>>($"api/{ControllerName}/values");
        }

        public async Task<T> GetById(long id)
        {
            return await SendRequest<T>($"api/{ControllerName}/values/{id}");
        }

        public async Task<long> Create(T entity)
        {
            return await SendRequest<long>($"api/{ControllerName}/values", Method.POST, entity);
        }        
    }
}
