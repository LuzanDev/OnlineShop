using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }

}
