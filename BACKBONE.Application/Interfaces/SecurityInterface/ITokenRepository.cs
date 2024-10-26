using BACKBONE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface ITokenRepository
    {
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        RefreshToken GetRefreshToken(string token);
        Task RevokeRefreshTokenAsync(string token);
    }

}
