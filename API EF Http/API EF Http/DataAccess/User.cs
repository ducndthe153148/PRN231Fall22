using Microsoft.AspNetCore.Identity;

namespace API_EF_Http.DataAccess
{
    public class User: IdentityUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
