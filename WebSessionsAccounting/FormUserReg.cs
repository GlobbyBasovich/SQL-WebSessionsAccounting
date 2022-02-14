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
    public partial class FormUserReg : Form
    {
        public FormUserReg()
        {
            InitializeComponent();
        }

        private void FormUserReg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms.Count < 3) Application.OpenForms[0].Show();
        }

        private void buttonReg_Click(object sender, EventArgs e)
        {
            Program.GlobalSqlCommand.CommandText =
                $"SELECT * FROM [Интернет-сеансы].[dbo].[УчётныеЗаписи] " +
                $"WHERE [Логин] = '{textBoxLogin.Text}'";

            var reader = Program.GlobalSqlCommand.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                Program.GlobalSqlCommand.CommandText =
                    $"INSERT INTO [Интернет-сеансы].[dbo].[УчётныеЗаписи] VALUES(" +
                    $"'{textBoxLogin.Text}', " +
                    $"HASHBYTES('md5', '{textBoxPassword.Text}'), " +
                    $"'{textBoxName.Text}', " +
                    $"SYSDATETIME()," +
                    $"{Convert.ToByte(checkBoxAdmin.Checked)})";
                Program.GlobalSqlCommand.ExecuteNonQuery();

                ((FormLogin)Application.OpenForms[0]).textBoxLogin.Text = textBoxLogin.Text;
                ((FormLogin)Application.OpenForms[0]).textBoxPassword.Text = textBoxPassword.Text;
                Close();
            }
            else
            {
                reader.Close();
                MessageBox.Show("Данный логин уже занят", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
