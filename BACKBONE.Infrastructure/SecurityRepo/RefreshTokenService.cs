using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Infrastructure.SecurityRepo
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public RefreshTokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }        

        public Task<string> GenerateRefreshTokenAsync()
        {
            return Task.Run(() =>
            {
                var randomBytes = new byte[64];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                    return Convert.ToBase64String(randomBytes);
                }
            });
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _tokenRepository.GetRefreshTokenAsync(refreshToken);

            if (storedToken != null && storedToken.ExpirationDate > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _tokenRepository.SaveRefreshTokenAsync(refreshToken);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            await _tokenRepository.RevokeRefreshTokenAsync(refreshToken);
        }

        public Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            return _tokenRepository.GetRefreshTokenAsync(refreshToken);
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            var storedToken = _tokenRepository.GetRefreshToken(refreshToken);
            return storedToken != null && storedToken.ExpirationDate > DateTime.Now;
        }

    }


}
