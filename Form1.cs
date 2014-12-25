using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace AJE2Tester
{
    public partial class Form1 : Form
    {
        public AJESolver aje = new AJESolver();
        public double Area
        {
            get
            {
                return Convert.ToDouble(textBox1.Text);
            }
            set
            {
                textBox1.Text = value.ToString();
            }
        }
        public double TPR
        {
            get
            {
                return Convert.ToDouble(textBox15.Text);
            }
            set
            {
                textBox15.Text = value.ToString();
            }
        }
        public double BPR
        {
            get
            {
                return Convert.ToDouble(textBox2.Text);
            }
            set
            {
                textBox2.Text = value.ToString();
            }
        }
        public double FHV
        {
            get
            {
                return Convert.ToDouble(textBox12.Text);
            }
            set
            {
                textBox12.Text = value.ToString();
            }
        }

        public double FPR
        {
            get
            {
                return Convert.ToDouble(textBox4.Text);
            }
            set
            {
                textBox4.Text = value.ToString();
            }
        }

        public double CPR
        {
            get
            {
                return Convert.ToDouble(textBox5.Text);
            }
            set
            {
                textBox5.Text = value.ToString();
            }
        }
        public double TIT
        {
            get
            {
                return Convert.ToDouble(textBox6.Text);
            }
            set
            {
                textBox6.Text = value.ToString();
            }
        }
        public double TAB
        {
            get
            {
                return Convert.ToDouble(textBox7.Text);
            }
            set
            {
                textBox7.Text = value.ToString();
            }
        }
        public double eta_c
        {
            get
            {
                return Convert.ToDouble(textBox8.Text);
            }
            set
            {
                textBox8.Text = value.ToString();
            }
        }
        public double eta_t
        {
            get
            {
                return Convert.ToDouble(textBox3.Text);
            }
            set
            {
                textBox3.Text = value.ToString();
            }
        }
        public double eta_n
        {
            get
            {
                return Convert.ToDouble(textBox9.Text);
            }
            set
            {
                textBox9.Text = value.ToString();
            }
        }
        public double Mach_D
        {
            get
            {
                return Convert.ToDouble(textBox13.Text);
            }
            set
            {
                textBox13.Text = value.ToString();
            }
        }
        public double Temp_D
        {
            get
            {
                return Convert.ToDouble(textBox14.Text);
            }
            set
            {
                textBox14.Text = value.ToString();
            }
        }
        public string status_string
        {
            get
            {
                return textBox10.Text;
            }
            set
            {
                textBox10.Text = value;
            }
        }
        public string result_string
        {
            get
            {
                return textBox11.Text;
            }
            set
            {
                textBox11.Text = value;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            button2.PerformClick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            double temperature = hScrollBar1.Value + 188;
            double speed = hScrollBar2.Value * 10.0;
            double pressure = vScrollBar3.Value;
            double throttle = 100.0 - vScrollBar2.Value;
            
            status_string = "Temperature:\t" + temperature.ToString() + " K\r\n"
                + "Throttle:\t\t" + throttle.ToString() + " %\r\n"
                + "Pressure:\t\t" + pressure.ToString() + " Kpa\r\n"
                + "Speed:\t\t" + speed.ToString() + " m/s\r\n";

            status_string += "\r\n\r\n";
            status_string += "\tMODULE\r\n";
            status_string += "\t{\r\n";
            status_string += "\t\tname = AJEModule\r\n";
            status_string += "\t\tArea = " + Area.ToString() + "\r\n";
            status_string += "\t\tBPR = " + BPR.ToString() + "\r\n";
            status_string += "\t\tCPR = " + CPR.ToString() + "\r\n";
            status_string += "\t\tFPR = " + FPR.ToString() + "\r\n";
            status_string += "\t\tMdes = " + Mach_D.ToString() + "\r\n";
            status_string += "\t\tTdes = " + Temp_D.ToString() + "\r\n";
            status_string += "\t\teta_c = " + eta_c.ToString() + "\r\n";
            status_string += "\t\teta_t = " + eta_t.ToString() + "\r\n";
            status_string += "\t\teta_n = " + eta_n.ToString() + "\r\n";
            status_string += "\t\tFHV = " + FHV.ToString() + "\r\n";
            status_string += "\t\tTIT = " + TIT.ToString() + "\r\n";
            status_string += "\t\tTAB = " + TAB.ToString() + "\r\n";
            status_string += "\t\texhaustMixer = " + checkBox1.Checked.ToString() + "\r\n";
            status_string += "\t\tmaxThrust = 999999\r\n";
            status_string += "\t\tmaxT3 = 9999\r\n";
            status_string += "\t}\r\n\r\n\r\n";

            aje.InitializeOverallEngineData(
                Area,
                TPR,
                BPR,
                CPR,
                FPR,
                Mach_D,
                Temp_D,
                eta_c,
                eta_t,
                eta_n,
                FHV,
                TIT,
                TAB,
                checkBox1.Checked
            );
            aje.CalculatePerformance(pressure, temperature, speed, throttle/100);
            result_string = aje.debugstring;
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            button2.PerformClick();
        }

        private void vScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            button2.PerformClick();
        }

        private void vScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            button2.PerformClick();
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
