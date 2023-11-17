using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mongo.Services.CouponAPI.Extension
{
    public static class WebApplicationBuilderExtension
    {
        public static WebApplicationBuilder AddAuthenticationExtension(this WebApplicationBuilder builder)
        {
            var secret = builder.Configuration.GetValue<string>("ApiSetting:Secret");
            var audience = builder.Configuration.GetValue<string>("ApiSetting:Audience");
            var issuer = builder.Configuration.GetValue<string>("ApiSetting:Issuer");

            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidAudience = audience,
                    ValidIssuer = issuer
                };
            });

            return builder;
        }
    }
}
