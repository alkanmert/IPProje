using IPProje.Models;
using IPProje.ViewModels.Categories;

namespace IPProje.ViewModels.Articles
{
    public class ArticleUpdateModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }

        public Image Image { get; set; }
        public IFormFile? Photo { get; set; }

        public IList<CategoryModel> Categories { get; set; }
    }
}
