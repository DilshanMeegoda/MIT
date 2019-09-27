using System.Collections.Generic;

namespace WorkFlowManagement.Model
{
    public class UserCompany
    {
        public string CompanyName { get; set; }
        public List<User> Users { get; set; }
    }
}