using MeBank.Server.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        /// Регулярное выражение для проверки логина и пароля.
        /// </summary>
        private readonly static string regexString = @"^[A-Za-z\d_\!\*]{5,50}$";

        /// <summary>
        /// Логгер.
        /// </summary>
        private readonly ILogger<ClientController> logger;

        /// <summary>
        /// Создание контроллера для действий клиента.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        public ClientController(ILogger<ClientController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Регистрация новоги клиета.
        /// </summary>
        /// <param name="login">Логин нового клиента.</param>
        /// <param name="password">Пароль нового клиента.</param>
        /// <returns>Токен пользователя.</returns>
        [HttpPost]
        [Route($"{nameof(RegistrationClient)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RegistrationClient(string login, string password)
        {
            logger.LogInformation("Вошёл в регистрацию");

            try
            {
                if (!Regex.IsMatch(login, regexString))
                {
                    throw new ArgumentException($"Логин должен подходить выражению {regexString}.", nameof(login));
                }
                else if (!Regex.IsMatch(password, regexString))
                {
                    throw new ArgumentException($"Пароль должен подходить выражению {regexString}.", nameof(password));
                }

                Core.Context.RegistrationClient(login, password);
                logger.LogTrace("Клиент {login} зарегистрирован.", login);
            }
            catch (Exception ex)
            {
                string fullMessage = ExceptionLevelDesigner.GetFullException(ex);
                logger.LogTrace("{fullMessage}", fullMessage);
                return BadRequest(fullMessage);
            }

            return Ok();
        }
    }
}
