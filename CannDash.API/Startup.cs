using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Cors;
using System.Web.Configuration;
using Microsoft.Owin.Security.ActiveDirectory;
using System.IdentityModel.Tokens;
using CannDash;

[assembly: OwinStartup(typeof(CannDash.API.Startup))]

namespace CannDash.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var auth0Issuer = WebConfigurationManager.AppSettings["Auth0Issuer"];
            var auth0Audience = WebConfigurationManager.AppSettings["Auth0Audience"];

            app.UseCors(CorsOptions.AllowAll);
            var activeDirectoryFederationServicesBearerAuthenticationOptions = new ActiveDirectoryFederationServicesBearerAuthenticationOptions
            {
                MetadataEndpoint = $"{auth0Issuer.TrimEnd('/')}/wsfed/FederationMetadata/2007-06/FederationMetadata.xml",
                TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidAudience = auth0Audience,
                        ValidIssuer = auth0Issuer,
                        //IssuerSigningKeyResolver = (token, securityToken, identifier, parameters) => parameters.IssuerSigningTokens.FirstOrDefault()?.SecurityKeys?.FirstOrDefault()
                    }
            };

            app.UseActiveDirectoryFederationServicesBearerAuthentication(activeDirectoryFederationServicesBearerAuthenticationOptions);

            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.MapHttpAttributeRoutes();
            httpConfiguration.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            //WebApiConfig.Register(config);
            
            app.UseWebApi(httpConfiguration);
        }
    }
}
