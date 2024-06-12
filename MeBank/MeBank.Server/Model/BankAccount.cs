namespace MeBank.Server.Model
{
    /// <summary>
    /// Банковский счёт.
    /// </summary>
    public class BankAccount
    {
        /// <summary>
        /// Идентификатор банковского счёта.
        /// </summary>
        private int idBankaccount;

        /// <summary>
        /// Идентификатор клиента владельца.
        /// </summary>
        private int idClient;

        /// <summary>
        /// Баланс.
        /// </summary>
        private decimal balance;

        /// <summary>
        /// Наименование валюты.
        /// </summary>
        private string currencyTitle;

        /// <summary>
        /// Идентификатор банковского счёта.
        /// </summary>
        public int IdBankAccount
        {
            get => idBankaccount;
            set => idBankaccount = value;
        }

        /// <summary>
        /// Идентификатор клиента владельца.
        /// </summary>
        public int IdClient
        {
            get => idClient;
            set => idClient = value;
        }

        /// <summary>
        /// Баланс.
        /// </summary>
        public decimal Balance
        {
            get => balance;
            set => balance = value;
        }

        /// <summary>
        /// Наименование валюты.
        /// </summary>
        public string CurrencyTitle
        {
            get => currencyTitle;
            set => currencyTitle = value;
        }
    }
}
