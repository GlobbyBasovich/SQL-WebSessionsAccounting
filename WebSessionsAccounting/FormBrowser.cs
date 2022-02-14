using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace WebSessionsAccounting
{
    public partial class FormBrowser : Form
    {
        public FormBrowser()
        {
            InitializeComponent();
            webBrowser.LifeSpanHandler = new LifeSpanHandler();

            Program.GlobalSqlCommand.CommandText =
                $"SELECT * FROM [Браузеры]";
            var reader = Program.GlobalSqlCommand.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                comboBoxBrowser.Items.Add($"{reader["Название"]} {reader["Версия"]}");
            }
            if (comboBoxBrowser.Items.Count > 0) comboBoxBrowser.SelectedIndex = 0;
            reader.Close();

            Program.GlobalSqlCommand.CommandText =
                $"SELECT * FROM [ОС]";
            reader = Program.GlobalSqlCommand.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                comboBoxOS.Items.Add($"{reader["Название"]}");
            }
            if (comboBoxOS.Items.Count > 0) comboBoxOS.SelectedIndex = 0;
            reader.Close();

            Program.GlobalSqlCommand.CommandText =
                $"SELECT TOP(10) * FROM (SELECT DISTINCT * FROM " +
                $"(SELECT [Протокол], [Доменное имя], [Страница] FROM [Посещения] " +
                $"WHERE [Учётная запись] = '{((FormLogin)Application.OpenForms[0]).textBoxLogin.Text}' " +
                $"ORDER BY [Время посещения] DESC OFFSET 0 ROWS) AS SQ1) AS SQ2";
            reader = Program.GlobalSqlCommand.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                comboBoxUrl.Items.Add($"{reader["Протокол"]}://{reader["Доменное имя"]}{reader["Страница"]}");
            }
            reader.Close();
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            webBrowser.Load(comboBoxUrl.Text);
        }

        private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((FormLogin)Application.OpenForms[0]).textBoxLogin.Text = "";
            ((FormLogin)Application.OpenForms[0]).textBoxPassword.Text = "";
            Application.OpenForms[0].Show();
        }

        private void textBoxUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                buttonGo_Click(sender, e);
        }

        private async void webBrowser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            string address = webBrowser.Address;
            comboBoxUrl.Invoke(new Action<string>(x => comboBoxUrl.Text = x), address);

            try
            {
                string browserInfo = (string)comboBoxBrowser.Invoke(new Func<string>(() => comboBoxBrowser.Text));
                var browserInfoSplit = browserInfo.Split(' ');
                string browserName = string.Join(" ", browserInfoSplit.Take(browserInfoSplit.Length - 1));
                string version = browserInfoSplit.Last();

                Program.GlobalSqlCommand.CommandText =
                    $"SELECT * FROM [Браузеры] " +
                    $"WHERE [Название] = '{browserName}' " +
                    $"AND [Версия] = '{version}'";
                var reader = Program.GlobalSqlCommand.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    Program.GlobalSqlCommand.CommandText =
                        $"INSERT INTO [Браузеры] VALUES(" +
                        $"'{browserName}', " +
                        $"'{version}', " +
                        $"NULL, NULL)";
                    //_dbg(Program.GlobalSqlCommand.CommandText);
                    Program.GlobalSqlCommand.ExecuteNonQuery();
                    comboBoxBrowser.Invoke(new Action<string>(x => comboBoxBrowser.Items.Add(x)), browserInfo);
                }
                else reader.Close();

                Program.GlobalSqlCommand.CommandText =
                     $"SELECT [ID браузера] FROM [Браузеры] " +
                     $"WHERE [Название] = '{browserName}' " +
                     $"AND [Версия] = '{version}'";
                reader = Program.GlobalSqlCommand.ExecuteReader();
                reader.Read();
                var browserId = reader["ID браузера"];
                reader.Close();

                string osName = (string)comboBoxBrowser.Invoke(new Func<string>(() => comboBoxOS.Text));

                Program.GlobalSqlCommand.CommandText =
                    $"SELECT * FROM [ОС] " +
                    $"WHERE [Название] = '{osName}' ";
                reader = Program.GlobalSqlCommand.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    Program.GlobalSqlCommand.CommandText =
                        $"INSERT INTO [ОС] VALUES(" +
                        $"'{osName}', " +
                        $"NULL, NULL, NULL)";
                    //_dbg(Program.GlobalSqlCommand.CommandText);
                    Program.GlobalSqlCommand.ExecuteNonQuery();
                    comboBoxBrowser.Invoke(new Action<string>(x => comboBoxBrowser.Items.Add(x)), browserInfo);
                }
                else reader.Close();

                var siteInfo = new Uri(address);
                string protocol = siteInfo.Scheme;
                string hostname = siteInfo.Host;
                string page = siteInfo.PathAndQuery;
                bool isUp = true;
                string ip = null, hosting = null;

                var httpClient = new System.Net.Http.HttpClient();
                try
                {
                    if (!(await httpClient.GetAsync(address)).IsSuccessStatusCode)
                    {
                        isUp = false;
                        throw new Exception("The website is down");
                    }
                    string response = await httpClient.GetStringAsync(
                        $"{"http://ip-api.com/json/"}{hostname}");
                    var jsonDoc = System.Text.Json.JsonDocument.Parse(response).RootElement;
                    ip = jsonDoc.GetProperty("query").GetString();
                    hosting = jsonDoc.GetProperty("isp").GetString();
                }
                catch
                {
                    ip = null;
                }
                httpClient.Dispose();

                if (ip != null)
                {
                    Program.GlobalSqlCommand.CommandText =
                    $"SELECT * FROM [IP-адреса] " +
                    $"WHERE [IP-адрес] = '{ip}'";
                    reader = Program.GlobalSqlCommand.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        Program.GlobalSqlCommand.CommandText =
                            $"INSERT INTO [IP-адреса] VALUES(" +
                            $"'{ip}', " +
                            $"'{hosting}')";
                        //_dbg(Program.GlobalSqlCommand.CommandText);
                        Program.GlobalSqlCommand.ExecuteNonQuery();
                        comboBoxBrowser.Invoke(new Action<string>(x => comboBoxBrowser.Items.Add(x)), browserInfo);
                    }
                    else reader.Close();
                }

                Program.GlobalSqlCommand.CommandText =
                    $"SELECT * FROM [Сайты] " +
                    $"WHERE [Доменное имя] = '{hostname}'";
                reader = Program.GlobalSqlCommand.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    string ipValue = ip != null ? $"'{ip}'" : "NULL";
                    Program.GlobalSqlCommand.CommandText =
                        $"INSERT INTO [Сайты] VALUES(" +
                        $"'{hostname}', " +
                        $"{ipValue}, " +
                        $"'Разрешён', " +
                        $"'{(isUp ? "OK" : "Недоступен")}')";
                    //_dbg(Program.GlobalSqlCommand.CommandText);
                    Program.GlobalSqlCommand.ExecuteNonQuery();
                    comboBoxBrowser.Invoke(new Action<string>(x => comboBoxBrowser.Items.Add(x)), browserInfo);
                }
                else reader.Close();

                Program.GlobalSqlCommand.CommandText =
                    $"INSERT INTO [Посещения] VALUES(" +
                    $"'{protocol}', " +
                    $"'{hostname}', " +
                    $"'{page}', " +
                    $"'{DateTime.Now}', " +
                    $"'{((FormLogin)Application.OpenForms[0]).textBoxLogin.Text}', " +
                    $"'{browserId}', " +
                    $"'{osName}')";
                //_dbg(Program.GlobalSqlCommand.CommandText);
                Program.GlobalSqlCommand.ExecuteNonQuery();
                comboBoxBrowser.Invoke(new Action<string>(x => comboBoxBrowser.Items.Add(x)), browserInfo);
            }
            catch (Exception ex)
            {
                var answer = MessageBox.Show(ex.Message, "Произошла ошибка",
                    MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                switch (answer)
                {
                    case DialogResult.Abort:
                        Invoke(new Action(() => Close()));
                        break;
                    case DialogResult.Retry:
                        webBrowser_AddressChanged(sender, e);
                        break;
                }
            }
        }

        void _dbg(string msg)
        {
            MessageBox.Show(msg);
            Invoke(new Action(() => Clipboard.SetText(msg)));
        }

        private void FormBrowser_Resize(object sender, EventArgs e)
        {
            comboBoxOS.Width = comboBoxBrowser.Width = Width / 2 - 80;
            comboBoxBrowser.Location = new Point(Width - comboBoxBrowser.Width - 28, comboBoxBrowser.Location.Y);
            labelBrowser.Location = new Point(comboBoxBrowser.Location.X - 55, labelBrowser.Location.Y);
        }
    }

    class LifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser) => false;

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser) { }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser) { }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition,
            bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo,
            IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            chromiumWebBrowser.Load(targetUrl);
            newBrowser = null;
            return true;
        }
    }
}
