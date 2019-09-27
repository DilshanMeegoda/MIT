using WorkFlowManagement.Enum;

namespace WorkFlowManagement.Services.Dto
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
        public DeviceType DeviceType { get; set; }
        public AppType AppType { get; set; }
    }
}