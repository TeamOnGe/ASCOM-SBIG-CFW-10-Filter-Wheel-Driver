using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;


namespace CFW10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnClose.Enabled = false;
            btnMove.Enabled = false;
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames()) 
            {
                comPort.Items.Add(s);
            }

        }

        public System.IO.Ports.SerialPort sport; //Port definieren

        public void serialport_connect(String port, int baudrate , Parity parity, int databits, StopBits stopbits)
        {
            DateTime date = DateTime.Now;
            String dtn = date.ToShortTimeString();

            sport = new System.IO.Ports.SerialPort(
                port, baudrate, parity, databits, stopbits);
            try
            {
                sport.Open();
                btnClose.Enabled = true;
                btnConnect.Enabled = false;
                txtPort.Clear();
                txtPort.AppendText("[" + dtn + "] " + "Connected");
                //sport.DataReceived += new SerialDataReceivedEventHandler(sport_DataRecieved);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error"); }
        }        

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                String port = comPort.Text;
                int baudrate = 9600; //Convert.ToInt32(comBaud.Text);
                Parity parity = Parity.None; //(Parity)Enum.Parse(typeof(Parity), comParity.Text);
                int databits = 8; //Convert.ToInt32(comDataBits.Text);
                StopBits stopbits = StopBits.One; //(StopBits)Enum.Parse(typeof(StopBits), comStopBits.Text);

                serialport_connect(port, baudrate, parity, databits, stopbits);

                btnMove.Enabled = true;

                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Start();
            }

            catch (Exception ex)
            {
                MessageBox.Show("ERROR: Please connect to a valid COM-Port!");
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            string err = "Error: Please Enter valid Hex-Code!";

            string pos = comPositions.Text;
            byte[] bit = new byte[6];

            if (pos == "Position 1")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x1, 0x0, 0xBA };
            }
            else if (pos == "Position 2")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x2, 0x0, 0xBB };
            }
            else if (pos == "Position 3")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x3, 0x0, 0xBC };
            }
            else if (pos == "Position 4")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x4, 0x0, 0xBD };
            }
            else if (pos == "Position 5")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x5, 0x0, 0xBE };
            }
            else if (pos == "Position 6")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x6, 0x0, 0xBF };
            }
            else if (pos == "Position 7")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x7, 0x0, 0xC0 };
            }
            else if (pos == "Position 8")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x8, 0x0, 0xC1 };
            }
            else if (pos == "Position 9")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0x9, 0x0, 0xC2 };
            }
            else if (pos == "Position 10")
            {
                bit = new byte[] { 0xA5, 0x3, 0x11, 0xA, 0x0, 0xC3 };
            }
            else
            {
                txtError.Text = err;
            }

            sport.Write(bit, 0, 6);
            Thread.Sleep(200);
            byte[] move = new byte[sport.BytesToRead];
            sport.Read(move, 0, sport.BytesToRead);
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (sport.IsOpen)
            {
                sport.Close();
                btnClose.Enabled = false;
                btnConnect.Enabled = true;
                btnMove.Enabled = false;
                txtPos.Clear();
                txtPort.Clear();
                txtPort.AppendText("Disconnected");
                timer1.Stop();
                txtError.Clear();

            }
        }

        //Timer wird beim Klicken des Connect Buttons gestartet. Sendet ständiges Signal an das Filterrad
        //zum Abfragen des aktuellen Status

        private void timer1_Tick(object sender, EventArgs e)     
        {
            txtError.Clear();
            byte[] request = new byte[] { 0xA5, 0x3, 0x2, 0x0, 0x0, 0xAA };
            sport.Write(request, 0, 6);
            Thread.Sleep(10);                                                  // Dem Filterrad Zeit zum Antworten geben
            byte[] recieve = new byte[6];
            sport.Read(recieve, 0, 6);
            byte posi = recieve[3];
            string pos = posi.ToString();
            //string pos = Convert.ToBase64String(posi);
            txtError.Text = ("Das Filterrad befindet sich auf Position:");
            txtPos.Text = (pos);
            int convert_string = Int32.Parse(pos);
            if (convert_string > 10)
            {
                txtPos.Clear();
                txtError.Clear();
                txtError.Text =("Filterwheel is moving...");
            }

        }
        
        public void Images(object sender, EventArgs e)
        {
            Image image = Image.FromFile("D:/tgenthner/VisualStudio_Projekte/cfw10.png");
            pictureBox1.Image = image;
            //pictureBox1.ImageLocation = ("D:/tgenthner/VisualStudio_Projekte/cfw10.png");
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }
    }//end class
}//end namespace
