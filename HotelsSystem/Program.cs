using HotelsSystem.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace HotelsSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IHotelsService, HotelSearchService>();

            builder.Host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console(new CompactJsonFormatter())
                //.WriteTo.Seq("http://localhost:5341")
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application_name", nameof(HotelsSystem))
                    .Enrich.WithCorrelationIdHeader("my-custom-correlation-id")
                  );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}