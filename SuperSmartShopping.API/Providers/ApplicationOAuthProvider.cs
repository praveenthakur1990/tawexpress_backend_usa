using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using SuperSmartShopping.API.Models;
using SuperSmartShopping.BAL.Services;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Enums;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            try
            {
                ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                string _roleName = roleManager.FindById(user.Roles.FirstOrDefault().RoleId).Name;
                if (_roleName == RolesEnum.User.ToString())
                {
                    if (!user.PhoneNumberConfirmed)
                    {
                        context.SetError("InActive Account", "The account is not activate yet.");
                        return;
                    }
                }
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                    CookieAuthenticationDefaults.AuthenticationType);
                AuthenticationProperties properties = CreateProperties(user.UserName, user.Id, roleManager.FindById(user.Roles.FirstOrDefault().RoleId).Name, user.FirstName.NullToEmptyString(), user.LastName.NullToEmptyString(), user.PhoneNumber.NullToEmptyString(), user.PhoneNumberConfirmed, user.Email.NullToEmptyString(), user.EmailConfirmed, user.CreatedBy.NullToEmptyString());
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);
            }
            catch (Exception ex)
            {
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }


        public static AuthenticationProperties CreateProperties(string userName, string userId, string roleName, string firstName, string lastName, string phoneNumber, bool IsPhoneConfirmed, string email, bool isEmailVerified, string createdBy)
        {
            //string tenantConnection = CommonManager.getTenantConnection(userId, string.Empty).FirstOrDefault().TenantConnection;
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "userId", userId },
                { "roleName", roleName },
                { "firstName", firstName },
                { "lastName", lastName },
                { "phoneNumber", phoneNumber },
                { "IsPhoneConfirmed", IsPhoneConfirmed.ToString() },
                { "email", email },
                { "isEmailVerified", isEmailVerified.ToString() },
                {"tenantConnection",string.Empty },
                { "createdBy",  createdBy}
               // { "IsOnline", roleName==RolesEnum.Rider.ToString() ? new RiderBusiness().IsRiderOnline(userName).ToString():""}

            };
            return new AuthenticationProperties(data);
        }
    }
}