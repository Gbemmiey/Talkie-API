using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Talkie.Controllers;
using Talkie.Data;
using Talkie.DTOs.Account;
using Talkie.Models;
using Talkie.Services.Auth;

namespace Talkie.Services.AuthenticationService
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthRepository(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> Login(string EmailAddress, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Accounts
                .FirstOrDefaultAsync(u => u.EmailAddress.ToLower().Equals(EmailAddress.ToLower()));

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Invalid Password or Username";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            return response;
        }

        public async Task<ServiceResponse<string>> Register(AddAccountDto NewAccount)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();

            if (await UserExists(NewAccount.EmailAddress))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }

            CreatePasswordHash(NewAccount.Password, out byte[] passwordHash, out byte[] passwordSalt);

            CreatePinHash(NewAccount.AuthPin, out byte[] pinHash, out byte[] pinSalt);

            Account user = new Account
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                PinHash = pinHash,
                PinSalt = pinSalt
            };

            _mapper.Map(NewAccount, user);
            _context.Accounts.Add(user);
            await _context.SaveChangesAsync();
            response.Data = user.Number;
            return response;
        }

        private void CreatePinHash(int authPin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(BitConverter.GetBytes(authPin));
            }
        }

        public async Task<bool> UserExists(string EmailAddress)
        {
            if (await _context.Accounts.AnyAsync(u => u.EmailAddress.ToLower() == EmailAddress.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // Creating new instance of hmac class
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(Account user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Number.ToString()),
                new Claim(ClaimTypes.Name, user.EmailAddress)
            };

            // Must be at least 16 characters
            var tokenValue = _configuration.GetSection("AppSettings:Token").Value;
            // tokenValue = "Hey, this is my private secret key";

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.
                        Encoding.UTF8.
                        GetBytes(tokenValue));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); //Token
        }
    }
}