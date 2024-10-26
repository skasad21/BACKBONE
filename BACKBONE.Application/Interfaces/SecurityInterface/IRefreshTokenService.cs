using BACKBONE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync();
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);  
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);

        bool ValidateRefreshToken(string refreshToken);
    }


}
