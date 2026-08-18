using System.Collections.Generic;
using System.Security.Claims;

namespace GameLogBack.Tests.Helpers;

public static class ClaimsPrincipalTestHelper
{
   public static ClaimsPrincipal CreatePrincipal(string userId, string name, string role)
   {
      var claims = new List<Claim>()
      {
         new Claim(ClaimTypes.NameIdentifier, userId),
         new Claim(ClaimTypes.Name, name),
         new Claim(ClaimTypes.Role, role)
      };
      var claimsIdentity = new ClaimsIdentity(claims, "Test");
      return new ClaimsPrincipal(claimsIdentity);
   }
}