using IPProje.EntityBase;
using Microsoft.AspNetCore.Http.HttpResults;

namespace IPProje.Models
{
    public class Category : Entity
    {
        public Category()
        {
            
        }

        public Category(string name, string createdBy)
        {
            Name = name;
            CreatedBy = createdBy;
        }


        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}
