using MdNotes.WebApi.Middlewares;
using MdNotesServer.Core.Stores;
using MdNotesServer.Infrastructure;
using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.Jwt;
using MdNotesServer.Infrastructure.Jwt.JwtConfiguration;
using MdNotesServer.Infrastructure.MappingConfigurations;
using MdNotesServer.Infrastructure.Security;
using MdNotesServer.Infrastructure.Stores;
using MediatR;
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
            var connectionString = _configuration.GetConnectionString("UserNotes");

            services.AddDbContext<NotesContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddDbContext<UsersContext>(options => options.UseSqlServer(connectionString))
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

            services.AddScoped<INotesStore<NoteCore>, NotesStore>();

            var jwtTokenOptions = _configuration.GetSection("jwtTokenOptions")
               .Get<JwtTokenOptions>()!;

            services.AddSingleton(jwtTokenOptions);

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

            services.AddScoped<IJwtAuthManager<UserEntity>, JwtAuthManager>();

            services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblies(typeof(Startup).Assembly);
            });

            services.AddControllers();

            services.AddCors(c =>
            {
                c.AddPolicy("NotesClient", p =>
                {
                    p.AllowAnyOrigin();
                });
            });

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
                c.AddPolicy(AuthorizeConstants.Policies.Administrator, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, AuthorizeConstants.Roles.Administrator);
                });

                c.AddPolicy(AuthorizeConstants.Policies.User, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, AuthorizeConstants.Roles.User);
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseCors("NotesClient");

            using (var scope =
                       app.ApplicationServices.CreateScope())
            using (var context = scope.ServiceProvider.GetService<UsersContext>())
                context.Database.EnsureCreated();

            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseEndpoints(c => c.MapControllers());
        }
    }
}
