using MeBank.Server.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace MeBank.Server.Controllers
{
    /// <summary>
    /// Контроллер для действий клиента.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ClientController : ControllerBase
    {
        /// <summary>
        /// Создание контроллера для действий клиента.
        /// </summary>
        public ClientController() { }

        /// <summary>
        /// Регистрация новоги клиета.
        /// </summary>
        /// <param name="login">Логин нового клиента.</param>
        /// <param name="password">Пароль нового клиента.</param>
        /// <returns>Результат регистрации.</returns>
        [HttpPost]
        [Route($"{nameof(Registration)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Registration(string login, string password)
        {
            try
            {
                Core.Context.RegistrationClient(login, password);
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }

            return Ok(true);
        }

        /// <summary>
        /// Авторизация клиента.
        /// </summary>
        /// <param name="login">Логин.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Токен.</returns>
        [HttpGet]
        [Route($"{nameof(Login)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Login(string login, string password)
        {
            try
            {
                if (!Core.Context.ExistUser(login, password))
                {
                    throw new Exception("Клиента не существует. Пройдите авторизацию.");
                }

                return Ok(JWTManager.GetTokenString(JWTManager.GetSecurityToken(login)));
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }
    }
}
