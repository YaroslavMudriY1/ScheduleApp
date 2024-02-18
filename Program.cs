using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Project2
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
/*        static void SQL(string[] args) { 
                using (SQLiteConnection conn = new SQLiteConnection("data source = db2.db"))

                {

                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        string strSql = "INSERT INTO[Customer] ([Id], [CustName]) VALUES(1, ‘Ming’)";
                        cmd.CommandText = strSql;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        // do something…
                        conn.Close();

                    }

                }

        }*/
    }
}
