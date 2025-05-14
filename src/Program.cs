using Application.Dtos;
using Application.Services.ArticleService;
using Application.Services.ArticleService.Validators;
using Application.Services.Auth;
using Application.Services.Auth.Validators;
using Application.Shared;
using Domain.Entites;
using FluentValidation;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace NBlog
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


            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
            builder.Services.AddScoped<IValidator<LoginDto>, LoginValidator>();
            builder.Services.AddScoped<IValidator<ArticleFilterQuery>, ArticleQueryValidator>();
            builder.Services.AddScoped<IValidator<CreateArticleDto>, CreateArticleValidator>();
           

            builder.Services.AddScoped<ArticleService>();
            builder.Services.AddScoped<BaseRepo<Article,int>>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var auditInterceptor = new AuditableEntitiesInterceptor();
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.AddInterceptors(auditInterceptor);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                };
            });

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

            app.Run();
        }
    }
}
