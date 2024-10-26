using BACKBONE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface IJwtTokenService 
    {
        string GenerateToken(User user);
        ClaimsPrincipal ValidateToken(string token);
    }

}
