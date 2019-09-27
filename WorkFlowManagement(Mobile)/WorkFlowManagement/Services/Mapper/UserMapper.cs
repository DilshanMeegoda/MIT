using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Services.Mapper
{
    public static class UserMapper
    {
        internal static IEnumerable<User> ToUserList(this IEnumerable<UserLiteDto> userLiteDtoList, int projectId)
        {
            IList<User> userList = new List<User>();
            foreach (var userLiteDto in userLiteDtoList)
            {
                var user = new User();
                user.Id = userLiteDto.UserId;
                user.UserId = userLiteDto.UserId;
                user.ProjectId = projectId;
                user.FullName = userLiteDto.FullName ?? string.Empty;
                user.Email = userLiteDto.Email;
                user.UserRole = userLiteDto.UserRole;
                user.ProjectUserRole = userLiteDto.ProjectUserRole;
                user.ImageName = userLiteDto.ImageName;
                user.CompanyName = userLiteDto.CompanyName;
                user.IsActive = userLiteDto.IsActive;
                user.ModifiedDateTime = userLiteDto.ModifiedDateTime;
                userList.Add(user);
            }
            return userList;
        }
    }
}