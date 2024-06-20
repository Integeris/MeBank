using MeBank.Server.Classes;
using MeBank.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
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
        /// Контекст базы данных.
        /// </summary>
        private readonly MeBankContext databaseContext;

        /// <summary>
        /// Создание контроллера для действий клиента.
        /// </summary>
        public ClientController(MeBankContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

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
                databaseContext.RegistrationClient(login, password);
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
                if (!databaseContext.ExistUser(login, password))
                {
                    throw new Exception("Клиента не существует. Пройдите авторизацию.");
                }

                return Ok($"Bearer {JWTManager.GetTokenString(JWTManager.GetSecurityToken(login))}");
            }
            catch (Exception ex)
            {
                return NotFound(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Проверка токена на правильность.
        /// </summary>
        /// <returns>Правильны ли токен.</returns>
        [HttpGet]
        [Authorize]
        [Route($"{nameof(ValidateToken)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ValidateToken()
        {
            return Ok();
        }

        /// <summary>
        /// Получение всех банковских аккаунтов клиента.
        /// </summary>
        /// <returns>Банковские аккаунты клиента.</returns>
        [HttpGet]
        [Authorize]
        [Route($"{nameof(GetBankAccounts)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetBankAccounts()
        {
            try
            {
                return Ok(databaseContext.GetBankAccounts(User.Identity.Name));
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
        /// <returns>Транзакции банковского счёта.</returns>
        [HttpGet]
        [Authorize]
        [Route($"{nameof(GetBankAccountEntries)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetBankAccountEntries(int idBankAccount)
        {
            try
            {
                return Ok(databaseContext.GetGetBankAccountEntries(idBankAccount));
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Получение валют банковских счетов.
        /// </summary>
        /// <returns>Валюты банковских счетов.</returns>
        [HttpGet]
        [Route($"{nameof(GetCurrencies)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCurrencies()
        {
            try
            {
                return Ok(databaseContext.GetCurrencies());
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Добавление банковского счёта клиенту.
        /// </summary>
        /// <param name="idCurrency">Идентификатр валюты.</param>
        /// <returns>Идентификатор нового банковского счёта.</returns>
        [HttpPost]
        [Authorize]
        [Route($"{nameof(AddBankAccount)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddBankAccount(int idCurrency)
        {
            try
            {
                return Ok(databaseContext.AddBankAccount(User.Identity.Name, idCurrency));
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Изменение валюты счёта клиента.
        /// </summary>
        /// <param name="idBankAccount">Идентификатор банковского счёта.</param>
        /// <param name="idCurrency">Идентификатор валюты.</param>
        /// <returns>Результат конвертации банковского счёта.</returns>
        [HttpPost]
        [Authorize]
        [Route($"{nameof(BankAccountConversion)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BankAccountConversion( int idBankAccount, int idCurrency)
        {
            try
            {
                databaseContext.BankAccountConversion(User.Identity.Name, idBankAccount, idCurrency);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }

        /// <summary>
        /// Перевод денежных средств.
        /// </summary>
        /// <param name="idCreditBankAcount">Счёт получателя.</param>
        /// <param name="idDebitBankAcount">Счёт отправителя.</param>
        /// <param name="amount">Сумма.</param>
        /// <returns>Удалось ли отправить деньги.</returns>
        [HttpPost]
        [Authorize]
        [Route($"{nameof(MoneyTransfer)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult MoneyTransfer( int idCreditBankAcount, int idDebitBankAcount, decimal amount)
        {
            try
            {
                databaseContext.MoneyTransfer(User.Identity.Name, idCreditBankAcount, idDebitBankAcount, amount);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ExceptionLevelDesigner.GetFullException(ex));
            }
        }
    }
}
