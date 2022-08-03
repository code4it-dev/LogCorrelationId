using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Common
{
    public static class HeaderPropagationExtensions
    {
        public static IServiceCollection AddHeaderPropagation(this IServiceCollection services, Action<HeaderPropagationOptions> configure)
        {
            services.AddHttpContextAccessor();
            services.ConfigureAll(configure);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, HeaderPropagationMessageHandlerBuilderFilter>());
            return services;
        }

        public static IHttpClientBuilder AddHeaderPropagation(this IHttpClientBuilder builder, Action<HeaderPropagationOptions> configure)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure(builder.Name, configure);
            builder.AddHttpMessageHandler((sp) =>
            {
                var options = sp.GetRequiredService<IOptionsMonitor<HeaderPropagationOptions>>();
                var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                return new HeaderPropagationMessageHandler(options.Get(builder.Name), contextAccessor);
            });

            return builder;
        }
    }
}