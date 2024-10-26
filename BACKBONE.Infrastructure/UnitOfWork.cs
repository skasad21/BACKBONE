using BACKBONE.Application.Interfaces;
using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Infrastructure.SecurityRepo;

namespace BACKBONE.Infrastructure
{
    public partial class UnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepository { get; }    
        public ITokenRepository TokenRepositor { get; }    
        public IRefreshTokenService RefreshTokenService { get; }    
        public IJwtTokenService JwtTokenService { get; }    

        public UnitOfWork(IUserRepository userRepository, ITokenRepository iTokenRepository, IRefreshTokenService refreshTokenService, IJwtTokenService jwtTokenService)
        {
            UserRepository = userRepository;
            TokenRepositor = iTokenRepository;
            RefreshTokenService = refreshTokenService;
            JwtTokenService = jwtTokenService;
        }
    }
}
