using AutoMapper;
using IPProje.Models;
using IPProje.ViewModels.Articles;

namespace IPProje.Extensions.AutoMapper
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<ArticleModel, Article>().ReverseMap();
            CreateMap<ArticleUpdateModel, Article>().ReverseMap();
            CreateMap<ArticleUpdateModel, ArticleModel>().ReverseMap();
            CreateMap<ArticleAddModel, Article>().ReverseMap();
        }
    }
}
