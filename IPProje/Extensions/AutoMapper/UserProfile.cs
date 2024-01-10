using AutoMapper;
using IPProje.Models;
using IPProje.ViewModels.Users;

namespace IPProje.Extensions.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<AppUser, UserModel>().ReverseMap();
            CreateMap<AppUser, UserAddModel>().ReverseMap();
            CreateMap<AppUser, UserUpdateModel>().ReverseMap();
            CreateMap<AppUser, UserProfileModel>().ReverseMap();
        }
    }
}
