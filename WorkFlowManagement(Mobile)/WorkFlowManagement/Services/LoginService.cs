using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using WorkFlowManagement.Services.Mapper;

namespace WorkFlowManagement.Services
{
    public class LoginService : BaseService, ILoginService
    {
        private const string Token = "Token";
        private const string Url = Configuration.BaseUrl + "Authenticate/";

        public LoginService()
        {
            Client.BaseAddress = new Uri(Url);
        }

        public async Task<UserSession> GetValidUserAsync(LoginDto loginDto)
        {
            var response = await Client.PostAsJsonAsync("AuthenticateUser", loginDto);
            if (response.IsSuccessStatusCode)
            {
                var userSessionDto = await response.Content.ReadAsAsync<UserSessionDto>();
                var userSession = userSessionDto.ToUserSession();
                var accessToken = response.Headers.GetValues(Token).FirstOrDefault();
                if (accessToken != null)
                {
                    userSession.AccessToken = accessToken;
                }

                return userSession;
            }
            return null;
        }

        public Task<HttpResponseMessage> GetCompanyImage(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}