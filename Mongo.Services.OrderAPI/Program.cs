using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mongo.MessageBus;
using Mongo.Services.OrderAPI.Data;
using Mongo.Services.OrderAPI.Extension;
using Mongo.Services.OrderAPI.Services;
using Mongo.Services.OrderAPI.Services.Iservice;
using Mongo.Services.ShoppingCartAPI;
using PayStack.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// mapper
IMapper mapper = MapingConfig.ResisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton<IPayStackApi>(new PayStackApi(builder.Configuration.GetValue<string>("PayStack:ApiKey")));



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddSwaggerGenExtention();
builder.AddAuthenticationExtension();
builder.Services.AddAuthorization();
builder.AddHttpClientExtention();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
