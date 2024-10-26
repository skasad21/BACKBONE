using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;

namespace BACKBONE.Application.Interfaces.SecurityInterface
{
    public interface IUserRepository
    {
        Task<EQResponse<User>> GetUserByEmailAsync(string email);

        Task<EQResponse<User>> GetUserByUserIdAsync(int UserId);
        void CreateUser(User user);
    }

}
