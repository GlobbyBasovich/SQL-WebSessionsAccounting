using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WebSessionsAccounting
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Program.GlobalSqlCommand.CommandText =
                @"SELECT TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME <> 'sysdiagrams' AND TABLE_NAME <> 'УчётныеЗаписи'";

            var reader = Program.GlobalSqlCommand.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                var item = new ToolStripMenuItem(reader.GetString(0), null, OpenTable)
                {
                    Tag = new Action<FormTable>(x => x.LoadTable())
                };
                tablesToolStripMenuItem.DropDownItems.Add(item);
            }
            reader.Close();

            Program.GlobalSqlCommand.CommandText =
                @"SELECT TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE='VIEW' AND TABLE_NAME <> 'ОбщийЗапрос'";

            reader = Program.GlobalSqlCommand.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                var item = new ToolStripMenuItem(reader.GetString(0), null, OpenTable)
                {
                    Tag = new Action<FormTable>(x => x.LoadView())
                };
                viewsToolStripMenuItem.DropDownItems.Add(item);
            }
            reader.Close();

            viewsToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            {
                var item = new ToolStripMenuItem("Посещения по пользователю...", null, (lSender, lE) =>
                    {
                        var child = new FormViewOpenerVisitsByUser { MdiParent = this };
                        child.Show();
                    });
                viewsToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                foreach (var child in MdiChildren.OfType<FormTable>())
                {
                    child.MaximumSize = new Size(Size.Width - 50, Size.Height - 90);
                    child.UpdateTableSize();
                }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((FormLogin)Application.OpenForms[0]).textBoxLogin.Text = "";
            ((FormLogin)Application.OpenForms[0]).textBoxPassword.Text = "";
            Application.OpenForms[0].Show();
        }

        private void userRegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var child = new FormUserReg { MdiParent = this };
            child.checkBoxAdmin.Visible = true;
            child.Show();
        }

        private void OpenTable(object sender, EventArgs e)
        {
            var child = new FormTable { MdiParent = this };
            child.Text = ((ToolStripMenuItem)sender).Text;
            child.MaximumSize = new Size(Size.Width - 50, Size.Height - 90);
            ((Action<FormTable>)((ToolStripMenuItem)sender).Tag)(child);
            child.Show();
        }
    }
}
