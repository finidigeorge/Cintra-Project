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

        private readonly string controllerName;

        public BaseRestApiClient(string controllerName)
        {
            this.controllerName = controllerName;
        }

        protected async Task<T1> SendRequest<T1>(string path, Method method = Method.GET, object body = null)
        {
            var client = new RestSharp.RestClient { BaseUrl = new Uri(_url) };

            var token = AuthProvider.GetToken()?.AccessToken;
            if (token != null)
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            var request = new RestRequest(path, method) { RequestFormat = DataFormat.Json, JsonSerializer = _serializer };
            if (body != null)
                request.AddBody(body);

            var response = await client.ExecuteTaskAsync<T1>(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"Requested path {path} not found");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {                
                var error = JsonConvert.DeserializeObject<ErrorMessageDto>(response.Content);
                throw new Exception(string.IsNullOrEmpty(response.ErrorMessage) ? error?.Error : response.ErrorMessage);
            }

            return response.Data;
        }

        public async Task Delete(T entity)
        {
            await SendRequest<T>($"api/{controllerName}/values", Method.DELETE, entity);
        }

        public async Task<List<T>> GetAll()
        {
            return await SendRequest<List<T>>($"api/{controllerName}/values");
        }

        public async Task<T> GetById(long id)
        {
            return await SendRequest<T>($"api/{controllerName}/values/{id}");
        }

        public async Task<long> Insert(T entity)
        {
            return await SendRequest<long>($"api/{controllerName}/values", Method.POST, entity);
        }

        public async Task Update(T entity)
        {
            await SendRequest<T>($"api/{controllerName}/values", Method.PUT, entity);
        }
    }
}
