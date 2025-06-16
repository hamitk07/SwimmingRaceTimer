using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;

namespace YüzmeSüreKayıtUygulaması
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Stopwatch stopwatch = new Stopwatch();
        private CancellationTokenSource cts;
        private bool running = false;

        private async void StartTimer()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;

            while (!token.IsCancellationRequested)
            {
                TimeSpan ts = stopwatch.Elapsed;

                lblTime.Invoke(new Action(() =>
                    lblTime.Text = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds/10)));
                await Task.Delay(10);
            }
        }

        bool isSaved = true;
        private void btnWrite_Click(object sender, EventArgs e)
        {
            btnWrite.Enabled = false;
            btnWrite.Enabled = true;

            if (listBoxSteps.Items.Count == 0)
            {
                MessageBox.Show("Boş liste kaydedilemez.");
            }
            else
            {
                isSaved = false;

                DialogResult msgBoxSave = MessageBox.Show("Verileri excel dosyasına kaydetmek istiyor musunuz?", "Kaydetme", MessageBoxButtons.YesNo);

                if (msgBoxSave == DialogResult.Yes)
                {
                    List<string> list = new List<string>();
                    foreach (var item in listBoxSteps.Items)
                    {
                        list.Add(item.ToString());
                    }
                    list.Reverse();

                    foreach (string item in list)
                    {
                        listBoxReverse.Items.Add(item);
                    }

                    Excel.Application excelApp = new Excel.Application();
                    excelApp.Visible = true; // Excel uygulamasını görünür yap

                    // Yeni bir Workbook oluştur
                    Excel.Workbook workbook = excelApp.Workbooks.Add();
                    Excel.Worksheet worksheet = workbook.Sheets[1]; // Aktif sayfayı al

                    for (int i = 0; i < listBoxReverse.Items.Count; i++)
                    {
                        worksheet.Cells[i + 1, 2] = listBoxReverse.Items[i];
                        worksheet.Cells[i + 1, 1] = (i + 1).ToString();
                    }

                    // Workbook'u kaydet ve Excel uygulamasını kapat
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    saveFileDialog.Title = "Save an Excel File";
                    saveFileDialog.ShowDialog();

                    if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Veriler Excel'e başarıyla eklendi.");
                        isSaved = true;
                    }

                    workbook.Close();
                    excelApp.Quit();

                    // Objeleri serbest bırak
                    Marshal.ReleaseComObject(worksheet);
                    Marshal.ReleaseComObject(workbook);
                    Marshal.ReleaseComObject(excelApp);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSaved == true)
            {
                DialogResult closeOrNot = MessageBox.Show("Uygulamayı kapatmak istiyor musunuz?", "Kapatma", MessageBoxButtons.OKCancel);
                if (closeOrNot == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                stopwatch.Stop();
            }
            else
            {
                DialogResult closeOrNot2 = MessageBox.Show("Kaydedilmemiş listeniz var, verileriniz silinecektir. Yine de kapatmak istiyor musunuz?", "Kapatma", MessageBoxButtons.OKCancel);
                if (closeOrNot2 == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                stopwatch.Stop();
            }  
        }

        private void btnStep_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnStep.PerformClick();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnStep.PerformClick();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnStep.PerformClick();
            }
        }

        private void lblTime_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnStep.PerformClick();
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnStep.PerformClick();
            }
        }
        bool isTimerOn = false;
        private void btnStartStop_Click_1(object sender, EventArgs e)
        {
            btnStartStop.Visible = false;
            btnStartStop.Visible = true;

            if (running)
            {
                cts.Cancel();
                stopwatch.Stop();
                btnStartStop.Text = "Başlat";
                isTimerOn = false;
            }
            else
            {
                stopwatch.Start();
                StartTimer();
                btnStartStop.Text = "Durdur";
                isTimerOn = true;
            }
            running = !running;
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            btnReset.Visible = false;
            btnReset.Visible = true;

            isSaved = true;

            DialogResult msgBoxResult = MessageBox.Show("Sıfırlama yapılacaktır. Kabul ediyor musunuz?", "Sıfırlama", MessageBoxButtons.YesNo);

            if (msgBoxResult == DialogResult.Yes)
            {
                if (cts != null)
                {
                    cts.Cancel();
                }
                stopwatch.Reset();
                running = false;
                btnStartStop.Text = "Başlat";
                lblTime.Text = "00:00:00:000";

                listBoxSteps.Items.Clear();
            } 
        }

        private void btnStep_Click_1(object sender, EventArgs e)
        {
            btnStep.Visible = false;
            btnStep.Visible = true;

            if (isTimerOn)
            {
                listBoxSteps.Items.Insert(0, lblTime.Text);
                listBoxSteps.TopIndex = 0;

                isSaved = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AcceptButton = null;
            this.CancelButton = null;
        }

        private void listBoxSteps_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnStep.PerformClick();
            }
        }

        private void btnStep_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnStep.PerformClick();
            }
        }
    }
}