using System.Data.SQLite;

namespace Project2U
{
    public class DatabaseManager
    {
        public static SQLiteConnection GetConnection(string databasePath)
        {
            string connectionString = $"Data Source={databasePath};Version=3;";
            return new SQLiteConnection(connectionString);
        }
    }
}
