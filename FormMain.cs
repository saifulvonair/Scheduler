using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduler
{
   
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            numHours.Value = DateTime.Now.Hour;
            numMins.Value = DateTime.Now.Minute;
            SystemEvents.PowerModeChanged += OnPowerChange;
        }

        SchedulerModel mSchedulerModel = new SchedulerModel();
        //
        private void startBtn_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
            listBox1.Items.Add("*Scheduler has been started!*");
            mSchedulerModel.setScheduleType(SchedulerModel.Scheduler.EveryMinutes);
            
            mSchedulerModel.start(delegate (object p)
            {
                // CallBack....
                string strText = (string)p;              
                this.BeginInvoke((Action)(() =>
                {
                    listBox1.Items.Add(strText);
                }));
            }, 1000); 
            
        }
        
        //
        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    {
                        listBox1.Items.Add(" case PowerModes.Resume at Time::" + DateTime.Now);
                    }
                    break;
                case PowerModes.Suspend:
                    listBox1.Items.Add(" PowerModes.Suspend at Time:" +DateTime.Now);
                    break;
            }
         }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = true;
            this.btnStop.Enabled = false;
            mSchedulerModel.cancel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mSchedulerModel.Running)
            {
                DialogResult result =  MessageBox.Show("Scheduler running! Do want to Calcel it?","Warining!", MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }
    }
}
