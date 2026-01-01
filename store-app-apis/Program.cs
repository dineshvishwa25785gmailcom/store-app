using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using store_app_apis.Container;
using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Service;
using System.Text;
using store_app_apis.Repos;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
string logpath = builder.Configuration.GetSection("Logging:LogPath").Value ?? "logs/log.txt";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(logpath)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

Log.Information("Starting up the application");

var _jwtsettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsettings);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();
builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.AddTransient<IMasterContainer, MasterContainer>();
//builder.Services.AddTransient<IInvoiceContainer, InvoiceContainer>();
builder.Services.AddTransient<IProductService, ProductContainer>();
builder.Services.AddTransient<IInvoiceContainer, InvoiceContainer>();

//builder.Services.AddScoped<InvoiceDocumentDataSource>();
builder.Services.AddDbContext<StoreAppContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabase")));


builder.Services.AddScoped<InvoiceRepository>();
builder.Services.AddScoped<InvoiceService>();








// we successfully registred our basic authenication handeler
//builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
// for jwt token we have to use jwt bearer nuget package: now we have registerd JWT token in our middleware/service
var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
var key = Encoding.ASCII.GetBytes(_authkey);

builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,/// If this false it means you will get Unable to verify the first certificate in post man and a-string-secret-at-least-256-bits-long

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);


/// Note: If we want to allow to access my API to all of the domain or application then we can use this code. *  e.g.  build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
// We have defined CORS Policy 
////builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
////{
////    /// build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
////    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
////}));

// We can have diffrent  cors policy for diffrent controller

////builder.Services.AddCors(p => p.AddPolicy("corspolicy1", build =>
////{
////    /// build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
////    build.WithOrigins("http://localhost:4300").AllowAnyMethod().AllowAnyHeader();
////}));

/////// For Default policy we can use this code
////builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
////{
////    /// build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
////    build.WithOrigins("http://localhost:4300").AllowAnyMethod().AllowAnyHeader();
////}));
// If we want to include this CORS policy for one of our controller or action methods then we can use this attribute: corspolicy1 

builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddCors(p => p.AddPolicy("corspolicy1", build =>
{
    build.WithOrigins("https://localhost:86").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));



builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedwindow", option =>
{
    option.Window = TimeSpan.FromSeconds(10);
    option.PermitLimit = 1;
    option.QueueLimit = 0;
    option.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode = 401);
//If we want any specific status code error we need to use '.RejectionStatusCode=401'
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();


var app = builder.Build();
// Minumal API Sample//
app.MapGet("/minimalapi", () => "Hello World!");
app.MapGet("/getchannel", (string channelname) => "Welcome to " + channelname);
//app.MapGet("/getchannel", (string channelname) => "Welcome to " + channelname)
//    .WithOpenApi(opt => {
//        var parameter = opt.Parameters.FirstOrDefault(p => p.Name == "channelname");
//        if (parameter != null)
//        {
//            parameter.Description = "The name of the channel";
//        }
//        return opt;
//    });

app.MapGet("/getcustomer", async (store_app_apis.Repos.StoreAppContext db) =>
{
    return await db.TblCustomers.ToListAsync();
});

//app.MapGet("/getcustomerbycode/{code}", async (StoreAppContext db, string code) =>
//{
//    return await db.TblCustomers.FindAsync(code);
//});
//app.MapPost("/createcustomer", async (StoreAppContext db, TblCustomer customer) =>
//{
//    await db.TblCustomers.AddAsync(customer);
//    await db.SaveChangesAsync();

//});

//app.MapPut("/updatecustomer", async (StoreAppContext db, TblCustomer customer, string code) =>
//{
//    var existdata = await db.TblCustomers.FindAsync(code);
//    if (existdata != null)
//    {
//        existdata.Name = customer.Name;
//        existdata.Email = customer.Email;
//        db.TblCustomers.Update(existdata);
//        await db.SaveChangesAsync();
//    }

//});

//app.MapDelete("/removecustomer", async (StoreAppContext db,  string code) =>
//{
//    var existdata = await db.TblCustomers.FindAsync(code);
//    if (existdata != null)
//    {

//        db.TblCustomers.Remove(existdata);
//        await db.SaveChangesAsync();
//    }

//});

//enable rate limiter
app.UseRateLimiter();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseStaticFiles();

// For enabling the CORS Policy we have two way , We can ebale globally in thiw middle ware option or we can enable it in the controller level.
// First enable middleware level 
//app.UseCors("corspolicy");
// For the default policy we can use 
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();
// lets enabling the basic authenication methods our midddleware side, so before the authorization lets include authentication 
app.UseAuthentication();// uise this in controller side (customer) , we haev to include a
app.UseAuthorization();
app.MapControllers();
QuestPDF.Settings.License = LicenseType.Community;  // ✅ Add this at the top


try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.Information("Shutting down the application");
    Log.CloseAndFlush();
}
