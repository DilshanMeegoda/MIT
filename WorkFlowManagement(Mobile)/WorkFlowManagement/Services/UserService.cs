using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkFlowManagement.Common;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;
using WorkFlowManagement.Services.Interfaces;
using WorkFlowManagement.Services.Mapper;

namespace WorkFlowManagement.Services
{
    public class UserService :BaseService, IUserService
    {
        private const string Url = Configuration.BaseUrl + "User/";

        public UserService(string token)
            : base(token)
        {
            Client.BaseAddress = new Uri(Url);
        }

        public async Task<IEnumerable<User>> SyncUsers(int projectId)
        {
            var lastSyncDateTime = DefaultSyncDateTime;
            var converteddate = DateTime.SpecifyKind(lastSyncDateTime, DateTimeKind.Utc);
            var utcSyncdate = converteddate.ToLocalTime().ToString(CultureInfo.InvariantCulture);

            var response = await Client.GetAsync(string.Format("SyncUsers?projectId={0}&lastSyncDateTime={1}", projectId, utcSyncdate));
            //response.CustomeEnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var userLiteDtoList = await response.Content.ReadAsAsync<IEnumerable<UserLiteDto>>();
                return userLiteDtoList.ToUserList(projectId).ToList();
            }
            return new List<User>();
        }
    }
}