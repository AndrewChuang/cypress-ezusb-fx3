using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using CyUSB;
using System.Runtime.InteropServices;

namespace Streamer
{

    public class Form1 : System.Windows.Forms.Form
    {
        bool bVista;
        private System.Diagnostics.PerformanceCounter CpuCounter;

        USBDeviceList usbDevices;
        CyUSBDevice MyDevice;
        CyUSBEndPoint EndPoint;

        DateTime t1, t2;
        TimeSpan elapsed;
        double XferBytes;
        long xferRate;
        static byte DefaultBufInitValue = 0xA5;

        int BufSz;
        int QueueSz;
        int PPX;
        int IsoPktBlockSize;
        int Successes;
        int Failures;

        Thread tListen;
        bool bRunning;

        // These are  needed for Thread to update the UI
        delegate void UpdateUICallback();
        UpdateUICallback updateUI;

        // These are needed to close the app from the Thread exception(exception handling)
        delegate void ExceptionCallback();
        ExceptionCallback handleException;

        public Form1()
        {
            bVista = (Environment.OSVersion.Version.Major == 6);

            // Required for Windows Form Designer support
            InitializeComponent();

            // Late setup of the CPU monitor - (Bails-out if in Vista)
            InitializePeformanceMonitor();

            // Hide the CPU load monitor if running in Windows Vista
            // Detecting the CPU load is a security violation in Vista
            CPULoadBox.Visible = !bVista;

            // Setup the callback routine for updating the UI
            updateUI = new UpdateUICallback(StatusUpdate);

            // Setup the callback routine for NullReference exception handling
            handleException = new ExceptionCallback(ThreadException);

            // Create the list of USB devices attached to the CyUSB3.sys driver.
            usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);

            //Assign event handlers for device attachment and device removal.
            usbDevices.DeviceAttached += new EventHandler(usbDevices_DeviceAttached);
            usbDevices.DeviceRemoved += new EventHandler(usbDevices_DeviceRemoved);

            //Set and search the device with VID-PID 04b4-1003 and if found, selects the end point
            SetDevice();
        }


        /*Summary
           This is the event handler for device removal. This method resets the device count and searches for the device with 
           VID-PID 04b4-1003
        */
        void usbDevices_DeviceRemoved(object sender, EventArgs e)
        {
            MyDevice = null;
            EndPoint = null;
            SetDevice();
        }



        /*Summary
           This is the event handler for device attachment. This method  searches for the device with 
           VID-PID 04b4-00F1
        */
        void usbDevices_DeviceAttached(object sender, EventArgs e)
        {
            SetDevice();
        }



        /*Summary
           Search the device with VID-PID 04b4-00F1 and if found, select the end point
        */
        private void SetDevice()
        {
            USBDevice dev = usbDevices[0x04B4, 0x00F1];

            if (dev != null)
            {
                MyDevice = (CyUSBDevice)dev;

                GetEndpointsOfNode(MyDevice.Tree);
                PpxBox.Text = "8"; //Set default value to 8 Packets
                if (EndPointsComboBox.Items.Count > 0)
                {
                    EndPointsComboBox.SelectedIndex = 0;
                    StartBtn.Enabled = true;
                }
                Text = MyDevice.FriendlyName;
            }
            else
            {
                StartBtn.Enabled = false;
                EndPointsComboBox.Items.Clear();
                EndPointsComboBox.Text = "";
                Text = "C# Streamer - no device";
            }


        }



        /*Summary
           Recursive routine populates EndPointsComboBox with strings 
           representing all the endpoints in the device.
        */
        private void GetEndpointsOfNode(TreeNode devTree)
        {
            foreach (TreeNode node in devTree.Nodes)
            {
                if (node.Nodes.Count > 0)
                    GetEndpointsOfNode(node);
                else
                {
                    CyUSBEndPoint ept = node.Tag as CyUSBEndPoint;
                    if (ept == null)
                    {
                        //return;
                    }
                    else if (!node.Text.Contains("Control"))
                    {
                        CyUSBInterface ifc = node.Parent.Tag as CyUSBInterface;
                        string s = string.Format("ALT-{0}, {1} Byte {2}", ifc.bAlternateSetting, ept.MaxPktSize, node.Text);
                        EndPointsComboBox.Items.Add(s);
                    }
                    
                }
            }

        }




