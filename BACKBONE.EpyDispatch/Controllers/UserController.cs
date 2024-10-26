using BACKBONE.Application.Interfaces.SecurityInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BACKBONE.EpyDispatch.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Get specific user by ID  
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByUserIdAsync(id); 
            if (user.Data.SingleValue == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        // Get specific user by ID without authorization
        [HttpGet("withoutauth/{id}")]
        public async Task<IActionResult> GetUserByWithoutAuthId(int id)
        {
            var user = await _userRepository.GetUserByUserIdAsync(id);
            if (user.Data.SingleValue == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
