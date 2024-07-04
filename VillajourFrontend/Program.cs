using Radzen;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using VillajourFrontend.Components;
using VillajourFrontend.Helper;

namespace VillajourFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddCascadingAuthenticationState();
            
            // Ajouter des services au conteneur.
            builder.Services.AddRazorPages();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();
            builder.Services.AddHttpClient("Villajour", (sp, client) =>
            {
                var httpClient = builder.Configuration.GetSection("HttpClient");
                client.BaseAddress = new Uri(httpClient["BaseUrl"]);
            })
                .AddHttpMessageHandler<JwtTokenHeaderHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Villajour"));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<JwtTokenHeaderHandler>();

            builder.Services.AddServerSideBlazor()
                .AddCircuitOptions(options => { options.DetailedErrors = true; });

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                builder.Configuration.GetSection("OpenIDConnectSettings").Bind(options);

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.SignOutScheme = CookieAuthenticationDefaults.LogoutPath;
                options.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateLifetime = true,
                };

                if (builder.Environment.IsDevelopment())
                {
                    options.RequireHttpsMetadata = false;
                }

                options.Events = new OpenIdConnectEvents
                {
                    OnAccessDenied = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                        return Task.CompletedTask;
                    },
                };

                options.Events.OnSignedOutCallbackRedirect += context =>
                {
                    context.Response.Redirect(context.Options.SignedOutRedirectUri);
                    context.HandleResponse();

                    return Task.CompletedTask;
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configurer le pipeline de requêtes HTTP.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
