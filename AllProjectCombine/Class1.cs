using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Security;

namespace WebApiOauth
{
    // override this bcoz to get the functionalities of OAuth Authorization server in our api.
    public class Class1 : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();  // means we have validated the client
        }

        // in this, we will validate the credintial of user
        // if we found valid user, then we will generate token for that user.
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            if (Membership.ValidateUser(context.UserName, context.Password))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                context.Validated(identity);
            }            
            else
            {
                context.SetError("invalid grant", "provided username and password is incorrect");
            }
        }
    }
}