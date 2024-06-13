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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return NotFound(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Проверка токена на правильность.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="token">Токен.</param>
        /// <returns>Правильны ли токен.</returns>
        [HttpGet]
        [Route($"{nameof(ValidateToken)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ValidateToken(string login, string token)
        {
            try
            {
                AssertValidateToken(login, token);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Получение всех банковских аккаунтов клиента.
        /// </summary>
        /// <param name="login">Логин клиента.</param>
        /// <param name="token">Токен.</param>
        /// <returns>Банковские аккаунты клиента.</returns>
        [HttpGet]
        [Route($"{nameof(GetBankAccounts)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetBankAccounts(string login, string token)
        {
            try
            {
                AssertValidateToken(login, token);
                return Ok(Core.Context.GetBankAccounts(login));
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Получение транзакий банковского счёта.
        /// </summary>
        /// <param name="idBankAccount">Идентификатор банковского счёта.</param>
        /// <param name="login">Логин.</param>
        /// <param name="token">Токен.</param>
        /// <returns>Транзакции банковского счёта.</returns>
        [HttpGet]
        [Route($"{nameof(GetGetBankAccountEntries)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetGetBankAccountEntries(int idBankAccount, string login, string token)
        {
            try
            {
                AssertValidateToken(login, token);
                return Ok(Core.Context.GetGetBankAccountEntries(idBankAccount));
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Проверка токена на правильность (Если не верный, то возврат ошибки).
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="token">Токен.</param>
        private static void AssertValidateToken(string login, string token)
        {
            JWTManager.AssertValidateToken(login, token);

            if (!Core.Context.ExistUser(login))
            {
                throw new ArgumentException($"Указанного пользователя ({login}) не сущесвует.", nameof(login));
            }
        }
    }
}
