using ApiTarea.Models;
using ApiTarea.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ApiTarea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly BookService _bookService;
        readonly ITokenService tokenService;

        public TokenController(BookService bookService, ITokenService tokenService)
        {
            _bookService = bookService;
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("refresh")]
        public ActionResult<User> Create(User book)
        {
            if (book.AccessToken == null)
            {
                return BadRequest("Invalid client request");
            }
            string accessToken = book.AccessToken;
            string refreshToken = book.refreshToken;
            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = _bookService.GetByName(username);
            if (user == null || user.refreshToken != refreshToken || user.refreshTokenExpiryTime <= DateTime.Now)
            {
                return Ok(new
                {
                    ok = false,
                    message="Cuenta expirada, vuelva a iniciar sesión"
                }) ;
            }
            var usuario = new User
            {
                role = user.role,
                Id = user.Id
            };
            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims, usuario);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            user.refreshToken = newRefreshToken;
            _bookService.Update(user.Id, user);
            return Ok(new
            {
                ok=true,
                accessToken = newAccessToken,
                refreshToken = newRefreshToken,
                id=user.Id
            });

        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var user = _bookService.GetByName(username);
            if (user == null) return BadRequest();
            user.refreshToken = null;
            _bookService.Update(user.Id, user);
            return Ok(new
            {
                ok = true
            });
        }
    }
}