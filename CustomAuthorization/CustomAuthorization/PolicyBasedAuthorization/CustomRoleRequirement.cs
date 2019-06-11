using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomActionFilter.PolicyBasedAuthorization
{
    public class CustomRoleRequirement : AuthorizationHandler<CustomRoleRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRoleRequirement requirement)
        {

            var roles = new[] { "Admin User", "Basic User", "Settlement User" };  //Get From DB.
            var userIsInRole = roles.Any(role => context.User.IsInRole(role));
            var user = roles.Any(rol => context.User.IsInRole(rol));
            
            if (!userIsInRole)
            {
                context.Fail();
            }

            return HandleRequirementAsync(context, requirement);

            //throw new NotImplementedException();
        }
    }

}

