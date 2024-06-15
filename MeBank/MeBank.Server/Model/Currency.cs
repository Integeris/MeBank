namespace MeBank.Server.Model
{
    /// <summary>
    /// Валюта.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Идентификатор валюты.
        /// </summary>
        private int idCurrency;

        /// <summary>
        /// Название.
        /// </summary>
        private string title;

        /// <summary>
        /// Курс по отношению к рублю.
        /// </summary>
        private decimal exchangeRate;

        /// <summary>
        /// Идентификатор валюты.
        /// </summary>
        public int IdCurrency
        {
            get => idCurrency;
            set => idCurrency = value;
        }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title
        {
            get => title;
            set => title = value;
        }

        /// <summary>
        /// Курс по отношению к рублю.
        /// </summary>
        public decimal ExchangeRate
        {
            get => exchangeRate;
            set => exchangeRate = value;
        }
    }
}
