using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoListApi.TokenAuthentication;

namespace ToDoListApi.Filters
{
    public class TokenAuthenticationFilter : Attribute , IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenManager = (ITokenManager)context.HttpContext.RequestServices.GetService(typeof(ITokenManager));

            var result = true;
            //沒有Headers值Authorization =>false
            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
                result = false;


            string token = string.Empty;
            if (result)
            {   //抓Headers值Authorization 為token
                token = context.HttpContext.Request.Headers.First(x => x.Key == "Authorization").Value;
                //
                if (!tokenManager.VerifyToken(token))
                    result = false;
            }
            if (!result)
            {
                context.ModelState.AddModelError("Unauthorized", "Check Your Authorization Value , Or Get New Authorization");
                context.Result = new UnauthorizedObjectResult(context.ModelState);
            }

        }
    }
}
