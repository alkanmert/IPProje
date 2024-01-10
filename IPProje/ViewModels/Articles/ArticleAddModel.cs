using IPProje.ViewModels.Categories;

namespace IPProje.ViewModels.Articles
{
    public class ArticleAddModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }

        public IFormFile Photo { get; set; }

        public IList<CategoryModel> Categories { get; set; }
    }
}
