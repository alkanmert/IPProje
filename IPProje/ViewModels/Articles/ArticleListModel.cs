using IPProje.Models;

namespace IPProje.ViewModels.Articles
{
    public class ArticleListModel
    {
        public IList<Article> Articles { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
