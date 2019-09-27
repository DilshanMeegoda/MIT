using System.Collections.Generic;
using System.Threading.Tasks;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> SyncUsers(int projectId);
    }
}