using IPProje.Models;

namespace IPProje.ViewModels.Users
{
    public class UserAddModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public Guid RoleId { get; set; }
        public List<AppRole> Roles { get; set; }
    }
}
