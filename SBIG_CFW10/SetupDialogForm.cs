using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.SBIG_CFW10;

namespace ASCOM.SBIG_CFW10
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        public SetupDialogForm()
        {
            InitializeComponent();
            cmdOK.Enabled = false;
            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
        }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Place any validation constraint checks here
            // Update the state variables with results from the dialogue
            FilterWheel.comPort = (string)comboBoxComPort.SelectedItem;
            FilterWheel.tl.Enabled = chkTrace.Checked;

        }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void InitUI()
        {
            chkTrace.Checked = FilterWheel.tl.Enabled;                                      // Trace Logger saves every step in D:\user\Documents\ASCOM
            // set the list of com ports to those that are currently available
            comboBoxComPort.Items.Clear();
            comboBoxComPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());      // use System.IO because it's static
            // select the current port if possible
            if (comboBoxComPort.Items.Contains(FilterWheel.comPort))
            {
                comboBoxComPort.SelectedItem = FilterWheel.comPort;
            }
        }

        public ASCOM.Utilities.Serial port;

        private void checkbtn_Click(object sender, EventArgs e)
        {
            port = new ASCOM.Utilities.Serial();
            port.PortName = comboBoxComPort.SelectedItem.ToString();
            port.Speed = (SerialSpeed)9600;
            port.StopBits = SerialStopBits.One;
            port.DataBits = 8;
            port.Parity = SerialParity.None;

            try
            {
                port.Connected = true;
                byte[] request = new byte[6] { 0xA5, 0x3, 0x2, 0x0, 0x0, 0xAA };
                port.TransmitBinary(request);
                port.ReceiveTimeoutMs = 100;
                byte[] receive = port.ReceiveCountedBinary(6);
                txtConnect.AppendText("Connected");
                cmdOK.Enabled = true;
                port.Connected = false;
            }
            catch
            {
                MessageBox.Show("No SBIG CFW-10 filter wheel found on current COM port!", "ASCOM Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}