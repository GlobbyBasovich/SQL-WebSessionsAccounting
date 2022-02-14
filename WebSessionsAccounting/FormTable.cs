using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace WebSessionsAccounting
{
    public partial class FormTable : Form
    {
        bool isActive;
        readonly DataSet dataSet = new DataSet();
        readonly DataTable dataTable = new DataTable();
        DataGridViewCell previousCell;

        public FormTable()
        {
            InitializeComponent();
            dataSet.Tables.Add(dataTable);
        }

        private void FormTable_Shown(object sender, EventArgs e)
        {
            isActive = true;
        }

        private void FormTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            isActive = false;
        }

        public void LoadTable(string sqlCommand = null)
        {
            Program.GlobalSqlCommand.CommandText = sqlCommand ??
                $"SELECT * FROM [{Text}]";
            var reader = Program.GlobalSqlCommand.ExecuteReader();
            dataSet.Load(reader, LoadOption.OverwriteChanges, dataTable);
            reader.Close();

            dataGridView.DataSource = dataTable;
            dataGridView.AutoSize = true;

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = $"{saveFileDialog.InitialDirectory}\\{$"{Text} {DateTime.Now:dd_MM_yyyy HH_mm_ss}"}";
        }

        public void LoadView(string sqlCommand = null)
        {
            dataGridView.AllowUserToAddRows = dataGridView.AllowUserToDeleteRows = !(dataGridView.ReadOnly = true);
            dataGridView.RowLeave -= dataGridView_RowLeave;
            LoadTable(sqlCommand);
        }

        public void UpdateTableSize()
        {
            dataGridView.AutoSize = false;
            dataGridView.AutoSize = true;
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void dataGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (isActive && e.RowIndex < dataGridView.Rows.Count - 1)
            {
                dataGridView.EndEdit();

                Program.GlobalSqlCommand.CommandText =
                    $"SELECT * FROM [{Text}] " +
                    $"WHERE [{dataGridView.Columns[0].HeaderText}] = '{dataGridView.Rows[e.RowIndex].Cells[0].Value}'";

                var reader = Program.GlobalSqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    Program.GlobalSqlCommand.CommandText =
                        $"UPDATE [{Text}] SET " +
                        string.Join(", ", dataGridView.Columns.Cast<DataGridViewColumn>().Skip(1).
                        Select(x => $"[{x.HeaderText}] = '{dataGridView.Rows[e.RowIndex].Cells[x.Index].Value}'")) +
                        $" WHERE [{dataGridView.Columns[0].HeaderText}] = '{dataGridView.Rows[e.RowIndex].Cells[0].Value}'";
                    //MessageBox.Show(Program.GlobalSqlCommand.CommandText);
                    //Clipboard.SetText(Program.GlobalSqlCommand.CommandText);
                }
                else
                {
                    reader.Close();
                    Program.GlobalSqlCommand.CommandText =
                        $"INSERT INTO [{Text}] VALUES(" +
                        string.Join(", ", dataGridView.Rows[e.RowIndex].Cells.Cast<DataGridViewCell>()
                        .Where(x => !x.ReadOnly).Select(x => $"'{x.Value}'")) +
                        $")";
                    //MessageBox.Show(Program.GlobalSqlCommand.CommandText);
                    //Clipboard.SetText(Program.GlobalSqlCommand.CommandText);
                }

                previousCell = null;
                try
                {
                    Program.GlobalSqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    previousCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Program.GlobalSqlCommand.CommandText =
                $"SELECT * FROM [{Text}] " +
                $"WHERE [{dataGridView.Columns[0].HeaderText}] = '{dataGridView.Rows[e.Row.Index].Cells[0].Value}'";

            var reader = Program.GlobalSqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                Program.GlobalSqlCommand.CommandText =
                    $"DELETE FROM [{Text}] " +
                    $"WHERE [{dataGridView.Columns[0].HeaderText}] = '{dataGridView.Rows[e.Row.Index].Cells[0].Value}'";
                //MessageBox.Show(Program.GlobalSqlCommand.CommandText);
                //Clipboard.SetText(Program.GlobalSqlCommand.CommandText);

                previousCell = null;
                try
                {
                    Program.GlobalSqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    e.Cancel = true;
                }
            }
            else reader.Close();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (previousCell != null)
            {
                var cell = previousCell;
                previousCell = null;
                dataGridView.CurrentCell = cell;
            }
        }

        private void выгрузитьВExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "xlsx";
            saveFileDialog.Filter = "Файлы Excel|*.xl*";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = $"{saveFileDialog.InitialDirectory}\\{$"{Text} {DateTime.Now:dd_MM_yyyy HH_mm_ss}"}";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var excel = new Excel.Application
                {
                    Visible = false,
                    DisplayAlerts = false
                };
                excel.Workbooks.Add();
                var sheet = excel.ActiveSheet;

                int excelRowIndex = 1, excelColumnIndex = 1;
                foreach (DataGridViewColumn dataColumn in dataGridView.Columns)
                {
                    sheet.Cells[excelRowIndex, excelColumnIndex].Font.Bold = true;
                    sheet.Cells[excelRowIndex, excelColumnIndex++].Value = dataColumn.HeaderText;
                }
                foreach (DataGridViewRow dataRow in dataGridView.Rows)
                {
                    excelColumnIndex = 1;
                    excelRowIndex++;
                    foreach (DataGridViewCell dataCell in dataRow.Cells)
                        sheet.Cells[excelRowIndex, excelColumnIndex++].Value = dataCell.Value;
                }
                sheet.Columns.EntireColumn.AutoFit();

                excel.ActiveWorkbook.SaveAs(saveFileDialog.FileName);
                excel.Quit();
                excel = null;
            }
        }

        private void выгрузитьВWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "docx";
            saveFileDialog.Filter = "Файлы Word|*.do*";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = $"{saveFileDialog.InitialDirectory}\\{$"{Text} {DateTime.Now:dd_MM_yyyy HH_mm_ss}"}";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var word = new Word.Application
                {
                    Visible = false,
                    DisplayAlerts = Word.WdAlertLevel.wdAlertsNone
                };
                var document = word.Documents.Add();
                var cursor = word.Selection;

                cursor.Font.Bold = 1;
                cursor.Font.Size = 24;
                cursor.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                cursor.TypeText(Text);
                cursor.TypeParagraph();

                cursor.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                var table = word.ActiveDocument.Tables.Add(cursor.Range, 1, dataGridView.Columns.Count);
                table.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
                table.Range.Font.Size = 14;
                table.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalBottom;
                table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineWidth = Word.WdLineWidth.wdLineWidth225pt;
                foreach (var header in dataGridView.Columns.Cast<DataGridViewColumn>()
                    .ToDictionary(x => x.Index + 1, x => x.HeaderText))
                {
                    table.Cell(1, header.Key).Range.Font.Bold = 1;
                    table.Cell(1, header.Key).Range.Text = header.Value;
                }
                foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
                {
                    var row = table.Rows.Add();
                    row.Range.Bold = 0;
                    foreach (DataGridViewCell cell in dataGridViewRow.Cells)
                        row.Cells[cell.ColumnIndex + 1].Range.Text = $"{cell.Value}";
                }
                document.Range(document.Content.End - 1, document.Content.End).Select();
                cursor.TypeParagraph();

                document.SaveAs(saveFileDialog.FileName);
                word.Quit();
                word = null;
            }
        }
    }
}
