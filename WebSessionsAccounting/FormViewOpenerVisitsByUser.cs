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
    public partial class FormViewOpenerVisitsByUser : Form
    {
        public FormViewOpenerVisitsByUser()
        {
            InitializeComponent();
        }

        private void FormViewOpenerVisitsByUser_Load(object sender, EventArgs e)
        {
            Program.GlobalSqlCommand.CommandText =
                @"SELECT [Логин] FROM [УчётныеЗаписи]
                WHERE [Права администратора] = 0";

            var reader = Program.GlobalSqlCommand.ExecuteReader();
            while (reader.HasRows && reader.Read())
                comboBoxUser.Items.Add(reader.GetString(0));
            reader.Close();

            if (comboBoxUser.Items.Count > 0) comboBoxUser.SelectedIndex = 0;
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            var child = new FormTable { MdiParent = MdiParent };
            child.Text = $"Посещения пользователя {comboBoxUser.SelectedItem}";
            child.MaximumSize = new Size(MdiParent.Size.Width - 50, MdiParent.Size.Height - 90);
            child.LoadView($"SELECT * FROM [Посещения] WHERE [Учётная запись] = '{comboBoxUser.SelectedItem}'");
            child.Show();
            Close();
        }
    }
}
