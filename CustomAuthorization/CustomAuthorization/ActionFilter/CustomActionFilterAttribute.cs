using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CustomActionFilter.Context;
using CustomActionFilter.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CustomActionFilter.ActionFilter
{

    public class CustomActionFilterAttribute : ActionFilterAttribute
    {

        //public string Role { get; set; }
        private ApplicationDbContext dbcontext = new ApplicationDbContext();
        public static string role;
        public override void OnActionExecuting(ActionExecutingContext context)
        {


           // string role = "Admin";

            if (role == "admin")
            {
                Debug.WriteLine("Not Empty.................");
                base.OnActionExecuting(context);
                return;

            }

            else
            {
                context.HttpContext.Response.StatusCode = 401;
                return;
            }

            //base.OnActionExecuting(context);
            //CustomActionFilter("OnActionExecuting", context.RouteData);


        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {


            // DB access
            //StudentsController studentController = new StudentsController (dbcontext);
            //string role = studentController.Role;

           // string role = "Admin";

            if (role == "admin")
            {
                Debug.WriteLine("Not Empty.................");
                base.OnActionExecuted(context);
                return;

            }

            else
            {
                context.HttpContext.Response.StatusCode = 401;
                return;
            }



            //CustomActionFilter("OnActionExecuted", context.RouteData);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            //base.OnResultExecuting(context);

            //CustomActionFilter("OnResultExecuting ", context.RouteData);

           // string role = "manager";


            if (role == "admin")
            {
                Debug.WriteLine("Not Empty.................");
                base.OnResultExecuting(context);
                return;

            }

            else
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedResult();
                return;
            }

        }

        //public override void OnResultExecuted(ResultExecutedContext context)
        //{

        //    //base.OnResultExecuted(context);

        //    //CustomActionFilter("OnResultExecuted", context.RouteData);

        //    string role = "manager";

        //    if (role == "Admin")
        //    {
        //        Debug.WriteLine("Not Empty.................");
        //        base.OnResultExecuted(context);
        //        return;

        //    }
        //    else
        //    {
        //        context.HttpContext.Response.StatusCode = 401;
        //        //context.HttpContext.Response= new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Students" }, { "action", "UnAuth" } });
        //        return;
        //    }

        //}


        public void CustomActionFilter(string methodName, RouteData routeData)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var message = String.Format("{0}- controller:{1} action:{2}", methodName,
                                                                        controllerName,
                                                                        actionName);
            Debug.WriteLine(message);
        }



    }

       
}

