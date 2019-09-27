using System.Net.Http;
using System.Threading.Tasks;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Services.Interfaces
{
    public interface ILoginService
    {
        Task<UserSession> GetValidUserAsync(LoginDto loginDto);
        Task<HttpResponseMessage> GetCompanyImage(string fileName);
    }
}