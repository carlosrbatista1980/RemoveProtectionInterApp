using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Security.Principal;

namespace RemoveProtectionInterApp
{
    public partial class Form1 : Form
    {
        bool existeCadeado = false;
        bool isAdm = false;
        string processPath = "";        

        public Form1()
        {
            InitializeComponent();
            //CheckAdmin();            
        }

        public void Form1_Load(object obj, EventArgs args)
        {
            CheckAdmin();            
        }


        private void CheckAdmin()
        {
            WindowsIdentity login = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(login);
            isAdm = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (isAdm)
            {
                CarregaTimer();
            }
            else
            {
                MessageBox.Show("Execute como administrador", "");
                this.Close();
            }
        }

        private void DestruirCadeado()
        {
            Process[] cadeado = Process.GetProcesses();

            foreach (Process proc in cadeado)
            {
                if (proc.ProcessName.Contains("qubnfe") || proc.ProcessName.Contains("winwmc"))
                {
                    processPath = proc.MainModule.FileName;
                    proc.Kill();
                    proc.WaitForExit();                    
                    if (processPath != "")
                    {
                        File.Delete(processPath);
                    }
                }                
            }
            
            Thread.Sleep(1000);
            CarregaTimer();
        }

        private void CarregaTimer()
        {
            timer1.Interval = 2000;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = true;
        }

        private void DetectarCadeado()
        {           

            Process[] cadeado = Process.GetProcesses();
            
            foreach (Process proc in cadeado)
            {
                if (proc.ProcessName == "qubnfe" || proc.ProcessName == "winwmc")
                {
                    label1.Text = "Cadeado Detectado !! ";
                    existeCadeado = true;
                    label1.ForeColor = Color.Red;
                    label3.Visible = true;
                    this.Update();
                    Thread.Sleep(1000);                    
                    label1.Text = "Removendo...";
                    label1.ForeColor = Color.Red;
                    label3.Visible = true;
                    this.Update();
                    Thread.Sleep(1000);                    
                }                
            }

            if (!existeCadeado)
            {
                label1.ForeColor = Color.Blue;
                label1.Text = "O InterApp não está executando.";                
                label3.Visible = true;                
                this.Update();
                //Thread.Sleep(1000);
                this.Update();
            }            
        }        

        private void timer1_Tick(object sender, EventArgs e)
        {                        
            label1.ForeColor = Color.Black;
            label1.Text = "....";
            label3.Visible = false;
            this.Update();
            Thread.Sleep(200);
            DetectarCadeado();
            if (existeCadeado)
            {
                DestruirCadeado();            
            }            
        }        
    }
}
