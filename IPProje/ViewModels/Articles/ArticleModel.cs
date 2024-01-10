using IPProje.Models;
using IPProje.ViewModels.Categories;

namespace IPProje.ViewModels.Articles
{
    public class ArticleModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryModel Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public Image Image { get; set; }
        public AppUser User { get; set; }
        public string CreatedBy { get; set; }
        public int ViewCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
