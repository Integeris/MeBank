using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeBank.Server.Classes
{
    /// <summary>
    /// Менеджер JWT-авторизации.
    /// </summary>
    public static class JWTManager
    {
        /// <summary>
        /// Проверять издателя.
        /// </summary>
        private static readonly bool validateIssuer = true;

        /// <summary>
        /// Проверять пользователя.
        /// </summary>
        private static readonly bool validateAudience = true;

        /// <summary>
        /// Проверять время действия.
        /// </summary>
        private static readonly bool validateLifetime = true;

        /// <summary>
        /// Проверять издательский ключ.
        /// </summary>
        private static readonly bool validateIssuerSigningKey = true;

        /// <summary>
        /// Издатель.
        /// </summary>
        private static readonly string validIssuer = "MeBankServer";

        /// <summary>
        /// Пользователь.
        /// </summary>
        private static readonly string validAudience = "MeBankClient";

        /// <summary>
        /// Секретный ключ.
        /// </summary>
        private static readonly string secretKey = "MeBankSecretKey12345678901234567890";

        /// <summary>
        /// Алгоритм шифрования.
        /// </summary>
        private static readonly string securityAlgorithm = SecurityAlgorithms.HmacSha256;

        /// <summary>
        /// Количество минут до окончания действия токена.
        /// </summary>
        private static readonly int expiresMinuteCount = 1;

        /// <summary>
        /// Симетричный ключ.
        /// </summary>
        private static readonly SymmetricSecurityKey issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        /// <summary>
        /// Токен.
        /// </summary>
        private static readonly TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = validateIssuer,
            ValidateAudience = validateAudience,
            ValidateLifetime = validateLifetime,
            ValidateIssuerSigningKey = validateIssuerSigningKey,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        /// <summary>
        /// Преобразатор токена в веб-токен.
        /// </summary>
        private static readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        /// <summary>
        /// Проверять издателя.
        /// </summary>
        public static bool ValidateIssuer
        {
            get => validateIssuer;
        }

        /// <summary>
        /// Проверять пользователя.
        /// </summary>
        public static bool ValidateAudience
        {
            get => validateAudience;
        }

        /// <summary>
        /// Проверять время действия.
        /// </summary>
        public static bool ValidateLifetime
        {
            get => validateLifetime;
        }

        /// <summary>
        /// Проверять издательский ключ.
        /// </summary>
        public static bool ValidateIssuerSigningKey
        {
            get => validateIssuerSigningKey;
        }

        /// <summary>
        /// Издатель.
        /// </summary>
        public static string ValidIssuer
        {
            get => validIssuer;
        }

        /// <summary>
        /// Пользователь.
        /// </summary>
        public static string ValidAudience
        {
            get => validAudience;
        }

        /// <summary>
        /// Алгоритм шифрования.
        /// </summary>
        public static string SecurityAlgorithm
        {
            get => securityAlgorithm;
        }

        /// <summary>
        /// Количество минут до окончания действия токена.
        /// </summary>
        public static int ExpiresMinuteCount
        {
            get => expiresMinuteCount;
        }

        /// <summary>
        /// Симетричный ключ.
        /// </summary>
        public static SymmetricSecurityKey IssuerSigningKey
        {
            get => issuerSigningKey;
        }

        /// <summary>
        /// Токен.
        /// </summary>
        public static TokenValidationParameters TokenValidationParameters
        {
            get => tokenValidationParameters;
        }

        /// <summary>
        /// Получает токен.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <returns>Токен авторизации.</returns>
        public static JwtSecurityToken GetSecurityToken(string login)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, login)
            };

            return new JwtSecurityToken
            (
                validIssuer,
                validAudience, 
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinuteCount),
                signingCredentials: new SigningCredentials(issuerSigningKey, securityAlgorithm)
            );
        }

        /// <summary>
        /// Получить текстовое представление токена.
        /// </summary>
        /// <param name="token">Токен.</param>
        /// <returns>Текстовое представление токена.</returns>
        public static string GetTokenString(JwtSecurityToken token)
        {
            return tokenHandler.WriteToken(token);
        }
    }
}
