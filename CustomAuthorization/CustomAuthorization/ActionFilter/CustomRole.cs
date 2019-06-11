using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomAuthorization.ActionFilter
{
    public static class CustomClaimType
    {
        public const string Role = "CustomRole";

    }

    public class CustomRoleRequirement: IAuthorizationRequirement
    {
        public List <string> Role { get; set; }

        public CustomRoleRequirement(List<string> Role)
        {
            this.Role = Role;

        }
    }

    public class CustomRoleHandler : AuthorizationHandler<CustomRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRoleRequirement requirement)
        {
            //throw new NotImplementedException();
           var username =  context.User.FindFirst(claim => claim.Type == CustomClaimType.Role).Value;
            // call dbContext add pass the parameter username and get role
           string role = "";
           if (username == "jon")
            {
                //get role from DB by using username
                role = "admin";
            }

           else
            {
                role = "other";
            }

           if (role == requirement.Role.ElementAt(0).ToLower())
            {
                context.Succeed(requirement);
                return Task.FromResult(0);
            }

            context.Fail();
            return Task.FromResult(0);
        }
    }





}
