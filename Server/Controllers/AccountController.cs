using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Server.Repository;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
         private readonly IAccountRepository _accountRepository;
         
         public AccountController(IAccountRepository accountRepository)
         {
             _accountRepository = accountRepository;
         }

        [AllowAnonymous] // Cho phép truy cập không cần xác thực
        [HttpPost]
        public async Task<IActionResult> Login(AccountLogin userLogin)
        {
            // Kiểm tra thông tin đăng nhập của người dùng
            var user = await _accountRepository.CheckLogin(userLogin);
            if (user != null)
            {
                // Tạo token cho người dùng
                var token =  _accountRepository.GenerateToken(user);
                return Ok(new { token });
            }
            return NotFound("User not found");
        }
    }
}

