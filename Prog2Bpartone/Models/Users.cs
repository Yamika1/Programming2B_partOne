using Microsoft.AspNetCore.Identity;

namespace Prog2Bpartone.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }

    }
}
