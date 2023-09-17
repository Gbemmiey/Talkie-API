global using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Talkie.Data;
using Talkie.Services.AccountService;
using Talkie.Services.Auth;
using Talkie.Services.AuthenticationService;
using Talkie.Services.ContactService;
using Talkie.Services.GenericServices;
using Talkie.Services.MessageService;
using Talkie.Services.TextService;
using Talkie.Services.TransactionService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection"),
    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
}

);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IGenericService, GenericService>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IContactService, ContactService>();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITextService, TextService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme, e.g. \"bearer {token}\" ",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddSwaggerGen();

var tokenValue = builder.Configuration.GetSection("AppSettings:Token").Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(tokenValue)
            ),
            ValidateAudience = false,
            ValidateIssuer = false,
        };
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigin",
        option =>
        {
            option.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseSwaggerUI();
    app.UseSwagger();
    app.UseCors(option =>
    {
        option
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();