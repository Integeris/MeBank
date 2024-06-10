using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml;

namespace MeBank.Server.Model
{
    /// <summary>
    /// Контекст базы данных MeBank.
    /// </summary>
    public class MeBankContext : DbContext
    {
        /// <summary>
        /// Название или адрес сервера.
        /// </summary>
        private readonly string host;

        /// <summary>
        /// Порт сервера.
        /// </summary>
        private readonly short port;

        /// <summary>
        /// Название базы данных.
        /// </summary>
        private readonly string database;

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        private readonly string username;

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        private readonly string password;

        /// <summary>
        /// Подключение к базе данных.
        /// </summary>
        private readonly NpgsqlConnection connection;

        /// <summary>
        /// Создание контекста базы данных MeBank;
        /// </summary>
        public MeBankContext()
        {
            host = "localhost";
            port = 5432;
            database = "MeBank";
            username = "Server";
            password = "Server";

            this.connection = (NpgsqlConnection)Database.GetDbConnection();
        }

        /// <summary>
        /// Регистрация клиента.
        /// </summary>
        /// <param name="login">Логин.</param>
        /// <param name="password">Пароль.</param>
        /// <exception cref="Exception"></exception>
        public void RegistrationClient(string login, string password)
        {
            const string regexString = @"^[A-Za-z\d_\!\*]{5,50}$";

            if (login == null)
            {
                throw new ArgumentException($"Логин не может быть пустым.", nameof(login));
            }
            else if (password == null)
            {
                throw new ArgumentException($"Пароль не может быть пустым.", nameof(password));
            }
            else if (!Regex.IsMatch(login, regexString))
            {
                throw new ArgumentException($"Логин должен подходить выражению {regexString}.", nameof(login));
            }
            else if (!Regex.IsMatch(password, regexString))
            {
                throw new ArgumentException($"Пароль должен подходить выражению {regexString}.", nameof(password));
            }

            const string sqlText = "CALL \"CreateClient\" (@login, @password)";

            NpgsqlParameter[] parameters =
            {
                new NpgsqlParameter("@login", DbType.StringFixedLength, 50)
                {
                    Value = login
                },
                new NpgsqlParameter("@password", DbType.StringFixedLength, 50)
                {
                    Value = password
                }
            };

            using (IDbContextTransaction transaction = Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sqlText, connection))
                {
                    try
                    {
                        command.Parameters.AddRange(parameters);
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Commit();
                        throw new Exception($"Ошибка при добавлении клиента. Запрос: {command.CommandText}.", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Проверка пользователя на наличие.
        /// </summary>
        /// <param name="login">Логин.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Существует ли клиент.</returns>
        public bool ExistUser(string login, string password)
        {
            bool clientExist;
            const string sqlText = "SELECT \"ExistClient\"(@login, @password);";

            NpgsqlParameter[] parameters =
            {
                new NpgsqlParameter("@login", DbType.StringFixedLength, 50)
                {
                    Value = login
                },
                new NpgsqlParameter("@password", DbType.StringFixedLength, 50)
                {
                    Value = password
                }
            };

            using (IDbContextTransaction transaction = Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sqlText, connection))
                {
                    try
                    {
                        command.Parameters.AddRange(parameters);
                        clientExist = (bool)command.ExecuteScalar();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Commit();
                        throw new Exception($"Ошибка при добавлении клиента. Запрос: {command.CommandText}.", ex);
                    }
                }
            }

            return clientExist;
        }

        /// <summary>
        /// Конфигурация подключения к базе данных.
        /// </summary>
        /// <param name="optionsBuilder">Опции подключения.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = database,
                Username = username,
                Password = password
            };

            optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
