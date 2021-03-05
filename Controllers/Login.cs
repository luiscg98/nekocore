using ApiTarea.Models;
using ApiTarea.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiTarea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly BookService _bookService;
        readonly ITokenService tokenService;
        readonly IEncriptarService encriptarService;

        public LoginController(BookService bookService, ITokenService tokenService, IEncriptarService encriptarService)
        {
            _bookService = bookService;
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.encriptarService = encriptarService ?? throw new ArgumentNullException(nameof(encriptarService));
        }

        [HttpPost]
        public ActionResult<User> Create(User book)
        {
          
            if (book.email == null)
            {
                return NotFound();
            }

            var user = _bookService.GetByEmail(book.email);

            if (user == null)
            {
                return Ok(new
                {
                    ok = false,
                    message = "Credenciales incorrectas",
                });
            }

            book.password = encriptarService.GetSHA256(book.password);

            if (user.password == book.password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.role)
                };
                var usuario = new User
                {
                    role = user.role,
                    Id = user.Id
                };
                var accessToken = tokenService.GenerateAccessToken(claims, usuario);
                var refreshToken = tokenService.GenerateRefreshToken();
                user.refreshToken = refreshToken;
                user.refreshTokenExpiryTime = DateTime.Now.AddDays(7);
                _bookService.Update(user.Id, user);
                return Ok(new
                {
                    ok=true,
                    accesToken = accessToken,
                    refreshToken = refreshToken,
                    id = user.Id
                });
            }

            return Ok(new
            {
                ok=false,
                message="Credenciales incorrectas",
            });
        }
        
    }
}