        /*Summary
           Clean up any resources being used.
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }



        /*Summary
           This method is used to do the initialization for detecting the CPU load
        */
        private void InitializePeformanceMonitor()
        {
            // This isn't allowed in Vista.
            if (bVista) return;

            CpuCounter = new System.Diagnostics.PerformanceCounter();
            ((System.ComponentModel.ISupportInitialize)(CpuCounter)).BeginInit();

            CpuCounter.CategoryName = "Processor";
            CpuCounter.CounterName = "% Processor Time";
            CpuCounter.InstanceName = "_Total";

            ((System.ComponentModel.ISupportInitialize)(CpuCounter)).EndInit();

        }


        #region Windows Form Designer generated code

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem ExitItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem AboutItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox PpxBox;
        private System.Windows.Forms.ComboBox QueueBox;
        private System.Windows.Forms.TextBox SuccessBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Label ThroughputLabel;
        private System.Windows.Forms.TextBox FailuresBox;
        private ComboBox EndPointsComboBox;
        private Label label5;
        private GroupBox CPULoadBox;
        private Label CpuLabel;
        private ProgressBar CpuBar;
        private System.Windows.Forms.Timer PerfTimer;

        private IContainer components;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.ExitItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.AboutItem = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PpxBox = new System.Windows.Forms.ComboBox();
            this.QueueBox = new System.Windows.Forms.ComboBox();
            this.SuccessBox = new System.Windows.Forms.TextBox();
            this.FailuresBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ThroughputLabel = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.StartBtn = new System.Windows.Forms.Button();
            this.EndPointsComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.CPULoadBox = new System.Windows.Forms.GroupBox();
            this.CpuLabel = new System.Windows.Forms.Label();
            this.CpuBar = new System.Windows.Forms.ProgressBar();
            this.PerfTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.CPULoadBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.ExitItem});
            this.menuItem1.Text = "File";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "-";
            // 
            // ExitItem
            // 
            this.ExitItem.Index = 1;
            this.ExitItem.Text = "Exit";
            this.ExitItem.Click += new System.EventHandler(this.ExitItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.AboutItem});
            this.menuItem3.Text = "Help";
            // 
            // AboutItem
            // 
            this.AboutItem.Index = 0;
            this.AboutItem.Text = "About";
            this.AboutItem.Click += new System.EventHandler(this.AboutItem_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Packets per Xfer";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Xfers to Queue";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(215, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Successes";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(215, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Failures";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // PpxBox
            // 
            this.PpxBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PpxBox.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256",
            "512"});
            this.PpxBox.Location = new System.Drawing.Point(111, 58);
            this.PpxBox.Name = "PpxBox";
            this.PpxBox.Size = new System.Drawing.Size(64, 21);
            this.PpxBox.TabIndex = 1;
            this.PpxBox.SelectedIndexChanged += new System.EventHandler(this.PpxBox_SelectedIndexChanged);
            // 
            // QueueBox
            // 
            this.QueueBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QueueBox.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64",
            "128"});
            this.QueueBox.Location = new System.Drawing.Point(111, 97);
            this.QueueBox.Name = "QueueBox";
            this.QueueBox.Size = new System.Drawing.Size(64, 21);
            this.QueueBox.TabIndex = 2;
            // 
            // SuccessBox
            // 
            this.SuccessBox.Location = new System.Drawing.Point(279, 58);
            this.SuccessBox.Name = "SuccessBox";
            this.SuccessBox.Size = new System.Drawing.Size(72, 20);
            this.SuccessBox.TabIndex = 6;
            this.SuccessBox.TabStop = false;
            this.SuccessBox.Text = "0";
            this.SuccessBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // FailuresBox
            // 
            this.FailuresBox.Location = new System.Drawing.Point(279, 98);
            this.FailuresBox.Name = "FailuresBox";
            this.FailuresBox.Size = new System.Drawing.Size(72, 20);
            this.FailuresBox.TabIndex = 7;
            this.FailuresBox.TabStop = false;
            this.FailuresBox.Text = "0";
            this.FailuresBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ThroughputLabel);
            this.groupBox1.Controls.Add(this.ProgressBar);
            this.groupBox1.Location = new System.Drawing.Point(19, 172);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 60);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Throughput (KB/s) ";
            // 
            // ThroughputLabel
            // 
            this.ThroughputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ThroughputLabel.Location = new System.Drawing.Point(114, 38);
            this.ThroughputLabel.Name = "ThroughputLabel";
            this.ThroughputLabel.Size = new System.Drawing.Size(100, 16);
            this.ThroughputLabel.TabIndex = 1;
            this.ThroughputLabel.Text = "0";
            this.ThroughputLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // ProgressBar
            // 
            this.ProgressBar.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.ProgressBar.Location = new System.Drawing.Point(16, 25);
            this.ProgressBar.Maximum = 500000;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(294, 10);
            this.ProgressBar.TabIndex = 0;
            // 
            // StartBtn
            // 
            this.StartBtn.BackColor = System.Drawing.Color.Aquamarine;
            this.StartBtn.Location = new System.Drawing.Point(223, 136);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(128, 20);
            this.StartBtn.TabIndex = 3;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = false;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // EndPointsComboBox
            // 
            this.EndPointsComboBox.DropDownHeight = 120;
            this.EndPointsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EndPointsComboBox.FormattingEnabled = true;
            this.EndPointsComboBox.IntegralHeight = false;
            this.EndPointsComboBox.Location = new System.Drawing.Point(111, 15);
            this.EndPointsComboBox.Name = "EndPointsComboBox";
            this.EndPointsComboBox.Size = new System.Drawing.Size(240, 21);
            this.EndPointsComboBox.TabIndex = 0;
            this.EndPointsComboBox.SelectedIndexChanged += new System.EventHandler(this.EndPointsComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Endpoint . . . . . . . ";
            // 
            // CPULoadBox
            // 
            this.CPULoadBox.Controls.Add(this.CpuLabel);
            this.CPULoadBox.Controls.Add(this.CpuBar);
            this.CPULoadBox.Location = new System.Drawing.Point(19, 247);
            this.CPULoadBox.Name = "CPULoadBox";
            this.CPULoadBox.Size = new System.Drawing.Size(330, 60);
            this.CPULoadBox.TabIndex = 12;
            this.CPULoadBox.TabStop = false;
            this.CPULoadBox.Text = " CPU Load ";
            // 
            // CpuLabel
            // 
            this.CpuLabel.AutoSize = true;
            this.CpuLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CpuLabel.Location = new System.Drawing.Point(150, 41);
            this.CpuLabel.Name = "CpuLabel";
            this.CpuLabel.Size = new System.Drawing.Size(27, 13);
            this.CpuLabel.TabIndex = 1;
            this.CpuLabel.Text = "0 %";
            this.CpuLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpuBar
            // 
            this.CpuBar.ForeColor = System.Drawing.Color.ForestGreen;
            this.CpuBar.Location = new System.Drawing.Point(16, 28);
            this.CpuBar.Name = "CpuBar";
            this.CpuBar.Size = new System.Drawing.Size(294, 10);
            this.CpuBar.Step = 1;
            this.CpuBar.TabIndex = 0;
            // 
            // PerfTimer
            // 
            this.PerfTimer.Enabled = true;
            this.PerfTimer.Interval = 500;
            this.PerfTimer.Tick += new System.EventHandler(this.PerfTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(374, 329);
            this.Controls.Add(this.CPULoadBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.EndPointsComboBox);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.FailuresBox);
            this.Controls.Add(this.SuccessBox);
            this.Controls.Add(this.QueueBox);
            this.Controls.Add(this.PpxBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "C# Streamer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.CPULoadBox.ResumeLayout(false);
            this.CPULoadBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion



        /*Summary
           The main entry point for the application.
        */
        [STAThread]
        static void Main()
        {
            try
            {
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, "Exception '" + e.Message + "' thrown by " + e.Source);
            }
        }



        /*Summary
           Executes on clicking Help->about
        */
        private void AboutItem_Click(object sender, System.EventArgs e)
        {
            string assemblyList = Util.Assemblies;
            MessageBox.Show(assemblyList, Text);
        }



        /*Summary
           Executes on clicking File->Exit
        */
        private void ExitItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            if (EndPointsComboBox.Items.Count > 0)
                EndPointsComboBox.SelectedIndex = 0;
        }



        /*Summary
           Executes on clicking close button
        */
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (usbDevices != null)
                usbDevices.Dispose();
        }



        /*Summary
         This is the System event handler.  
         Enforces valid values for PPX(Packet per transfer)
        */
        private void PpxBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EndPoint == null) return;

            int ppx = Convert.ToUInt16(PpxBox.Text);
            int len = EndPoint.MaxPktSize * ppx;

            int maxLen = 0x400000; // 4MBytes
            if (len > maxLen)
            {
                //ppx = maxLen / (EndPoint.MaxPktSize) / 8 * 8;
                if (EndPoint.MaxPktSize ==0)
                {
                    MessageBox.Show("Please correct MaxPacketSize in Descriptor", "Invalid MaxPacketSize");
                    return;
                }
                ppx = maxLen / (EndPoint.MaxPktSize);
                ppx -= (ppx % 8);
                MessageBox.Show("Maximum of 4MB per transfer.  Packets reduced.", "Invalid Packets per Xfer.");

                //Update the DropDown list for the packets
                int iIndex = PpxBox.SelectedIndex; // Get the packet index
                PpxBox.Items.Remove(PpxBox.Text); // Remove the Existing  Packet index
                PpxBox.Items.Insert(iIndex, ppx.ToString()); // insert the ppx
                PpxBox.SelectedIndex = iIndex; // update the selected item index

            }


            if ((MyDevice.bSuperSpeed || MyDevice.bHighSpeed) && (EndPoint.Attributes == 1) && (ppx < 8))
            {
                PpxBox.Text = "8";
                MessageBox.Show("Minimum of 8 Packets per Xfer required for HS/SS Isoc.", "Invalid Packets per Xfer.");
            }
            if ((MyDevice.bHighSpeed) && (EndPoint.Attributes == 1))
            {
                if (ppx > 128)
                {
                    PpxBox.Text = "128";
                    MessageBox.Show("Maximum 128 packets per transfer for High Speed Isoc", "Invalid Packets per Xfer.");
                }
            }

        }



        /*Summary
         This is a system event handler, when the selected index changes(end point selection).
        */
        private void EndPointsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the Alt setting
            string sAlt = EndPointsComboBox.Text.Substring(4, 1);
            byte a = Convert.ToByte(sAlt);
            MyDevice.AltIntfc = a;

            // Get the endpoint
            int aX = EndPointsComboBox.Text.LastIndexOf("0x");
            string sAddr = EndPointsComboBox.Text.Substring(aX, 4);
            byte addr = (byte)Util.HexToInt(sAddr);

            EndPoint = MyDevice.EndPointOf(addr);

            // Ensure valid PPX for this endpoint
            PpxBox_SelectedIndexChanged(sender, null);
        }



        /*Summary
          Executes on Start Button click 
        */
        private void StartBtn_Click(object sender, System.EventArgs e)
        {
            if (MyDevice == null)
                return;

            if (QueueBox.Text == "")
            {
                MessageBox.Show("Please Select Xfers to Queue", "Invalid Input");
                return;
            }

            if (StartBtn.Text.Equals("Start"))
            {
                EndPointsComboBox.Enabled = false;
                StartBtn.Text = "Stop";
                StartBtn.BackColor = Color.Pink;
                PpxBox.Enabled = false;
                QueueBox.Enabled = false;


                BufSz = EndPoint.MaxPktSize * Convert.ToUInt16(PpxBox.Text);
                QueueSz = Convert.ToUInt16(QueueBox.Text);
                PPX = Convert.ToUInt16(PpxBox.Text);

                EndPoint.XferSize = BufSz;

                if (EndPoint is CyIsocEndPoint)
                    IsoPktBlockSize = (EndPoint as CyIsocEndPoint).GetPktBlockSize(BufSz);
                else
                    IsoPktBlockSize = 0;

                bRunning = true;

                tListen = new Thread(new ThreadStart(XferThread));
                tListen.IsBackground = true;
                tListen.Priority = ThreadPriority.Highest;
                tListen.Start();
            }
            else
            {
                if (tListen.IsAlive)
                {
                    EndPointsComboBox.Enabled = true;
                    PpxBox.Enabled = true;
                    QueueBox.Enabled = true;
                    StartBtn.Text = "Start";
                    bRunning = false;

                    t2 = DateTime.Now;
                    elapsed = t2 - t1;
                    xferRate = (long)(XferBytes / elapsed.TotalMilliseconds);
                    xferRate = xferRate / (int)100 * (int)100;

                    tListen.Abort();
                    tListen.Join();
                    tListen = null;

                    StartBtn.BackColor = Color.Aquamarine;
                }

            }

        }


        /*Summary
          Data Xfer Thread entry point. Starts the thread on Start Button click 
        */
        public unsafe void XferThread()
        {
            // Setup the queue buffers
            byte[][] cmdBufs = new byte[QueueSz][];
            byte[][] xferBufs = new byte[QueueSz][];
            byte[][] ovLaps = new byte[QueueSz][];
            ISO_PKT_INFO[][] pktsInfo = new ISO_PKT_INFO[QueueSz][];

            int xStart = 0;

            try
            {
                LockNLoad(ref xStart, cmdBufs, xferBufs, ovLaps, pktsInfo);
            }
            catch (NullReferenceException e)
            {
                // This exception gets thrown if the device is unplugged 
                // while we're streaming data
                e.GetBaseException();
                this.Invoke(handleException);
            }
        }




        /*Summary
          This is a recursive routine for pinning all the buffers used in the transfer in memory.
        It will get recursively called QueueSz times.  On the QueueSz_th call, it will call
        XferData, which will loop, transferring data, until the stop button is clicked.
        Then, the recursion will unwind.
        */
        public unsafe void LockNLoad(ref int j, byte[][] cBufs, byte[][] xBufs, byte[][] oLaps, ISO_PKT_INFO[][] pktsInfo)
        {
            // Allocate one set of buffers for the queue, Buffered IO method require user to allocate a buffer as a part of command buffer,
            // the BeginDataXfer does not allocated it. BeginDataXfer will copy the data from the main buffer to the allocated while initializing the commands.
            cBufs[j] = new byte[CyConst.SINGLE_XFER_LEN + IsoPktBlockSize + ((EndPoint.XferMode == XMODE.BUFFERED) ? BufSz : 0)];
            
            xBufs[j] = new byte[BufSz];

            //initialize the buffer with initial value 0xA5
            for (int iIndex = 0; iIndex < BufSz; iIndex++)
                xBufs[j][iIndex] = DefaultBufInitValue;

            oLaps[j] = new byte[20];
            pktsInfo[j] = new ISO_PKT_INFO[PPX];

            fixed (byte* tL0 = oLaps[j], tc0 = cBufs[j], tb0 = xBufs[j])  // Pin the buffers in memory
            {
                OVERLAPPED* ovLapStatus = (OVERLAPPED*)tL0;
                ovLapStatus->hEvent = (IntPtr)PInvoke.CreateEvent(0, 0, 0, 0);

                // Pre-load the queue with a request
                int len = BufSz;
                EndPoint.BeginDataXfer(ref cBufs[j], ref xBufs[j], ref len, ref oLaps[j]);

                j++;

                if (j < QueueSz)
                    LockNLoad(ref j, cBufs, xBufs, oLaps, pktsInfo);  // Recursive call to pin next buffers in memory
                else
                    XferData(cBufs, xBufs, oLaps, pktsInfo);          // All loaded. Let's go!

            }

        }



        /*Summary
          Called at the end of recursive method, LockNLoad().
          XferData() implements the infinite transfer loop
        */
        public unsafe void XferData(byte[][] cBufs, byte[][] xBufs, byte[][] oLaps, ISO_PKT_INFO[][] pktsInfo)
        {
            int k = 0;
            int len = 0;

            Successes = 0;
            Failures = 0;

            XferBytes = 0;
            t1 = DateTime.Now;

            for (; bRunning; )
            {
                // WaitForXfer
                fixed (byte* tmpOvlap = oLaps[k])
                {
                    OVERLAPPED* ovLapStatus = (OVERLAPPED*)tmpOvlap;
                    if (!EndPoint.WaitForXfer(ovLapStatus->hEvent, 500))
                    {
                        EndPoint.Abort();
                        PInvoke.WaitForSingleObject(ovLapStatus->hEvent, 500);
                    }
                }

                if (EndPoint.Attributes == 1)
                {
                    CyIsocEndPoint isoc = EndPoint as CyIsocEndPoint;
                    // FinishDataXfer
                    if (isoc.FinishDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k], ref pktsInfo[k]))
                    {
                        //XferBytes += len;
                        //Successes++;

                        ISO_PKT_INFO[] pkts = pktsInfo[k];

                        for (int j = 0; j < PPX; j++)
                        {
                            if (pkts[j].Status == 0)
                            {
                                XferBytes += pkts[j].Length;

                                Successes++;
                            }
                            else
                                Failures++;

                            pkts[j].Length = 0;
                        }

                    }
                    else
                        Failures++;
                }
                else
                {
                    // FinishDataXfer
                    if (EndPoint.FinishDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k]))
                    {
                        XferBytes += len;
                        Successes++;
                    }
                    else
                        Failures++;

                }

                k++;
                if (k == QueueSz)  // Only update displayed stats once each time through the queue
                {
                    k = 0;

                    t2 = DateTime.Now;
                    elapsed = t2 - t1;

                    xferRate = (long)(XferBytes / elapsed.TotalMilliseconds);
                    xferRate = xferRate / (int)100 * (int)100;

                    // Call StatusUpdate() in the main thread
                    this.Invoke(updateUI);

                    // For small QueueSz or PPX, the loop is too tight for UI thread to ever get service.   
                    // Without this, app hangs in those scenarios.
                    Thread.Sleep(1);
                }

                // Re-submit this buffer into the queue
                len = BufSz;
                EndPoint.BeginDataXfer(ref cBufs[k], ref xBufs[k], ref len, ref oLaps[k]);



            } // End infinite loop

        }


        /*Summary
          The callback routine delegated to updateUI.
        */
        public void StatusUpdate()
        {
            if (xferRate > ProgressBar.Maximum)
                ProgressBar.Maximum = (int)(xferRate * 1.25);

            ProgressBar.Value = (int)xferRate;
            ThroughputLabel.Text = ProgressBar.Value.ToString();

            SuccessBox.Text = Successes.ToString();
            FailuresBox.Text = Failures.ToString();
        }


        /*Summary
          The callback routine delegated to handleException.
        */
        public void ThreadException()
        {
            StartBtn.Text = "Start";
            bRunning = false;

            t2 = DateTime.Now;
            elapsed = t2 - t1;
            xferRate = (long)(XferBytes / elapsed.TotalMilliseconds);
            xferRate = xferRate / (int)100 * (int)100;

            tListen = null;

            StartBtn.BackColor = Color.Aquamarine;

        }


        /*Summary
          Updates the CPU Load meter.
        */
        private void PerfTimer_Tick(object sender, EventArgs e)
        {
            if (bVista) return;

            float cpu = CpuCounter.NextValue();
            CpuBar.Value = (int)cpu;
            CpuLabel.Text = string.Format("{0} %", (int)cpu);

        }

    }
}
