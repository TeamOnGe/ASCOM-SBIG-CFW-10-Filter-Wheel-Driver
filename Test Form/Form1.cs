using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;


namespace ASCOM.SBIG_CFW10
{
    public partial class Form1 : Form
    {

        private ASCOM.DriverAccess.FilterWheel driver;


        internal ASCOM.Utilities.Serial serPort;

        public Form1()
        {
            InitializeComponent();
            SetUIState();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsConnected)
                driver.Connected = false;

            Properties.Settings.Default.Save();
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DriverId = ASCOM.DriverAccess.FilterWheel.Choose(Properties.Settings.Default.DriverId);
            SetUIState();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                driver.Connected = false;
            }
            else
            {
                driver = new ASCOM.DriverAccess.FilterWheel(Properties.Settings.Default.DriverId);
                driver.Connected = true;
                serPort = new ASCOM.Utilities.Serial();
                serPort.ReceiveTimeout = 15;
                serPort.Port = 2;
                serPort.Speed = (SerialSpeed)9600;
                serPort.StopBits = SerialStopBits.One;
                serPort.DataBits = 8;
                serPort.Parity = SerialParity.None;
                serPort.Connected = true;
                serPort.ClearBuffers();
            }
            SetUIState();
        }

        private void SetUIState()
        {
            buttonConnect.Enabled = !string.IsNullOrEmpty(Properties.Settings.Default.DriverId);
            buttonChoose.Enabled = !IsConnected;
            buttonConnect.Text = IsConnected ? "Disconnect" : "Connect";
        }

        private bool IsConnected
        {
            get
            {
                return ((this.driver != null) && (driver.Connected == true));
            }
        }
    }
}
