﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace GTRevo.Platform.Security.Identity
{
    public class AppSignInManager : SignInManager<IIdentityUser, Guid>
    {
        public AppSignInManager(AppUserManager userManager, IAuthenticationManager authenticationManager)
               : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(IIdentityUser user)
        {
            return user.GenerateUserIdentityAsync((AppUserManager)UserManager, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}
