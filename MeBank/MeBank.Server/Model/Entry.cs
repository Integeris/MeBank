using System;

namespace MeBank.Server.Model
{
    /// <summary>
    /// Транзакция.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Идентификатор транзакции.
        /// </summary>
        private int idEntry;

        /// <summary>
        /// Банковский счёт поступления или списания.
        /// </summary>
        private int idBankAccount;

        /// <summary>
        /// Сумма операции.
        /// </summary>
        private decimal amount;

        /// <summary>
        /// Дата транзакции.
        /// </summary>
        private DateTime date;

        /// <summary>
        /// Идентификатор транзакции.
        /// </summary>
        public int IdEntry
        {
            get => idEntry;
            set => idEntry = value;
        }

        /// <summary>
        /// Банковский счёт поступления или списания.
        /// </summary>
        public int IdBankAccount
        {
            get => idBankAccount;
            set => idBankAccount = value;
        }

        /// <summary>
        /// Сумма операции.
        /// </summary>
        public decimal Amount
        {
            get => amount;
            set => amount = value;
        }

        /// <summary>
        /// Дата транзакции.
        /// </summary>
        public DateTime Date
        {
            get => date;
            set => date = value;
        }
    }
}
