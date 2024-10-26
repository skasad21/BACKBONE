using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using BACKBONE.DB;
using static BACKBONE.Core.ApplicationConnectionString.ApplicationConnectionString;


namespace BACKBONE.Infrastructure.SecurityRepo
{
    public class UserRepository : IUserRepository
    {
        public void CreateUser(User user)
        {
            var connectionString = GetConnectionString(1);
            IDBHelper _db = new MssqlDbHelper(connectionString);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            var sql = "INSERT INTO Users (EmpCode, PasswordHash, FullName) VALUES (@EmpCode, @PasswordHash, @FullName)";
            _db.Execute(sql, new
            {
                EmpCode = user.EmpCode,
                PasswordHash = user.PasswordHash,
                FullName = user.FullName
            });
        }


        public async Task<EQResponse<User>> GetUserByEmailAsync(string email)
        {
            var response = new EQResponse<User>();

            try
            {
                var connectionString = GetConnectionString(1); 
                IDBHelper _db = new MssqlDbHelper(connectionString); 
                var sql = "SELECT * FROM Users WHERE EmpCode = @EmpCode";
                var user = await _db.QuerySingleOrDefaultAsync<User>(sql, new { EmpCode = email });

                if (user != null)
                {
                    response.Success = true;
                    response.Message = "User retrieved successfully.";
                    response.Data = new EQResponseData<User> { SingleValue = user };
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Data = new EQResponseData<User> { SingleValue = null };
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error: " + ex.Message;
                response.Data = new EQResponseData<User> { SingleValue = null };
            }

            return response;
        }



        public async Task<EQResponse<User>> GetUserByUserIdAsync(int UserId)
        {
            var response = new EQResponse<User>();

            try
            {
                var connectionString = GetConnectionString(1); 
                IDBHelper _db = new MssqlDbHelper(connectionString);
                var sql = "SELECT * FROM Users WHERE UserId = @UserId";
                var user = await _db.QuerySingleOrDefaultAsync<User>(sql, new { UserId = UserId });

                if (user != null)
                {
                    response.Success = true;
                    response.Message = "User retrieved successfully.";
                    response.Data = new EQResponseData<User> { SingleValue = user };
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Data = new EQResponseData<User> { SingleValue = null };
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error: " + ex.Message;
                response.Data = new EQResponseData<User> { SingleValue = null };
            }

            return response;
        }

    }
}
