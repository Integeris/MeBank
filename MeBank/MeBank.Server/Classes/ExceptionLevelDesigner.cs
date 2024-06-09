using System;
using System.Text;

namespace MeBank.Server.Classes
{
    /// <summary>
    /// Преобразует ошибку в читаймую строку.
    /// </summary>
    public static class ExceptionLevelDesigner
    {
        /// <summary>
        /// Возвращает полный стек ошибок в читаймом виде.
        /// </summary>
        /// <param name="exception">Ошибка.</param>
        /// <returns>Стек ошибок в читаймом виде.</returns>
        public static string GetFullException(Exception exception)
        {
            int level = 1;
            StringBuilder builder = new StringBuilder();

            do
            {
                builder.AppendLine($"{level}) {exception.Message}.");
                exception = exception.InnerException;
                level++;
            } while (exception != null);

            return builder.ToString();
        }
    }
}
