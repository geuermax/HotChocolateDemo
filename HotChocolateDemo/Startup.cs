using System;
using System.Collections.Generic;
using System.Data;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Server;
using HotChocolateDemo.graphql.types;
using HotChocolateDemo.hcWebsockets;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HotChocolateDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://loclahost:80",
                            "http://localhost")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
            #region IdentityServer4 Authentication
                // Damit Websockets ein Verbindung aufbauen können bei der erst in der initialen Nachricht das Token enthalten ist.
                .AddJwtBearer("Websockets", ctx => { })
                // IdentiyServerAuthentication deswegen, da bei diesem der TokenRetriver konfiguriert werden kann --> Also wo das Token entnommen werden kann.
                .AddIdentityServerAuthentication(options =>
            {
                // Hier die Konfigurationen aus Keycloak verwenden.
                options.Authority = "http://localhost:8080/auth/realms/master";
                // Api = Client
                options.ApiName = "test";
                options.RequireHttpsMetadata = false;

                // Ermitteln welche AuthenticationStrategy verwendet werden soll.
                options.ForwardDefaultSelector = context =>
                {
                    // Handelt es sich bei der Anfrage um einen WebsocketRequest
                    if (!context.Items.ContainsKey(AuthenticationSocketInterceptor.HTTP_CONTEXT_WEBSOCKET_AUTH_KEY) && context.Request.Headers.TryGetValue("Upgrade", out var value))
                    {
                        if (value.Count > 0 && value[0] is string stringValue && stringValue == "websocket")
                        {
                            return "Websockets";
                        }
                    }
                    return JwtBearerDefaults.AuthenticationScheme;
                };

                // Das Token entnehmen.
                options.TokenRetriever = new Func<HttpRequest, string>(req =>
                {
                    if (req.HttpContext.Items.TryGetValue(AuthenticationSocketInterceptor.HTTP_CONTEXT_WEBSOCKET_AUTH_KEY, out object token) && token is string stringToken)
                    {                        
                        return stringToken;
                    }
                    var fromHeader = TokenRetrieval.FromAuthorizationHeader();
                    var fromQuery = TokenRetrieval.FromQueryString();

                    var verdammtesToken = fromHeader(req) ?? fromQuery(req);
                    return verdammtesToken;
                });

                
                options.JwtBearerEvents = new JwtBearerEvents
                {
                    // Wenn die Anmeldung am Keycloak fehlschlägt soll ein internal Server Error zurückgegeben werden.
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();

                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";

                        return c.Response.WriteAsync("An error occured while processing your authentication.");
                    }                    
                };

            });
            #endregion


            #region JWT Bearer Authentication
            /**
             * JwtBearer kann nur dann verwendet werden, wenn keine Subscription verwendet werden. Dann kann die Konfiguration wie folgt verwendet werden.
             */
            //.AddJwtBearer(options =>
            //{
            //    options.Authority = "http://localhost:8080/auth/realms/master";
            //    // CleindID ... Im Keycloak muss ein Mapper hinterlegt werden damit die ClientID zum aud-Claim hinzugefügt wird
            //    options.Audience = "test";
            //    // Wenn über HTTP
            //    options.RequireHttpsMetadata = false;



            //    options.Events = new JwtBearerEvents()
            //    {
            //        OnAuthenticationFailed = c =>
            //        {
            //            c.NoResult();

            //            c.Response.StatusCode = 500;
            //            c.Response.ContentType = "text/plain";

            //            // Die Fehlermeldung sollte es sich in einer Developmentumgebung befinden
            //            //if (Environment.IsDevelopment())
            //            //{
            //            //    return c.Response.WriteAsync(c.Exception.ToString());
            //            //}

            //            return c.Response.WriteAsync("An error occured while processing your authentication.");
            //        },                    

            //    };
            //});
            #endregion

            // Hinzufügen von GraphQL
            services
                // InMemorySubscription für Subs innerhalb einer Anwendung (Mögliche Anbindung von Messagingsystemen)
                .AddInMemorySubscriptions()
                .AddGraphQL(sp => SchemaBuilder.New()
                .AddServices(sp)
                // Type registrieren --> Schema definieren
                .AddQueryType<QueryResolverType>()
                .AddMutationType<MutationResolverType>()
                .AddSubscriptionType<SubscriptionResolverType>()
                // Damit [Authorize verwendet werden kann]
                .AddAuthorizeDirectiveType()
                
                .Create()
                );

            services.AddGraphQLSubscriptions();


            // Interceptor einbinden der die erste Nachricht eines Websockets abfängt und dort das Token entnimmt und dem HTTP-Context hinzufügt
            services.AddSingleton<ISocketConnectionInterceptor<HttpContext>, AuthenticationSocketInterceptor>();
            


            // Damit bei Websocket-Request der Context immer gesetzt wird. <-- Ohne das scheiß Teil funktioniert die Authorisierung auch nicht -.-
            services.AddQueryRequestInterceptor((
                HttpContext context,
                IQueryRequestBuilder requestBuilder,
                CancellationToken _) =>
            {
                requestBuilder.TryAddProperty(nameof(HttpContext), context);
                requestBuilder.TryAddProperty(nameof(ClaimsPrincipal), context.GetUser());

                return Task.CompletedTask;
            });

            services.AddTransient<MyWebSocketManager>();                        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseCors();
            
            app.UseAuthentication();
            // GraphQL und Websockets zur Pipeline hinzufügen
            app.UseWebSockets();
            // Add middleware to track ws sessions and close those after a given time because of security reasons
            app.UseMiddleware<MySubscriptionMiddleware>(new SubscriptionMiddlewareOptions());            
            app.UseGraphQL("/graphql");            
        }
    }
}
