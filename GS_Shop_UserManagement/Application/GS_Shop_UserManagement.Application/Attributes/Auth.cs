using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GS_Shop_UserManagement.Application.Attributes;

public class Auth : AuthorizeAttribute
{
    public Auth(string policyName)
    {
        AuthenticationSchemes = "Bearer";
        Policy = policyName;
    }
    //Attribute,
    //IAuthenticationHandler
    //IAuthorizationFilter
//{
 //   public string Policy { get;  }
   // public Auth(string policyName)
//    {
  //      Policy = policyName;
  //  }

 //   public void OnAuthorization(AuthorizationFilterContext context)
  //  {

   //     var claim = "GetUser";
        
  //  }
}