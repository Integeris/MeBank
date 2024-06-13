using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка при добавлении клиента. Запрос: {command.CommandText}.", ex);
                    }
                    finally
                    {
                        transaction.Commit();
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
        public bool ExistUser(string login, string password = null)
        {
            bool clientExist;

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT \"ExistClient\"(@login");

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("@login", DbType.StringFixedLength, 50)
                {
                    Value = login
                }
            };

            if (password != null)
            {
                sqlBuilder.Append(", @password");
                parameters.Add(new NpgsqlParameter("@password", DbType.StringFixedLength, 50)
                {
                    Value = password
                });
            }

            sqlBuilder.Append(");");

            using (IDbContextTransaction transaction = Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sqlBuilder.ToString(), connection))
                {
                    try
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        clientExist = (bool)command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка при проверке существования клиента. Запрос: {command.CommandText}.", ex);
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }

            return clientExist;
        }

        /// <summary>
        /// Получение всех банковских счетов клиента.
        /// </summary>
        /// <param name="login">Идентификатор клиента.</param>
        /// <returns>Банковские счета клиента.</returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<BankAccount> GetBankAccounts(string login)
        {
            List<BankAccount> accounts = new List<BankAccount>();

            const string sqlText = "SELECT * FROM \"GetUserBankAccounts\" (@login)";

            NpgsqlParameter[] parameters =
            {
                new NpgsqlParameter("@login", DbType.StringFixedLength, 50)
                {
                    Value = login
                }
            };

            using (IDbContextTransaction transaction = Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sqlText, connection))
                {
                    try
                    {
                        command.Parameters.AddRange(parameters);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            PropertyInfo[] properties = typeof(BankAccount).GetProperties();

                            while (reader.Read())
                            {
                                BankAccount bankAccount = new BankAccount();

                                foreach (PropertyInfo propertyInfo in properties)
                                {
                                    propertyInfo.SetValue(bankAccount, Convert.ChangeType(reader.GetValue(propertyInfo.Name), propertyInfo.PropertyType));
                                }

                                accounts.Add(bankAccount);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка при получении банковских аккаунтов клиента. Запрос: {command.CommandText}.", ex);
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }

            return accounts;
        }

        /// <summary>
        /// Получение транзакий банковского счёта.
        /// </summary>
        /// <param name="idBankAccount">Идентификатор банковского счёта.</param>
        /// <returns>Транзакции банковского счёта.</returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<Entry> GetGetBankAccountEntries(int idBankAccount)
        {
            List<Entry> entries = new List<Entry>();

            const string sqlText = "SELECT * FROM \"GetBankAccountEntries\" (@idBankAccount)";

            NpgsqlParameter[] parameters =
            {
                new NpgsqlParameter("@idBankAccount", DbType.Int32)
                {
                    Value = idBankAccount
                }
            };

            using (IDbContextTransaction transaction = Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sqlText, connection))
                {
                    try
                    {
                        command.Parameters.AddRange(parameters);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            PropertyInfo[] properties = typeof(BankAccount).GetProperties();

                            while (reader.Read())
                            {
                                Entry entry = new Entry();

                                foreach (PropertyInfo propertyInfo in properties)
                                {
                                    propertyInfo.SetValue(entry, Convert.ChangeType(reader.GetValue(propertyInfo.Name), propertyInfo.PropertyType));
                                }

                                entries.Add(entry);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка при получении транзакций банковского счёта клиента. Запрос: {command.CommandText}.", ex);
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }

            return entries;
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
