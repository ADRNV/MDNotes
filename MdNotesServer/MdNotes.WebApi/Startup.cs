using MdNotesServer.Infrastructure;
using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.Jwt.JwtConfiguration;
using MdNotesServer.Infrastructure.MappingConfigurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace MdNotes.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;   
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<NotesContext>(options =>
            {
                options.UseSqlite($@"Data Source={Environment.CurrentDirectory}\notes.db;");
            });

            services.AddDbContext<UsersContext>(options => options.UseSqlite($@"Data Source={Environment.CurrentDirectory}\notes.db;"))
                .AddDefaultIdentity<User>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<UsersContext>();

            services.AddSingleton<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

            var jwtTokenOptions = _configuration.GetSection("jwtTokenOptions")
               .Get<JwtTokenOptions>()!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = true;
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidIssuer = jwtTokenOptions.Issuer,
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenOptions.Secret)),
                      ValidAudience = jwtTokenOptions.Audience,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.FromMinutes(60)
                  };
            });

            services.AddAutoMapper(c =>
            {
                c.AddProfile<UserEntityMappingConfiguration>();
            }, Assembly.GetExecutingAssembly());

            services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblies(typeof(Startup).Assembly);
            });

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                {
                    Title = "MD NOtes API",
                    Version = "V1",
                    Description = "API for MD Notes"
                });

                var xmlFile = Assembly.GetExecutingAssembly()
                .GetName()
                .Name + ".xml";

                var path = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(path);
            });

            services.AddAuthorization(c =>
            {
                c.AddPolicy("Administrator", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "Administrator");
                });

                c.AddPolicy("User", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "User");
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            using (var scope =
                       app.ApplicationServices.CreateScope())
            using (var context = scope.ServiceProvider.GetService<UsersContext>())
                context.Database.EnsureCreated();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseEndpoints(c => c.MapControllers());
        }
    }
}
