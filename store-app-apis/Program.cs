

//https://www.youtube.com/watch?v=zNRVz7dgfuE           Complete .NET Core Web API Tutorial with JWT Token | Learn from Scratch





using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Container;
using store_app_apis.Helper;
using store_app_apis.Repos;
using store_app_apis.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService,CustomerService>();
builder.Services.AddDbContext<LearnDataContext>(o=>o.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase")));
var automapper=new MapperConfiguration(item  =>item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper(); 
builder.Services.AddSingleton(mapper);  
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();
 
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
