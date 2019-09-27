using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WorkFlowManagement.Services
{
    public class BaseService
    {
        public DateTime DefaultSyncDateTime = new DateTime(2000, 1, 1, 12, 00, 00);
        public readonly HttpClient Client;
        public BaseService()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public BaseService(string token) : this()
        {
            Client.DefaultRequestHeaders.Add("Token", token);
        }
    }
}