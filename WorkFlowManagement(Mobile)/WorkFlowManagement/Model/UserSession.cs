namespace WorkFlowManagement.Model
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string UserImage { get; set; }
        public string AccessToken { get; set; }
    }
}