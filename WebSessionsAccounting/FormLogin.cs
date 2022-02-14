using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSessionsAccounting
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Program.GlobalSqlCommand.CommandText =
                $"SELECT [Права администратора] FROM [Интернет-сеансы].[dbo].[УчётныеЗаписи] " +
                $"WHERE [Логин] = '{textBoxLogin.Text}' AND [Хеш пароля] = HASHBYTES('md5', '{textBoxPassword.Text}')";

            var reader = Program.GlobalSqlCommand.ExecuteReader();
            if (reader.HasRows && reader.Read())
            {
                bool isAdmin = reader.GetBoolean(0);
                reader.Close();
                Hide();
                if (isAdmin) new FormMain().Show();
                else new FormBrowser().Show();
            }
            else
            {
                reader.Close();
                MessageBox.Show("Недействительная пара логин-пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.GlobalSqlCommand.Connection.Close();
        }

        private void buttonReg_Click(object sender, EventArgs e)
        {
            Hide();
            new FormUserReg().Show();
        }
    }
}
