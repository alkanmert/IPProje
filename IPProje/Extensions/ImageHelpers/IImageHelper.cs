using IPProje.Models;
using IPProje.ViewModels.Images;

namespace IPProje.Extensions.ImageHelpers
{
    public interface IImageHelper
    {
        Task<ImageUploadedModel> Upload(string name, IFormFile imageFile, ImageType imageType, string folderName = null);
        void Delete(string imageName);
    }
}
