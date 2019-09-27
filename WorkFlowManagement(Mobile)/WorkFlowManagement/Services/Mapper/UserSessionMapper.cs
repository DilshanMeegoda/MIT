using WorkFlowManagement.Model;
using WorkFlowManagement.Services.Dto;

namespace WorkFlowManagement.Services.Mapper
{
    public static class UserSessionMapper
    {
        internal static UserSession ToUserSession(this UserSessionDto userSessionDto)
        {
            return new UserSession
            {
                UserId = userSessionDto.UserSessionId,
                Email = userSessionDto.Email,
                FullName = userSessionDto.FullName,
                Password = userSessionDto.Password,
                Role = userSessionDto.Role,
                UserImage = userSessionDto.UserImage,
            };

        }
    }
}