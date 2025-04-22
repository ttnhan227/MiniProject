using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Models;

namespace Server.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    public AccountRepository(DatabaseContext context, IConfiguration _configuration)
    {
        this._context = context;
        this._configuration = _configuration;
    }
    // account tang Total Amount vaf lay User
    public async Task<AccountModel?> CheckLogin (AccountLogin accountLogin)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync
                (a => a.Username.Equals(accountLogin.Username) 
                      && a.Password.Equals(accountLogin.Password));

        return account;
    }

    public string GenerateToken(AccountModel account)
    {
        // Tạo khóa bảo mật từ chuỗi ký tự được lưu trong cấu hình (appsettings.json)
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        // Tạo thông tin ký (credentials) sử dụng thuật toán HMAC-SHA256
        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        // Tạo danh sách các claim (thông tin người dùng) để đưa vào token
        var claims = new[]
        {
            new Claim("Username", account.Username.ToString()), // Thêm claim chứa tên người dùng
            new Claim(ClaimTypes.Role, account.Role)                    // Thêm claim chứa vai trò người dùng
        };

        // Tạo token JWT với các thông tin như issuer, audience, claims, thời gian hết hạn và thông tin ký
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],          // Định danh của server phát hành token
            audience: _configuration["Jwt:Audience"],      // Định danh của client nhận token
            claims: claims,                                // Danh sách các claim
            expires: DateTime.Now.AddMinutes(30),          // Thời gian hết hạn của token (30 phút)
            signingCredentials: credentials);             // Thông tin ký token

        // Trả về chuỗi token đã được mã hóa
        return  new JwtSecurityTokenHandler().WriteToken(token);
    }
}