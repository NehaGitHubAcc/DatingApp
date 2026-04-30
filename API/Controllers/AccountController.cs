using API.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.Extensions;


namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService): BaseApiController()
{
    [HttpPost("register")] // localhost:5001/api/account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await EmailExists(registerDTO.Email)) return BadRequest("Email is already taken");
        
        using var hmac = new HMACSHA512(); //this is randomly generated key and salt

        var user = new AppUser
        {
            Email = registerDTO.Email,
            DisplayName = registerDTO.DisplayName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.ToDto(tokenService); // 200 + user
    }

    [HttpPost("login")] // localhost:5001/api/account/login
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == loginDTO.Email.ToLower());
        if (user == null) return Unauthorized("Invalid email"); // 401

        using var hmac = new HMACSHA512(user.PasswordSalt); // 
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password"); // 401
        }

        return user.ToDto(tokenService); // 200 + user
    }

    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
    }
}
