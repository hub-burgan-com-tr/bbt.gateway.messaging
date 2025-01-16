using bbt.gateway.common;
using bbt.gateway.messaging.ui.Base;
using bbt.gateway.messaging.ui.Base.Administration;
using bbt.gateway.messaging.ui.Base.Token;
using bbt.gateway.messaging.ui.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Radzen;
using Refit;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseVaultSecrets(typeof(Program));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<bbt.gateway.messaging.ui.Data.HttpContextAccessor>();

IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
   .AddCookie()

    .AddOpenIdConnect("Auth0", options =>
    {
        options.MetadataAddress = builder.Configuration["OpenId:MetadataAddress"];
        options.NonceCookie.SameSite = SameSiteMode.Unspecified;
        options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

        options.Authority = $"{builder.Configuration["Okta:OktaDomain"]}";
        // options.SaveTokens = true;
        // Configure the Auth0 Client ID and Client Secret
        options.ClientId = builder.Configuration["Okta:ClientId"];
        options.ClientSecret = builder.Configuration["Okta:ClientSecret"];
        options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        options.RequireHttpsMetadata = false;
        options.GetClaimsFromUserInfoEndpoint = false;
        // Use the authorization code flow.
        options.ResponseType = OpenIdConnectResponseType.Code;

        options.CallbackPath = new PathString("/authorization-code/callback");
        options.SignedOutCallbackPath = new PathString("/authorization-code/signout/callback");
        // Configure the Claims Issuer to be Auth0
        //options.ClaimsIssuer = "Auth0";
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false
        };
        options.SecurityTokenValidator = new JwtSecurityTokenHandler
        {
            // Disable the built-in JWT claims mapping feature.
            InboundClaimTypeMap = new Dictionary<string, string>()
        };
        options.ProtocolValidator = new OpenIdConnectProtocolValidator()
        {
            RequireNonce = false,
            RequireState = false
        };
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {

                var builder = new UriBuilder(context.ProtocolMessage.RedirectUri);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    builder.Scheme = "https";
                    builder.Port = -1;
                }

                context.ProtocolMessage.RedirectUri = builder.ToString();
                return Task.FromResult(0);
            },
            OnTokenValidated = context =>
            {
                try
                {
                    if (context is not null && context.Principal is not null && context.Principal.Identity is not null)
                    {
                        var identity = (ClaimsIdentity)context.Principal.Identity;
                        List<Claim> addToken = new();

                        if (context?.TokenEndpointResponse is not null && context?.TokenEndpointResponse?.AccessToken is not null)
                        {
                            addToken.Add(new Claim("access_token", context?.TokenEndpointResponse?.AccessToken));

                            var handler = new JwtSecurityTokenHandler();
                            var login_name = string.Empty;

                            // Token'ýn içeriðini çöz
                            if (handler.CanReadToken(context?.TokenEndpointResponse?.AccessToken))
                            {
                                var jwtToken = handler.ReadJwtToken(context?.TokenEndpointResponse?.AccessToken);

                                // Payload'daki claim'lere eriþim
                                foreach (var claim in jwtToken.Claims)
                                {
                                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                                }

                                // Örneðin, belirli bir claim'i alma
                                login_name = jwtToken.Claims.FirstOrDefault(c => c.Type == "login_name")?.Value;
                                Console.WriteLine($"Preferred Username: {login_name}");
                            }
                            else
                            {
                                Console.WriteLine("Token okunamýyor.");
                            }

                            if (login_name != null && !string.IsNullOrEmpty(login_name) && login_name.Length < 12)
                                addToken.Add(new Claim("sicil", login_name));
                        }

                        if (context?.TokenEndpointResponse is not null && context?.TokenEndpointResponse?.IdToken is not null)
                        {
                            addToken.Add(new Claim("id_token", context?.TokenEndpointResponse?.IdToken));
                        }

                        if (context?.TokenEndpointResponse is not null && context?.TokenEndpointResponse?.RefreshToken is not null)
                        {
                            addToken.Add(new Claim("refresh_token", context?.TokenEndpointResponse?.RefreshToken));
                        }

                        if (addToken.Count > 0)
                        {
                            identity.AddClaims(addToken);
                        }

                        // so that we don't issue a session cookie but one with a fixed expiration
                        context.Properties.IsPersistent = true;
                        context.Properties.AllowRefresh = true;

                        // align expiration of the cookie with expiration of the

                        var accessToken = new JwtSecurityToken(context.TokenEndpointResponse.AccessToken);
                    }
                }
                catch
                {

                }

                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                // If your authentication logic is based on users then add your logic here
                return Task.CompletedTask;
            },
            OnTicketReceived = context =>
            {
                // If your authentication logic is based on users then add your logic here
                return Task.CompletedTask;
            },

            //HK save for later
            OnSignedOutCallbackRedirect = context =>
            {
                context.Response.Redirect("~/");
                context.HandleResponse();

                return Task.CompletedTask;
            },
            OnUserInformationReceived = context =>
            {
                return Task.CompletedTask;
            },
        };
    });

builder.Services.Configure<OktaSettings>(builder.Configuration.GetSection("Okta"));
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRefitClient<IMessagingGatewayService>()
               .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["Api:MessagingGateway"]));
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<AdministrationService>();

builder.Services.AddScoped<ITokenService, OktaTokenService>();
FrameworkDependencyHelper.Instance.LoadServiceCollection(builder.Services);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();