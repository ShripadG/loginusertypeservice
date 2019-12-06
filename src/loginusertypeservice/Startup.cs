using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using loginusertypeservice.Models;
using loginusertypeservice.Services;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using employeeservice.Common;
using System.Reflection;
using System.IO;

public class Startup
{
    public IConfigurationRoot Configuration { get; set; }

    readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public Startup(IHostingEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddJsonFile("vcap-local.json", optional: true) // when running locally, store VCAP_SERVICES credentials in vcap-local.json
            .AddEnvironmentVariables();

        Configuration = builder.Build();

        string vcapServices = Environment.GetEnvironmentVariable("VCAP_SERVICES");
        if (vcapServices != null)
        {
            dynamic json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(vcapServices);

            // CF 'cloudantNoSQLDB' service
            if (json.ContainsKey("cloudantNoSQLDB"))
            {
                try
                {
                    Configuration["cloudantNoSQLDB:0:credentials:username"] = json["cloudantNoSQLDB"][0].credentials.username;
                    Console.WriteLine("username ");
                    Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:username"]);
                    Configuration["cloudantNoSQLDB:0:credentials:password"] = json["cloudantNoSQLDB"][0].credentials.password;
                    Console.WriteLine("password ");
                    Console.WriteLine(json["cloudantNoSQLDB"][0].credentials.password);
                    Configuration["cloudantNoSQLDB:0:credentials:host"] = json["cloudantNoSQLDB"][0].credentials.host;
                    Console.WriteLine("host ");
                    Console.WriteLine(json["cloudantNoSQLDB"][0].credentials.host);
                    Configuration["cloudantNoSQLDB:0:credentials:url"] = json["cloudantNoSQLDB"][0].credentials.url;
                    Console.WriteLine("url ");
                    Console.WriteLine(json["cloudantNoSQLDB"][0].credentials.url);
                }
                catch (Exception)
                {
                    // Failed to read Cloudant uri, ignore this and continue without a database
                }
            }
            // user-provided service with 'cloudant' in the name
            else if (json.ContainsKey("user-provided"))
            {
                foreach (var service in json["user-provided"])
                {
                    if (((String)service.name).Contains("cloudant"))
                    {
                        try
                        {
                            Configuration["cloudantNoSQLDB:0:credentials:username"] = json["cloudantNoSQLDB"][0].credentials.username;
                            Configuration["cloudantNoSQLDB:0:credentials:password"] = json["cloudantNoSQLDB"][0].credentials.password;
                            Configuration["cloudantNoSQLDB:0:credentials:host"] = json["cloudantNoSQLDB"][0].credentials.host;
                            Configuration["cloudantNoSQLDB:0:credentials:url"] = json["cloudantNoSQLDB"][0].credentials.url;
                        }
                        catch (Exception)
                        {
                            // Failed to read Cloudant uri, ignore this and continue without a database
                        }
                    }
                }
            }

        }
        else if (Configuration.GetSection("services").Exists())
        {
            try
            {
                Configuration["cloudantNoSQLDB:0:credentials:username"] = Configuration["services:cloudantNoSQLDB:0:credentials:username"];
                Console.WriteLine("username ");
                Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:username"]);
                Configuration["cloudantNoSQLDB:0:credentials:password"] = Configuration["services:cloudantNoSQLDB:0:credentials:password"];
                Console.WriteLine("password ");
                Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:password"]);
                Configuration["cloudantNoSQLDB:0:credentials:host"] = Configuration["services:cloudantNoSQLDB:0:credentials:host"];
                Console.WriteLine("host ");
                Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:host"]);
                Configuration["cloudantNoSQLDB:0:credentials:url"] = Configuration["services:cloudantNoSQLDB:0:credentials:url"];
                Console.WriteLine("url ");
                Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:url"]);
            }
            catch (Exception)
            {
                // Failed to read Cloudant uri, ignore this and continue without a database
            }

        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add framework services.
        var allowedDomains = this.Configuration.GetSection("CorsSettings:AllowOrigins").Get<string[]>();
        var allowedMethods = this.Configuration.GetSection("CorsSettings:AllowMethods").Get<string[]>();
        var allowedHeaders = this.Configuration.GetSection("CorsSettings:AllowHeaders").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
            builder =>
            {
                builder.WithOrigins(allowedDomains)
                .WithMethods(allowedMethods)
                .WithHeaders(allowedHeaders)
                .AllowCredentials();
            });
        });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Info { Title = "Login User Type Service", Version = "v1" });


            //Locate the XML file being generated by ASP.NET...
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile.ToLower());

            //... and tell Swagger to use those XML comments.
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("ApiKeyScheme", new ApiKeyScheme() { In = "header", Description = "Please insert ID", Name = "AppID", Type = "apiKey" });
            c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "ApiKeyScheme", new string[0] } });
        });
        var creds = new Creds()
        {
            username = Configuration["cloudantNoSQLDB:0:credentials:username"],
            password = Configuration["cloudantNoSQLDB:0:credentials:password"],
            host = Configuration["cloudantNoSQLDB:0:credentials:host"]
        };

        if (creds.username != null && creds.password != null && creds.host != null)
        {
            services.AddAuthorization();
            services.AddSingleton(typeof(Creds), creds);
            services.AddTransient<ILoginUserTypeCloudantService, UserTypeCloudantService>();
            services.AddTransient<LoggingHandler>();
            services.AddHttpClient("cloudant", client =>
            {
                var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(creds.username + ":" + creds.password));

                client.BaseAddress = new Uri(Configuration["cloudantNoSQLDB:0:credentials:url"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
            })
            .AddHttpMessageHandler<LoggingHandler>();
        }

        services.AddMvc();

    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        var cloudantService = ((ILoginUserTypeCloudantService)app.ApplicationServices.GetService(typeof(ILoginUserTypeCloudantService)));

        loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        loggerFactory.AddDebug();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseSecurityHeadersMiddleware(new SecurityHeadersBuilder()
               .AddCustomHeader("X-Content-Type-Options", "nosniff")
               .AddCustomHeader("Strict-Transport-Security", "max-age=31536000")
               );

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
        // specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Web API V1");
            c.RoutePrefix = "swagger";
        });

        app.UseStaticFiles();
        app.UseCors(MyAllowSpecificOrigins);
        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
        });
    }

    class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine("{0}\t{1}", request.Method, request.RequestUri);
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine(response.StatusCode);
            return response;
        }
    }
}