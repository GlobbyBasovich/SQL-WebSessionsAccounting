using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace WebSessionsAccounting
{
    static class Program
    {
        public static readonly SqlCommand GlobalSqlCommand = new SqlConnection(
            new Properties.Settings().WebSessionsConnectionString).CreateCommand();

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Cef.Initialize(new CefSettings());
            GlobalSqlCommand.Connection.Open();
            Application.Run(new FormLogin());
        }
    }
}
