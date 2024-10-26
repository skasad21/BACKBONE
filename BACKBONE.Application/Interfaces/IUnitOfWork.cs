using BACKBONE.Application.Interfaces.SecurityInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ITokenRepository TokenRepositor { get; }
        IRefreshTokenService RefreshTokenService { get; }
        IJwtTokenService JwtTokenService { get; }
    }
}
