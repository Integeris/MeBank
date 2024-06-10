using MeBank.Server.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;

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
        [Route($"{nameof(Registration)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Registration(string login, string password)
        {
            logger.LogInformation("Вошёл в регистрацию.");

            try
            {
                
                Core.Context.RegistrationClient(login, password);
                logger.LogTrace("Клиент {login} зарегистрирован.", login);
            }
            catch (Exception ex)
            {
                string fullMessage = ExceptionLevelDesigner.GetFullException(ex);
                logger.LogTrace("{fullMessage}", fullMessage);
                return BadRequest(fullMessage);
            }

            logger.LogInformation("Выход из регистрации.");
            return Ok();
        }
    }
}
