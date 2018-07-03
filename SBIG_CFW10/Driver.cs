//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM FilterWheel driver for SBIG_CFW10
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM FilterWheel interface version: <To be completed by driver developer>
// Author:		Timon Genthner   <timon.genthner@yahoo.de>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 29-06-2018	TG	6.0.0	ASCOM driver for a SBIG CFW-10 filter wheel.
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code canbe deleted and this definition removed.
#define FilterWheel

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;

namespace ASCOM.SBIG_CFW10
{
    //
    // Your driver's DeviceID is ASCOM.SBIG_CFW10.FilterWheel
    //
    // The Guid attribute sets the CLSID for ASCOM.SBIG_CFW10.FilterWheel
    // The ClassInterface/None addribute prevents an empty interface called
    // _SBIG_CFW10 from being created and used as the [default] interface
    //
    // TODO Replace the not implemented exceptions with code to implement the function or
    // throw the appropriate ASCOM exception.
    //

    /// <summary>
    /// ASCOM FilterWheel Driver for SBIG_CFW10.
    /// </summary>
    [Guid("598bcc0c-edd5-47da-ac5b-e462c4ff5774")]
    [ClassInterface(ClassInterfaceType.None)]
    /// [ProgId(FilterWheel.driverID)]
    public class FilterWheel : IFilterWheelV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal static string driverID = "ASCOM.SBIG_CFW10.FilterWheel";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        private static string driverDescription = "ASCOM SBIG CFW-10 Filter Wheel Driver";

        internal static string comPortProfileName = "COM Port"; // Constants used for Profile persistence
        internal static string comPortDefault = "COM1";
        internal static string traceStateProfileName = "Trace Level";
        internal static string traceStateDefault = "false";

        internal static String comPort; // Variables to hold the currrent device configuration

        internal static ASCOM.Utilities.Serial serPort;

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        private bool connectedState;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
        /// </summary>
        internal static TraceLogger tl;

        /// <summary>
        /// Initializes a new instance of the <see cref="SBIG_CFW10"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public FilterWheel()
        {
            tl = new TraceLogger("", "SBIG_CFW10");
            ReadProfile(); // Read device configuration from the ASCOM Profile store

            tl.LogMessage("FilterWheel", "Starting initialisation");

            connectedState = false; // Initialise connected to false
            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object
            //TODO: Implement your additional construction here

            tl.LogMessage("FilterWheel", "Completed initialisation");
        }


