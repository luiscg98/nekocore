using ApiTarea.Models;
using ApiTarea.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ApiTarea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookService _bookService;
        public UserController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public ActionResult<List<User>> Get() =>
            _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        [Authorize]
        public ActionResult<User> Get(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public ActionResult<User> Create(User book)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(book.password));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            book.password=sb.ToString();
            book.role = "USER_ROLE";
            book.estado = true;
            book.fecha = DateTime.UtcNow.Date;
            _bookService.Create(book);

            return Ok(new
            {
                ok = true,
                message = "Tu cuenta se ha creado con exito"
            });
        }

        [HttpPost]
        [Route("admin")]
        public ActionResult<User> Create2(User book)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(book.password));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            book.password = sb.ToString();
            book.role = "ADMIN_ROLE";
            book.estado = true;
            book.fecha = DateTime.UtcNow.Date;
            _bookService.Create(book);

            return Ok(new
            {
                ok = true,
                message = "Tu cuenta se ha creado con exito"
            });
        }


        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, User bookIn)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Update(id, bookIn);

            return Ok(new
            {
                ok = true,
                message = "Tu cuenta se ha actualizado"
            });
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Remove(book.Id);

            return NoContent();
        }
    }
}