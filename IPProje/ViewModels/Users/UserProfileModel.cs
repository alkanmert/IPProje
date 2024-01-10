using IPProje.Models;

namespace IPProje.ViewModels.Users
{
    public class UserProfileModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile? Photo { get; set; }
        public Image Image { get; set; }
        public string Role { get; set; }
    }
}