        //
        // PUBLIC COM INTERFACE IFilterWheelV2 IMPLEMENTATION
        //

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public void SetupDialog()
        {
            // consider only showing the setup dialog if not connected
            // or call a different dialog if connected
            if (IsConnected)
                System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

            using (SetupDialogForm F = new SetupDialogForm())
            {
                var result = F.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    WriteProfile(); // Persist device configuration values to the ASCOM Profile store
                }
            }
        }

        public ArrayList SupportedActions
        {
            get
            {
                tl.LogMessage("SupportedActions Get", "Returning empty arraylist");
                return new ArrayList();
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            LogMessage("", "Action {0}, parameters {1} not implemented", actionName, actionParameters);
            throw new ASCOM.ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
        }

        public void CommandBlind(string command, bool raw)
        {
            CheckConnected("CommandBlind");
            // Call CommandString and return as soon as it finishes
            this.CommandString(command, raw);
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBlind");
            // DO NOT have both these sections!  One or the other
        }

        public bool CommandBool(string command, bool raw)
        {
            CheckConnected("CommandBool");
            string ret = CommandString(command, raw);
            // TODO decode the return string and return true or false
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBool");
            // DO NOT have both these sections!  One or the other
        }

        public string CommandString(string command, bool raw)
        {
            CheckConnected("CommandString");
            // it's a good idea to put all the low level communication with the device here,
            // then all communication calls this function
            // you need something to ensure that only one command is in progress at a time

            throw new ASCOM.MethodNotImplementedException("CommandString");
        }

        public void Dispose()
        {
            // Clean up the tracelogger and util objects
            tl.Enabled = false;
            tl.Dispose();
            tl = null;
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }


        public bool Connected
        {
            get
            {
                LogMessage("Connected", "Get {0}", IsConnected);
                return IsConnected;
            }
            set
            {
                tl.LogMessage("Connected", "Set {0}", value);
                if (value == IsConnected)
                    return;

                if (value)
                {
                    connectedState = true;
                    LogMessage("Connected Set", "Connecting to port {0}", comPort);

                    // TODO connect to the device
                    
                    serPort = new ASCOM.Utilities.Serial();
                    serPort.ReceiveTimeoutMs = 15;
                    serPort.PortName = comPort;
                    serPort.Speed = (SerialSpeed)9600;
                    serPort.StopBits = SerialStopBits.One;
                    serPort.DataBits = 8;
                    serPort.Parity = SerialParity.None;
                    serPort.Connected = true;

                }
                else
                {
                    connectedState = false;
                    LogMessage("Connected Set", "Disconnecting from port {0}", comPort);

                    // TODO disconnect from the device

                    serPort.Connected = false;
                }
            }
        }

        public string Description
        {
            // TODO customise this device description
            get
            {
                tl.LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // TODO customise this driver description
                string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                LogMessage("InterfaceVersion Get", "2");
                return Convert.ToInt16("2");
            }
        }

        public string Name
        {
            get
            {
                string name = "CFW 10 Driver";
                tl.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region IFilerWheel Implementation
        private int[] fwOffsets = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //class level variable to hold focus offsets
        private string[] fwNames = new string[10] { "Filter 1", "Filter 2", "Filter 3", "Filter 4", "Filter 5", "Filter 6", "Filter 7", "Filter 8", "Filter 9", "Filter 10" }; //class level variable to hold the filter names
        private short fwPosition = 0; // class level variable to retain the current filterwheel position

        public int[] FocusOffsets
        {
            get
            {
                foreach (int fwOffset in fwOffsets) // Write filter offsets to the log
                {
                    tl.LogMessage("FocusOffsets Get", fwOffset.ToString());
                }

                return fwOffsets;
            }
        }

        public string[] Names
        {
            get
            {
                foreach (string fwName in fwNames) // Write filter names to the log
                {
                    tl.LogMessage("Names Get", fwName);
                }



                return fwNames;
            }
        }
        /// <summary>
        /// Position setting and position request to Port.
        /// </summary
        
        public short Position
        {
            get
            {
                byte[] request = new byte[6] { 0xA5, 0x3, 0x2, 0x0, 0x0, 0xAA };
                serPort.TransmitBinary(request);
                serPort.ReceiveTimeoutMs = 10;                                 //Kurze Pause (500ms) um Port Zeit zu geben zum Antworten
                byte[] receive = new byte[6];
                receive = serPort.ReceiveCountedBinary(6);
                byte pos = receive[3];                                          // Vierter Byte zeigt Position an Hex 0-9 = Dec 0-9. Daher möglich einfach in String zu konvertieren
                string str_receive = pos.ToString();
                short position = Int16.Parse(str_receive);
                tl.LogMessage("", "Current Position: " + str_receive);

                tl.LogMessage("Position Get", position.ToString());
                //return fwPosition;
                return position;
            }
            set
            {
                tl.LogMessage("Position Set", value.ToString());
                if ((value < 0) | (value > fwNames.Length - 1))
                {
                    tl.LogMessage("", "Throwing InvalidValueException - Position: " + value.ToString() + ", Range: 0 to " + (fwNames.Length - 1).ToString());
                    throw new InvalidValueException("Position", value.ToString(), "0 to " + (fwNames.Length - 1).ToString());
                }

                else if (value == 0)
                {
                    MoveFilter(1);
                }
                else if (value == 1)
                {
                    MoveFilter(2);
                }
                else if (value == 2)
                {
                    MoveFilter(3);
                }
                else if (value == 3)
                {
                    MoveFilter(4);
                }
                else if (value == 4)
                {
                    MoveFilter(5);
                }
                else if (value == 5)
                {
                    MoveFilter(6);
                }
                else if (value == 6)
                {
                    MoveFilter(7);
                }
                else if (value == 7)
                {
                    MoveFilter(8);
                }
                else if (value == 8)
                {
                    MoveFilter(9);
                }
                else if (value == 9)
                {
                    MoveFilter(10);
                }

                fwPosition = value;
            }
        }

        /// <summary>
        /// Contains all filter positions and sends the respective hex bytes to the Port
        /// </summary>

        public void MoveFilter(int pos)
        {
            byte[] position = new byte[6];

            if (pos == 1)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x1, 0x0, 0xBA };
            }

            else if (pos == 2)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x2, 0x0, 0xBB };
            }

            else if (pos == 3)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x3, 0x0, 0xBC };
            }

            else if (pos == 4)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x4, 0x0, 0xBD };
            }

            else if (pos == 5)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x5, 0x0, 0xBE };
            }

            else if (pos == 6)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x6, 0x0, 0xBF };
            }

            else if (pos == 7)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x7, 0x0, 0xC0 };
            }

            else if (pos == 8)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x8, 0x0, 0xC1 };
            }

            else if (pos == 9)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0x9, 0x0, 0xC2 };
            }

            else if (pos == 10)
            {
                position = new byte[] { 0xA5, 0x3, 0x11, 0xA, 0x0, 0xC3 };
            }

            serPort.TransmitBinary(position);

        }

        #endregion

        #region Private properties and methods
        // here are some useful properties and methods that can be used as required
        // to help with driver development

        #region ASCOM Registration

        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        /// <summary>
        /// Register or unregister the driver with the ASCOM Platform.
        /// This is harmless if the driver is already registered/unregistered.
        /// </summary>
        /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
        private static void RegUnregASCOM(bool bRegister)
        {
            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "FilterWheel";
                if (bRegister)
                {
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
                }
            }
        }

        /// <summary>
        /// This function registers the driver with the ASCOM Chooser and
        /// is called automatically whenever this class is registered for COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is successfully built.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
        /// </remarks>
        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        /// <summary>
        /// This function unregisters the driver from the ASCOM Chooser and
        /// is called automatically whenever this class is unregistered from COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is cleaned or prior to rebuilding.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
        /// </remarks>
        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }

        #endregion

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
        {
            get
            {

                if (serPort == null) return false;
                if (serPort.Connected == false) return false;
                // TODO check that the driver hardware connection exists and is connected to the hardware
                return connectedState;
            }
        }

        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        private void CheckConnected(string message)
        {
            if (!IsConnected)
            {
                throw new ASCOM.NotConnectedException(message);
            }
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "FilterWheel";
                tl.Enabled = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
                comPort = driverProfile.GetValue(driverID, comPortProfileName, string.Empty, comPortDefault);
            }
        }

        /// <summary>
        /// Write the device configuration to the  ASCOM  Profile store
        /// </summary>
        internal void WriteProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "FilterWheel";
                driverProfile.WriteValue(driverID, traceStateProfileName, tl.Enabled.ToString());
                driverProfile.WriteValue(driverID, comPortProfileName, comPort);
            }
        }

        /// <summary>
        /// Log helper function that takes formatted strings and arguments
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void LogMessage(string identifier, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            tl.LogMessage(identifier, msg);
        }
        #endregion
    }
}
