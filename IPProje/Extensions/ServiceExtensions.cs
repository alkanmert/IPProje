using FluentValidation.AspNetCore;
using IPProje.Extensions.FluentValidations;
using IPProje.Extensions.ImageHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Reflection;

namespace IPProje.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection LoadServiceLayerExtension(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddScoped<IImageHelper, ImageHelper>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper(assembly);


            services.AddControllersWithViews()
                .AddFluentValidation(opt =>
                {
                    opt.RegisterValidatorsFromAssemblyContaining<ArticleValidator>();
                    opt.DisableDataAnnotationsValidation = true;
                    opt.ValidatorOptions.LanguageManager.Culture = new CultureInfo("tr");
                });

            return services;
        }
    }
}
