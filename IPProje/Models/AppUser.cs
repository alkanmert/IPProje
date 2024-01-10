using IPProje.EntityBase;
using Microsoft.AspNetCore.Identity;

namespace IPProje.Models
{
    public class AppUser : IdentityUser<Guid>, IEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid ImageId { get; set; } = Guid.Parse("259cd056-5e69-491f-a4cb-9f94db863edf");
        public Image Image { get; set; } 

        public ICollection<Article> Articles { get; set; }
    }
}
