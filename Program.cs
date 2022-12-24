using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Savana.Product.API.Data;
using Savana.Product.API.Interfaces;
using Savana.Product.API.Services;
using Treasures.Common.Extensions;
using Treasures.Common.Interfaces;
using Treasures.Common.Middlewares;
using Treasures.Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}).ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<StoreContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!,
        c => c.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});
//Inject services from Treasures.Common library
builder.Services.AddScoped(typeof(ISqlRepository<>), typeof(SqlRepository<>));
builder.Services.AddScoped<IUnitOfWork>(s => new UnitOfWork(s.GetService<StoreContext>()!));
builder.Services.AddJwtAuthentication(builder.Configuration["Token:Key"], builder.Configuration["Token:Issuer"]);
builder.Services.AddScoped<ICloudinaryService>(_ =>
    new CloudinaryService(builder.Configuration["Cloudinary:CloudName"],
        builder.Configuration["Cloudinary:ApiKey"], builder.Configuration["Cloudinary:ApiSecret"]
    )
);
builder.Services.AddSwaggerAuthenticated("Savana Product API Service", "v1");
builder.Services.AddErrorResponse<ApiBehaviorOptions>();

builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddRouting(opt => {
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = long.MaxValue; });

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope()) {
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
    } catch (Exception ex) {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred during Product DB migration");
    }
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
    { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto }
);
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment()) app.UseSavanaSwaggerDoc("Savana Product API Service v1");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();