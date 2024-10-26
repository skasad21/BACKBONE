using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Models;
using BACKBONE.DB;
using static BACKBONE.Core.ApplicationConnectionString.ApplicationConnectionString;

namespace BACKBONE.Infrastructure.SecurityRepo
{
    public class TokenRepository : ITokenRepository
    {
        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            var _connectionString = GetConnectionString(1);
            IDBHelper _db = new MssqlDbHelper(_connectionString);
            var sql = "INSERT INTO RefreshTokens (UserId, Token, ExpirationDate) VALUES (@UserId, @Token, @ExpirationDate)";
            await _db.ExecuteAsync(sql, new
            {
                UserId = refreshToken.UserId,
                Token = refreshToken.Token,
                ExpirationDate = refreshToken.ExpirationDate
            });
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            var _connectionString = GetConnectionString(1);
            IDBHelper _db = new MssqlDbHelper(_connectionString);
            var sql = "SELECT * FROM RefreshTokens WHERE Token = @Token";
            return await _db.QuerySingleOrDefaultAsync<RefreshToken>(sql, new { Token = token });
        }

        public RefreshToken GetRefreshToken(string token)
        {
            var _connectionString = GetConnectionString(1);
            IDBHelper _db = new MssqlDbHelper(_connectionString);
            var sql = "SELECT * FROM RefreshTokens WHERE Token = @Token";
            return _db.QuerySingleOrDefault<RefreshToken>(sql, new { Token = token });
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var _connectionString = GetConnectionString(1);
            IDBHelper _db = new MssqlDbHelper(_connectionString);
            var sql = "DELETE FROM RefreshTokens WHERE Token = @Token";
            await _db.ExecuteAsync(sql, new { Token = token });
        }
    }
}
