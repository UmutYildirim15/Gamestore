using System.Text;
using Business.Facades;
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.OpenApi.Models;
using Web.Middleware;
using Business.Services.Abstract;
using Business.Services.Concrete;
using Data.Repositories.Abstract;
using Data.Repositories.Concrete;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Web.Logs;
using Web.Models.Validators.GameValidators;
using Web.Models.Validators.GenreValidators;
using Web.Models.Validators.PlatformValidators;
using Newtonsoft.Json;
using Northwind.Business.Services.Abstract;
using Northwind.Business.Services.Concrete;
using Northwind.Data;
using Northwind.Data.Logger;
using Northwind.Data.Repositories.Abstract;
using Northwind.Data.Repositories.Concrete;
using Web.Models.Validators.OrderValidators;
using Web.Models.Validators.PublishValidators;
using IOrderRepository = Data.Repositories.Abstract.IOrderRepository;
using OrderRepository = Data.Repositories.Concrete.OrderRepository;


var builder = WebApplication.CreateBuilder(args);

// Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<MongoClient>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<MongoDbContext>();

// Configure Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
// Configure Cache Profiles for Controllers
builder.Services.AddControllers(opts =>
{
    opts.CacheProfiles.Add("OneMinuteCache", new CacheProfile
    {
        Duration = 60,
        Location = ResponseCacheLocation.Any,
        NoStore = builder.Environment.IsDevelopment()
    });
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.MaxDepth = 64;
});


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<GameCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GenreCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PlatformCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<OrderCreateValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PublisherCreateValidator>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure Scoped Services
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IGameGenreRepository, GameGenreRepository>();
builder.Services.AddScoped<IGamePlatformRepository, GamePlatformRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<GameRepositoryFacade>();

// MongoDb services and repositories
builder.Services.AddScoped<IMongoContext, MongoDbContext>();
builder.Services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IShipperRepository, ShipperRepository>();
builder.Services.AddScoped<IEntityLoggerRepository, EntityLoggerRepository>();
builder.Services.AddScoped<IEntityLogger, EntityLogger>();
builder.Services.AddScoped<Northwind.Data.Repositories.Abstract.IOrderRepository, Northwind.Data.Repositories.Concrete.OrderRepository>();
builder.Services.AddScoped<IMongoSupplierService, MongoSupplierService>();
builder.Services.AddScoped<IMongoProductService, MongoProductService>();
builder.Services.AddScoped<IMongoCategoryService, MongoCategoryService>();
builder.Services.AddScoped<IMongoOrderService, MongoOrderService>();
builder.Services.AddScoped<IMongoShipperService, MongoShipperService>();
builder.Services.AddScoped<OrderDataAccess>();


// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// Configure Logging
builder.Logging.AddProvider(new FileLoggerProvider("Logs", LogLevel.Information));

builder.Logging.AddConsole();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AuthService"] ?? throw new InvalidOperationException("AuthService URL missing"));
});




builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("Missing JWT Secret")))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<HeaderMiddleware>();
    app.UseMiddleware<LoggingMiddleware>();
    //app.UseMiddleware<PlaceholderReplacerMiddleware>();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});



app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll"); 
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

