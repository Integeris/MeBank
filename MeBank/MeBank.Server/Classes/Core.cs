using MeBank.Server.Model;

namespace MeBank.Server.Classes
{
    /// <summary>
    /// Шлюз доступа к управлению базы данных.
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Контекст базы данных.
        /// </summary>
        public static MeBankContext Context { get; } = new MeBankContext();
    }
}
