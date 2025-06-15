using System.Windows;
using CurConvApp.Views;

namespace CurConvApp.Services
{
    public static class HistoryWindowService
    {
        /// <summary>
        /// Відкриває вікно історії конвертацій для вказаного користувача.
        /// </summary>
        /// <param name="userId">ID користувача</param>
        /// <param name="owner">Вікно-власник (може бути null)</param>
        public static void ShowHistoryWindow(int userId, Window owner = null)
        {
            var historyWindow = new HistoryWindow(userId);
            if (owner != null)
                historyWindow.Owner = owner;
            historyWindow.ShowDialog();
        }
    }
}
