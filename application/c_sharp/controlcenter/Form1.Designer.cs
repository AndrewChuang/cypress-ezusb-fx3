using System.IO;

namespace CyControl
{
    public class TTransaction
    {
        public const byte ReqType_DIR_MASK = 0x80;
        public const byte ReqType_TYPE_MASK = 0x60;
        public const byte ReqType_TGT_MASK = 0x03;
        public const byte TotalHeaderSize = 32;

        public uint Signature; //4 
        public uint RecordSize; //8
        public ushort HeaderSize; //10
        public byte Tag; //11
        public byte ConfigNum; //12
        public byte IntfcNum; //13
        public byte AltIntfc; //14
        public byte EndPtAddr; //15

        public byte bReqType; //16 //EP0 Xfer
        public byte CtlReqCode; //17  //EP0 Xfer 
        public byte reserved0; //18 

        public ushort wValue; //20
        public ushort wIndex; //22
        public byte reserved1; //23 
        public byte reserved2; //24

        public uint Timeout; //28
        public uint DataLen; //32

        public TTransaction()
        {
            this.Signature = 0x54505343;
            this.HeaderSize = TotalHeaderSize;

            this.ConfigNum = 0;
            this.IntfcNum = 0;
            this.AltIntfc = 0;
            this.EndPtAddr = 0;

            this.Tag = 0;
            this.bReqType = 0;

            //this.Target = 0x00;//TGT_DEVICE
            //this.ReqType = 0x40;//REQ_VENDOR
            //this.Direction = 0x00; //DIR_TO_DEVICE           
        }

        public void WriteToStream(FileStream f)
        {
            BinaryWriter wr = new BinaryWriter(f);
            wr.Write(this.Signature);
            wr.Write(this.RecordSize);
            wr.Write(this.HeaderSize);
            wr.Write(this.Tag);
            wr.Write(this.ConfigNum);
            wr.Write(this.IntfcNum);
            wr.Write(this.AltIntfc);
            wr.Write(this.EndPtAddr);
            wr.Write(this.bReqType);
            wr.Write(this.CtlReqCode);
            wr.Write(this.reserved0);
            wr.Write(this.wValue);
            wr.Write(this.wIndex);
            wr.Write(this.reserved1);
            wr.Write(this.reserved2);
            wr.Write(this.Timeout);
            wr.Write(this.DataLen);
        }

        public void ReadFromStream(FileStream f)
        {
            BinaryReader rd = new BinaryReader(f);

            this.Signature = rd.ReadUInt32();
            this.RecordSize = rd.ReadUInt32();
            this.HeaderSize = rd.ReadUInt16();
            this.Tag = rd.ReadByte();
            this.ConfigNum = rd.ReadByte();
            this.IntfcNum = rd.ReadByte();
            this.AltIntfc = rd.ReadByte();
            this.EndPtAddr = rd.ReadByte();
            this.bReqType = rd.ReadByte();
            this.CtlReqCode = rd.ReadByte();
            this.reserved0 = rd.ReadByte();
            this.wValue = rd.ReadUInt16();
            this.wIndex = rd.ReadUInt16();
            this.reserved1 = rd.ReadByte();
            this.reserved2 = rd.ReadByte();
            this.Timeout = rd.ReadUInt32();
            this.DataLen = rd.ReadUInt32();
        }

        public void ReadToBuffer(FileStream f, ref byte[] buffer, ref int len)
        {
            if (len > 0)
            {
                BinaryReader rd = new BinaryReader(f);
                rd.Read(buffer, 0, len);
            }
        }

        public void WriteFromBuffer(FileStream f, ref byte[] buffer, ref int len)
        {
            if (len > 0)
            {
                BinaryWriter wr = new BinaryWriter(f);
                wr.Write(buffer, 0, len);
            }
        }
    }

    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programFX2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fX2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgramFX2RamMenuItm = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgramFX2smallEEPROMMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgramFX264kBEEPROMMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ProgramFX2haltMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgramFX2runMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fX3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgramFX3Ram = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgramFX3I2CEEPROM = new System.Windows.Forms.ToolStripMenuItem();
            this.sPIFLASHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UsersGuide = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Record = new System.Windows.Forms.ToolStripButton();
            this.Stop = new System.Windows.Forms.ToolStripButton();
            this.Pause = new System.Windows.Forms.ToolStripButton();
            this.Create_script = new System.Windows.Forms.ToolStripButton();
            this.Script_parameters = new System.Windows.Forms.ToolStripButton();
            this.load_button = new System.Windows.Forms.ToolStripButton();
            this.play_button = new System.Windows.Forms.ToolStripButton();
            this.Reset_device = new System.Windows.Forms.ToolStripButton();
            this.Reconnect_device = new System.Windows.Forms.ToolStripButton();
            this.Reset_endpoint = new System.Windows.Forms.ToolStripButton();
            this.Abort_endpoint = new System.Windows.Forms.ToolStripButton();
            this.Reset_Pipe = new System.Windows.Forms.ToolStripButton();
            this.Abort_Pipe = new System.Windows.Forms.ToolStripButton();
            this.URB_Stat = new System.Windows.Forms.ToolStripButton();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.Split1 = new System.Windows.Forms.SplitContainer();
            this.gb = new System.Windows.Forms.GroupBox();
            this.Ok = new System.Windows.Forms.Button();
            this.Addr = new System.Windows.Forms.Label();
            this.Cpu = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.DeviceTreeView = new System.Windows.Forms.TreeView();
            this.TabPages = new System.Windows.Forms.TabControl();
            this.DescrTab = new System.Windows.Forms.TabPage();
            this.DescText = new System.Windows.Forms.TextBox();
            this.XferTab = new System.Windows.Forms.TabPage();
            this.IsPacket = new System.Windows.Forms.CheckBox();
            this.Clear = new System.Windows.Forms.Button();
            this.XferDataBox = new System.Windows.Forms.MaskedTextBox();
            this.wValueBox = new System.Windows.Forms.TextBox();
            this.ReqCodeBox = new System.Windows.Forms.TextBox();
            this.wIndexBox = new System.Windows.Forms.TextBox();
            this.ReqCodeLabel = new System.Windows.Forms.Label();
            this.wIndexLabel = new System.Windows.Forms.Label();
            this.wValueLabel = new System.Windows.Forms.Label();
            this.TargetLabel = new System.Windows.Forms.Label();
            this.ReqTypeLabel = new System.Windows.Forms.Label();
            this.DirectionLabel = new System.Windows.Forms.Label();
            this.TargetBox = new System.Windows.Forms.ComboBox();
            this.ReqTypeBox = new System.Windows.Forms.ComboBox();
            this.DirectionBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.FileXferBtn = new System.Windows.Forms.Button();
            this.DataXferBtn = new System.Windows.Forms.Button();
            this.NumBytesBox = new System.Windows.Forms.TextBox();
            this.XferTextBox = new System.Windows.Forms.TextBox();
            this.OutputBox = new System.Windows.Forms.TextBox();
            this.DriversTab = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.CyUSBDeviceBox = new System.Windows.Forms.CheckBox();
            this.HIDDeviceBox = new System.Windows.Forms.CheckBox();
            this.MSCDeviceBox = new System.Windows.Forms.CheckBox();
            this.FOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.FSave = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.StatusBar.SuspendLayout();
            this.Split1.Panel1.SuspendLayout();
            this.Split1.Panel2.SuspendLayout();
            this.Split1.SuspendLayout();
            this.gb.SuspendLayout();
            this.TabPages.SuspendLayout();
            this.DescrTab.SuspendLayout();
            this.XferTab.SuspendLayout();
            this.DriversTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Silver;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.programFX2ToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(674, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // programFX2ToolStripMenuItem
            // 
            this.programFX2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fX2ToolStripMenuItem,
            this.fX3ToolStripMenuItem});
            this.programFX2ToolStripMenuItem.Name = "programFX2ToolStripMenuItem";
            this.programFX2ToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.programFX2ToolStripMenuItem.Text = "Program";
            this.programFX2ToolStripMenuItem.Click += new System.EventHandler(this.programFX2ToolStripMenuItem_Click);
            // 
            // fX2ToolStripMenuItem
            // 
            this.fX2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgramFX2RamMenuItm,
            this.ProgramFX2smallEEPROMMenuItem,
            this.ProgramFX264kBEEPROMMenuItem,
            this.toolStripSeparator1,
            this.ProgramFX2haltMenuItem,
            this.ProgramFX2runMenuItem});
            this.fX2ToolStripMenuItem.Enabled = false;
            this.fX2ToolStripMenuItem.Name = "fX2ToolStripMenuItem";
            this.fX2ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.fX2ToolStripMenuItem.Text = "FX2";
            // 
            // ProgramFX2RamMenuItm
            // 
            this.ProgramFX2RamMenuItm.Enabled = false;
            this.ProgramFX2RamMenuItm.Name = "ProgramFX2RamMenuItm";
            this.ProgramFX2RamMenuItm.Size = new System.Drawing.Size(153, 22);
            this.ProgramFX2RamMenuItm.Text = "RAM";
            this.ProgramFX2RamMenuItm.Click += new System.EventHandler(this.ProgE2Item_Click);
            // 
            // ProgramFX2smallEEPROMMenuItem
            // 
            this.ProgramFX2smallEEPROMMenuItem.Enabled = false;
            this.ProgramFX2smallEEPROMMenuItem.Name = "ProgramFX2smallEEPROMMenuItem";
            this.ProgramFX2smallEEPROMMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ProgramFX2smallEEPROMMenuItem.Text = "Small EEPROM";
            this.ProgramFX2smallEEPROMMenuItem.Click += new System.EventHandler(this.ProgE2Item_Click);
            // 
            // ProgramFX264kBEEPROMMenuItem
            // 
            this.ProgramFX264kBEEPROMMenuItem.Enabled = false;
            this.ProgramFX264kBEEPROMMenuItem.Name = "ProgramFX264kBEEPROMMenuItem";
            this.ProgramFX264kBEEPROMMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ProgramFX264kBEEPROMMenuItem.Text = "64KB EEPROM";
            this.ProgramFX264kBEEPROMMenuItem.Click += new System.EventHandler(this.ProgE2Item_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // ProgramFX2haltMenuItem
            // 
            this.ProgramFX2haltMenuItem.Enabled = false;
            this.ProgramFX2haltMenuItem.Name = "ProgramFX2haltMenuItem";
            this.ProgramFX2haltMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ProgramFX2haltMenuItem.Text = "Halt";
            this.ProgramFX2haltMenuItem.Click += new System.EventHandler(this.HaltItem_Click);
            // 
            // ProgramFX2runMenuItem
            // 
            this.ProgramFX2runMenuItem.Enabled = false;
            this.ProgramFX2runMenuItem.Name = "ProgramFX2runMenuItem";
            this.ProgramFX2runMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ProgramFX2runMenuItem.Text = "Run";
            this.ProgramFX2runMenuItem.Click += new System.EventHandler(this.HaltItem_Click);
            // 
            // fX3ToolStripMenuItem
            // 
            this.fX3ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgramFX3Ram,
            this.ProgramFX3I2CEEPROM,
            this.sPIFLASHToolStripMenuItem});
            this.fX3ToolStripMenuItem.Enabled = false;
            this.fX3ToolStripMenuItem.Name = "fX3ToolStripMenuItem";
            this.fX3ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.fX3ToolStripMenuItem.Text = "FX3";
            // 
            // ProgramFX3Ram
            // 
            this.ProgramFX3Ram.Enabled = false;
            this.ProgramFX3Ram.Name = "ProgramFX3Ram";
            this.ProgramFX3Ram.Size = new System.Drawing.Size(146, 22);
            this.ProgramFX3Ram.Text = "RAM";
            this.ProgramFX3Ram.Click += new System.EventHandler(this.ProgramFX3Ram_Click);
            // 
            // ProgramFX3I2CEEPROM
            // 
            this.ProgramFX3I2CEEPROM.Name = "ProgramFX3I2CEEPROM";
            this.ProgramFX3I2CEEPROM.Size = new System.Drawing.Size(146, 22);
            this.ProgramFX3I2CEEPROM.Text = "I2C E2PROM";
            this.ProgramFX3I2CEEPROM.Click += new System.EventHandler(this.ProgramFX3I2CEEPROM_Click);
            // 
            // sPIFLASHToolStripMenuItem
            // 
            this.sPIFLASHToolStripMenuItem.Name = "sPIFLASHToolStripMenuItem";
            this.sPIFLASHToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.sPIFLASHToolStripMenuItem.Text = "SPI FLASH";
            this.sPIFLASHToolStripMenuItem.Click += new System.EventHandler(this.sPIFLASHToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UsersGuide,
            this.AboutMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // UsersGuide
            // 
            this.UsersGuide.Name = "UsersGuide";
            this.UsersGuide.Size = new System.Drawing.Size(139, 22);
            this.UsersGuide.Text = "Help Topics";
            this.UsersGuide.Click += new System.EventHandler(this.UsersGuide_Click);
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.Size = new System.Drawing.Size(139, 22);
            this.AboutMenuItem.Text = "About";
            this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Gainsboro;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Record,
            this.Stop,
            this.Pause,
            this.Create_script,
            this.Script_parameters,
            this.load_button,
            this.play_button,
            this.Reset_device,
            this.Reconnect_device,
            this.Reset_endpoint,
            this.Abort_endpoint,
            this.Reset_Pipe,
            this.Abort_Pipe,
            this.URB_Stat});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(674, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Record
            // 
            this.Record.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Record.Image = ((System.Drawing.Image)(resources.GetObject("Record.Image")));
            this.Record.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Record.Name = "Record";
            this.Record.Size = new System.Drawing.Size(23, 22);
            this.Record.Text = "Start Recording";
            this.Record.Click += new System.EventHandler(this.Record_Click);
            // 
            // Stop
            // 
            this.Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Stop.Enabled = false;
            this.Stop.Image = ((System.Drawing.Image)(resources.GetObject("Stop.Image")));
            this.Stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(23, 22);
            this.Stop.Text = "Stop Recording";
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Pause
            // 
            this.Pause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Pause.Enabled = false;
            this.Pause.Image = ((System.Drawing.Image)(resources.GetObject("Pause.Image")));
            this.Pause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Pause.Name = "Pause";
            this.Pause.Size = new System.Drawing.Size(23, 22);
            this.Pause.Text = "Insert 100ms wait";
            this.Pause.Click += new System.EventHandler(this.Pause_Click);
            // 
            // Create_script
            // 
            this.Create_script.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Create_script.Enabled = false;
            this.Create_script.Image = ((System.Drawing.Image)(resources.GetObject("Create_script.Image")));
            this.Create_script.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Create_script.Name = "Create_script";
            this.Create_script.Size = new System.Drawing.Size(23, 22);
            this.Create_script.Text = "Create Script";
            this.Create_script.Click += new System.EventHandler(this.Create_script_Click);
            // 
            // Script_parameters
            // 
            this.Script_parameters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Script_parameters.Enabled = false;
            this.Script_parameters.Image = ((System.Drawing.Image)(resources.GetObject("Script_parameters.Image")));
            this.Script_parameters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Script_parameters.Name = "Script_parameters";
            this.Script_parameters.Size = new System.Drawing.Size(23, 22);
            this.Script_parameters.Text = "Script Parameters";
            this.Script_parameters.Click += new System.EventHandler(this.Script_parameters_Click);
            // 
            // load_button
            // 
            this.load_button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.load_button.Image = ((System.Drawing.Image)(resources.GetObject("load_button.Image")));
            this.load_button.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.load_button.Name = "load_button";
            this.load_button.Size = new System.Drawing.Size(23, 22);
            this.load_button.Text = "Load Script";
            this.load_button.Click += new System.EventHandler(this.load_button_Click);
            // 
            // play_button
            // 
            this.play_button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.play_button.Enabled = false;
            this.play_button.Image = ((System.Drawing.Image)(resources.GetObject("play_button.Image")));
            this.play_button.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.play_button.Name = "play_button";
            this.play_button.Size = new System.Drawing.Size(23, 22);
            this.play_button.Text = "Play Script";
            this.play_button.Click += new System.EventHandler(this.play_button_Click);
            // 
            // Reset_device
            // 
            this.Reset_device.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Reset_device.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Reset_device.Image = ((System.Drawing.Image)(resources.GetObject("Reset_device.Image")));
            this.Reset_device.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.Reset_device.Name = "Reset_device";
            this.Reset_device.Size = new System.Drawing.Size(23, 22);
            this.Reset_device.Text = "Reset Device";
            this.Reset_device.Click += new System.EventHandler(this.Reset_device_Click);
            // 
            // Reconnect_device
            // 
            this.Reconnect_device.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Reconnect_device.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Reconnect_device.Image = ((System.Drawing.Image)(resources.GetObject("Reconnect_device.Image")));
            this.Reconnect_device.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.Reconnect_device.Name = "Reconnect_device";
            this.Reconnect_device.Size = new System.Drawing.Size(23, 22);
            this.Reconnect_device.Text = "Reconnect Device";
            this.Reconnect_device.Click += new System.EventHandler(this.Reconnect_device_Click);
            // 
            // Reset_endpoint
            // 
            this.Reset_endpoint.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Reset_endpoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Reset_endpoint.Image = ((System.Drawing.Image)(resources.GetObject("Reset_endpoint.Image")));
            this.Reset_endpoint.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.Reset_endpoint.Name = "Reset_endpoint";
            this.Reset_endpoint.Size = new System.Drawing.Size(23, 22);
            this.Reset_endpoint.Text = "Reset Endpoint";
            this.Reset_endpoint.Click += new System.EventHandler(this.Reset_endpoint_Click);
            // 
            // Abort_endpoint
            // 
            this.Abort_endpoint.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Abort_endpoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Abort_endpoint.Image = ((System.Drawing.Image)(resources.GetObject("Abort_endpoint.Image")));
            this.Abort_endpoint.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.Abort_endpoint.Name = "Abort_endpoint";
            this.Abort_endpoint.Size = new System.Drawing.Size(23, 22);
            this.Abort_endpoint.Text = "Abort Endpoint";
            this.Abort_endpoint.Click += new System.EventHandler(this.Abort_endpoint_Click);
            // 
            // Reset_Pipe
            // 
            this.Reset_Pipe.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Reset_Pipe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Reset_Pipe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Reset_Pipe.Name = "Reset_Pipe";
            this.Reset_Pipe.Size = new System.Drawing.Size(62, 22);
            this.Reset_Pipe.Text = "Reset Pipe";
            this.Reset_Pipe.Click += new System.EventHandler(this.Reset_Pipe_Click);
            // 
            // Abort_Pipe
            // 
            this.Abort_Pipe.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Abort_Pipe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Abort_Pipe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Abort_Pipe.Name = "Abort_Pipe";
            this.Abort_Pipe.Size = new System.Drawing.Size(61, 22);
            this.Abort_Pipe.Text = "Abort Pipe";
            this.Abort_Pipe.Click += new System.EventHandler(this.Abort_Pipe_Click);
            // 
            // URB_Stat
            // 
            this.URB_Stat.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.URB_Stat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.URB_Stat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.URB_Stat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.URB_Stat.Name = "URB_Stat";
            this.URB_Stat.Size = new System.Drawing.Size(54, 22);
            this.URB_Stat.Text = "URB Stat";
            this.URB_Stat.Click += new System.EventHandler(this.URB_Stat_Click);
            // 
            // StatusBar
            // 
            this.StatusBar.BackColor = System.Drawing.SystemColors.Control;
            this.StatusBar.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.StatLabel});
            this.StatusBar.Location = new System.Drawing.Point(0, 533);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(674, 22);
            this.StatusBar.TabIndex = 2;
            // 
            // StatusLabel
            // 
            this.StatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // StatLabel
            // 
            this.StatLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.StatLabel.Name = "StatLabel";
            this.StatLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // Split1
            // 
            this.Split1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Split1.Location = new System.Drawing.Point(0, 49);
            this.Split1.Name = "Split1";
            // 
            // Split1.Panel1
            // 
            this.Split1.Panel1.Controls.Add(this.gb);
            this.Split1.Panel1.Controls.Add(this.DeviceTreeView);
            // 
            // Split1.Panel2
            // 
            this.Split1.Panel2.Controls.Add(this.TabPages);
            this.Split1.Size = new System.Drawing.Size(674, 484);
            this.Split1.SplitterDistance = 244;
            this.Split1.TabIndex = 3;
            // 
            // gb
            // 
            this.gb.BackColor = System.Drawing.Color.Gainsboro;
            this.gb.Controls.Add(this.Ok);
            this.gb.Controls.Add(this.Addr);
            this.gb.Controls.Add(this.Cpu);
            this.gb.Controls.Add(this.textBox2);
            this.gb.Controls.Add(this.textBox1);
            this.gb.Location = new System.Drawing.Point(0, 322);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(243, 162);
            this.gb.TabIndex = 1;
            this.gb.TabStop = false;
            this.gb.Text = "Change Parameters";
            this.gb.Visible = false;
            // 
            // Ok
            // 
            this.Ok.Location = new System.Drawing.Point(83, 119);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(75, 23);
            this.Ok.TabIndex = 4;
            this.Ok.Text = "OK";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // Addr
            // 
            this.Addr.AutoSize = true;
            this.Addr.Location = new System.Drawing.Point(0, 80);
            this.Addr.Name = "Addr";
            this.Addr.Size = new System.Drawing.Size(129, 13);
            this.Addr.TabIndex = 3;
            this.Addr.Text = "Maximum internal Address";
            // 
            // Cpu
            // 
            this.Cpu.AutoSize = true;
            this.Cpu.Location = new System.Drawing.Point(0, 32);
            this.Cpu.Name = "Cpu";
            this.Cpu.Size = new System.Drawing.Size(117, 13);
            this.Cpu.TabIndex = 2;
            this.Cpu.Text = "Location of CPUCS reg";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(137, 80);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "0x4000";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(137, 32);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "0xE600";
            // 
            // DeviceTreeView
            // 
            this.DeviceTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DeviceTreeView.HideSelection = false;
            this.DeviceTreeView.Location = new System.Drawing.Point(0, 0);
            this.DeviceTreeView.Name = "DeviceTreeView";
            this.DeviceTreeView.Size = new System.Drawing.Size(244, 484);
            this.DeviceTreeView.TabIndex = 0;
            this.DeviceTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DeviceTreeView_AfterSelect);
            // 
            // TabPages
            // 
            this.TabPages.Controls.Add(this.DescrTab);
            this.TabPages.Controls.Add(this.XferTab);
            this.TabPages.Controls.Add(this.DriversTab);
            this.TabPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabPages.Location = new System.Drawing.Point(0, 0);
            this.TabPages.Name = "TabPages";
            this.TabPages.SelectedIndex = 0;
            this.TabPages.Size = new System.Drawing.Size(426, 484);
            this.TabPages.TabIndex = 0;
            this.TabPages.SelectedIndexChanged += new System.EventHandler(this.Form1_Resize);
            // 
            // DescrTab
            // 
            this.DescrTab.Controls.Add(this.DescText);
            this.DescrTab.Location = new System.Drawing.Point(4, 22);
            this.DescrTab.Name = "DescrTab";
            this.DescrTab.Padding = new System.Windows.Forms.Padding(3);
            this.DescrTab.Size = new System.Drawing.Size(418, 458);
            this.DescrTab.TabIndex = 0;
            this.DescrTab.Text = "Descriptor Info";
            this.DescrTab.UseVisualStyleBackColor = true;
            // 
            // DescText
            // 
            this.DescText.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.DescText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescText.Location = new System.Drawing.Point(3, 3);
            this.DescText.Multiline = true;
            this.DescText.Name = "DescText";
            this.DescText.ReadOnly = true;
            this.DescText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DescText.Size = new System.Drawing.Size(412, 452);
            this.DescText.TabIndex = 0;
            this.DescText.WordWrap = false;
            // 
            // XferTab
            // 
            this.XferTab.BackColor = System.Drawing.Color.Gainsboro;
            this.XferTab.Controls.Add(this.IsPacket);
            this.XferTab.Controls.Add(this.Clear);
            this.XferTab.Controls.Add(this.XferDataBox);
            this.XferTab.Controls.Add(this.wValueBox);
            this.XferTab.Controls.Add(this.ReqCodeBox);
            this.XferTab.Controls.Add(this.wIndexBox);
            this.XferTab.Controls.Add(this.ReqCodeLabel);
            this.XferTab.Controls.Add(this.wIndexLabel);
            this.XferTab.Controls.Add(this.wValueLabel);
            this.XferTab.Controls.Add(this.TargetLabel);
            this.XferTab.Controls.Add(this.ReqTypeLabel);
            this.XferTab.Controls.Add(this.DirectionLabel);
            this.XferTab.Controls.Add(this.TargetBox);
            this.XferTab.Controls.Add(this.ReqTypeBox);
            this.XferTab.Controls.Add(this.DirectionBox);
            this.XferTab.Controls.Add(this.label3);
            this.XferTab.Controls.Add(this.label2);
            this.XferTab.Controls.Add(this.label1);
            this.XferTab.Controls.Add(this.FileXferBtn);
            this.XferTab.Controls.Add(this.DataXferBtn);
            this.XferTab.Controls.Add(this.NumBytesBox);
            this.XferTab.Controls.Add(this.XferTextBox);
            this.XferTab.Controls.Add(this.OutputBox);
            this.XferTab.Location = new System.Drawing.Point(4, 22);
            this.XferTab.Name = "XferTab";
            this.XferTab.Padding = new System.Windows.Forms.Padding(3);
            this.XferTab.Size = new System.Drawing.Size(418, 458);
            this.XferTab.TabIndex = 1;
            this.XferTab.Text = "Data Transfers";
            // 
            // IsPacket
            // 
            this.IsPacket.AutoSize = true;
            this.IsPacket.Location = new System.Drawing.Point(163, 69);
            this.IsPacket.Name = "IsPacket";
            this.IsPacket.Size = new System.Drawing.Size(69, 17);
            this.IsPacket.TabIndex = 27;
            this.IsPacket.Text = "PktMode";
            this.IsPacket.UseVisualStyleBackColor = true;
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(324, 95);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(86, 23);
            this.Clear.TabIndex = 26;
            this.Clear.Text = "Clear Box";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // XferDataBox
            // 
            this.XferDataBox.Location = new System.Drawing.Point(156, 29);
            this.XferDataBox.Name = "XferDataBox";
            this.XferDataBox.PromptChar = ' ';
            this.XferDataBox.Size = new System.Drawing.Size(241, 20);
            this.XferDataBox.TabIndex = 21;
            this.XferDataBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.XferDataBox_KeyUp);
            // 
            // wValueBox
            // 
            this.wValueBox.Location = new System.Drawing.Point(171, 168);
            this.wValueBox.Name = "wValueBox";
            this.wValueBox.Size = new System.Drawing.Size(55, 20);
            this.wValueBox.TabIndex = 20;
            this.wValueBox.Text = "0x0000";
            // 
            // ReqCodeBox
            // 
            this.ReqCodeBox.Location = new System.Drawing.Point(58, 168);
            this.ReqCodeBox.Name = "ReqCodeBox";
            this.ReqCodeBox.Size = new System.Drawing.Size(42, 20);
            this.ReqCodeBox.TabIndex = 19;
            this.ReqCodeBox.Text = "0x00";
            // 
            // wIndexBox
            // 
            this.wIndexBox.Location = new System.Drawing.Point(325, 168);
            this.wIndexBox.Name = "wIndexBox";
            this.wIndexBox.Size = new System.Drawing.Size(55, 20);
            this.wIndexBox.TabIndex = 18;
            this.wIndexBox.Text = "0x0000";
            // 
            // ReqCodeLabel
            // 
            this.ReqCodeLabel.AutoSize = true;
            this.ReqCodeLabel.Location = new System.Drawing.Point(3, 171);
            this.ReqCodeLabel.Name = "ReqCodeLabel";
            this.ReqCodeLabel.Size = new System.Drawing.Size(55, 13);
            this.ReqCodeLabel.TabIndex = 17;
            this.ReqCodeLabel.Text = "Req Code";
            this.ReqCodeLabel.Visible = false;
            // 
            // wIndexLabel
            // 
            this.wIndexLabel.AutoSize = true;
            this.wIndexLabel.Location = new System.Drawing.Point(281, 171);
            this.wIndexLabel.Name = "wIndexLabel";
            this.wIndexLabel.Size = new System.Drawing.Size(41, 13);
            this.wIndexLabel.TabIndex = 16;
            this.wIndexLabel.Text = "wIndex";
            this.wIndexLabel.Visible = false;
            // 
            // wValueLabel
            // 
            this.wValueLabel.AutoSize = true;
            this.wValueLabel.Location = new System.Drawing.Point(123, 171);
            this.wValueLabel.Name = "wValueLabel";
            this.wValueLabel.Size = new System.Drawing.Size(42, 13);
            this.wValueLabel.TabIndex = 15;
            this.wValueLabel.Text = "wValue";
            this.wValueLabel.Visible = false;
            // 
            // TargetLabel
            // 
            this.TargetLabel.AutoSize = true;
            this.TargetLabel.Location = new System.Drawing.Point(281, 133);
            this.TargetLabel.Name = "TargetLabel";
            this.TargetLabel.Size = new System.Drawing.Size(38, 13);
            this.TargetLabel.TabIndex = 14;
            this.TargetLabel.Text = "Target";
            this.TargetLabel.Visible = false;
            // 
            // ReqTypeLabel
            // 
            this.ReqTypeLabel.AutoSize = true;
            this.ReqTypeLabel.Location = new System.Drawing.Point(111, 133);
            this.ReqTypeLabel.Name = "ReqTypeLabel";
            this.ReqTypeLabel.Size = new System.Drawing.Size(54, 13);
            this.ReqTypeLabel.TabIndex = 13;
            this.ReqTypeLabel.Text = "Req Type";
            this.ReqTypeLabel.Visible = false;
            // 
            // DirectionLabel
            // 
            this.DirectionLabel.AutoSize = true;
            this.DirectionLabel.Location = new System.Drawing.Point(3, 133);
            this.DirectionLabel.Name = "DirectionLabel";
            this.DirectionLabel.Size = new System.Drawing.Size(49, 13);
            this.DirectionLabel.TabIndex = 12;
            this.DirectionLabel.Text = "Direction";
            this.DirectionLabel.Visible = false;
            // 
            // TargetBox
            // 
            this.TargetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TargetBox.FormattingEnabled = true;
            this.TargetBox.Items.AddRange(new object[] {
            "Device",
            "Interface",
            "Endpoint",
            "Other"});
            this.TargetBox.Location = new System.Drawing.Point(325, 130);
            this.TargetBox.Name = "TargetBox";
            this.TargetBox.Size = new System.Drawing.Size(72, 21);
            this.TargetBox.TabIndex = 11;
            this.TargetBox.SelectedIndex = 0;
            // 
            // ReqTypeBox
            // 
            this.ReqTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ReqTypeBox.FormattingEnabled = true;
            this.ReqTypeBox.Items.AddRange(new object[] {
            "Standard",
            "Class",
            "Vendor"});
            this.ReqTypeBox.Location = new System.Drawing.Point(171, 130);
            this.ReqTypeBox.Name = "ReqTypeBox";
            this.ReqTypeBox.Size = new System.Drawing.Size(72, 21);
            this.ReqTypeBox.TabIndex = 10;
            this.ReqTypeBox.SelectedIndex = 0;
            // 
            // DirectionBox
            // 
            this.DirectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DirectionBox.FormattingEnabled = true;
            this.DirectionBox.Items.AddRange(new object[] {
            "Out",
            "In"});
            this.DirectionBox.Location = new System.Drawing.Point(58, 130);
            this.DirectionBox.Name = "DirectionBox";
            this.DirectionBox.Size = new System.Drawing.Size(42, 21);
            this.DirectionBox.TabIndex = 9;
            this.DirectionBox.SelectedIndex =0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Bytes to Transfer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Data to send (Hex)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Text to send";
            // 
            // FileXferBtn
            // 
            this.FileXferBtn.Enabled = false;
            this.FileXferBtn.Location = new System.Drawing.Point(164, 95);
            this.FileXferBtn.Name = "FileXferBtn";
            this.FileXferBtn.Size = new System.Drawing.Size(110, 23);
            this.FileXferBtn.TabIndex = 5;
            this.FileXferBtn.Text = "Transfer File";
            this.FileXferBtn.UseVisualStyleBackColor = true;
            this.FileXferBtn.Visible = false;
            this.FileXferBtn.Click += new System.EventHandler(this.FileXferBtn_Click);
            // 
            // DataXferBtn
            // 
            this.DataXferBtn.Location = new System.Drawing.Point(6, 95);
            this.DataXferBtn.Name = "DataXferBtn";
            this.DataXferBtn.Size = new System.Drawing.Size(110, 23);
            this.DataXferBtn.TabIndex = 4;
            this.DataXferBtn.Text = "Transfer Data";
            this.DataXferBtn.UseVisualStyleBackColor = true;
            this.DataXferBtn.Click += new System.EventHandler(this.DataXferBtn_Click);
            // 
            // NumBytesBox
            // 
            this.NumBytesBox.Location = new System.Drawing.Point(6, 68);
            this.NumBytesBox.Name = "NumBytesBox";
            this.NumBytesBox.Size = new System.Drawing.Size(87, 20);
            this.NumBytesBox.TabIndex = 3;
            this.NumBytesBox.Text = "512";
            this.NumBytesBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // XferTextBox
            // 
            this.XferTextBox.Location = new System.Drawing.Point(6, 29);
            this.XferTextBox.MaxLength = 2048;
            this.XferTextBox.Name = "XferTextBox";
            this.XferTextBox.Size = new System.Drawing.Size(131, 20);
            this.XferTextBox.TabIndex = 1;
            this.XferTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.XferTextBox_KeyUp);
            // 
            // OutputBox
            // 
            this.OutputBox.BackColor = System.Drawing.SystemColors.Info;
            this.OutputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.OutputBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputBox.Location = new System.Drawing.Point(3, 202);
            this.OutputBox.Multiline = true;
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OutputBox.Size = new System.Drawing.Size(412, 253);
            this.OutputBox.TabIndex = 0;
            // 
            // DriversTab
            // 
            this.DriversTab.Controls.Add(this.label4);
            this.DriversTab.Controls.Add(this.CyUSBDeviceBox);
            this.DriversTab.Controls.Add(this.HIDDeviceBox);
            this.DriversTab.Controls.Add(this.MSCDeviceBox);
            this.DriversTab.Location = new System.Drawing.Point(4, 22);
            this.DriversTab.Name = "DriversTab";
            this.DriversTab.Size = new System.Drawing.Size(418, 458);
            this.DriversTab.TabIndex = 2;
            this.DriversTab.Text = "Device Class Selection";
            this.DriversTab.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(37, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(207, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Select the USB devices of interest.";
            // 
            // CyUSBDeviceBox
            // 
            this.CyUSBDeviceBox.AutoSize = true;
            this.CyUSBDeviceBox.Checked = true;
            this.CyUSBDeviceBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CyUSBDeviceBox.Location = new System.Drawing.Point(57, 43);
            this.CyUSBDeviceBox.Name = "CyUSBDeviceBox";
            this.CyUSBDeviceBox.Size = new System.Drawing.Size(298, 17);
            this.CyUSBDeviceBox.TabIndex = 10;
            this.CyUSBDeviceBox.Text = "Devices served by the CyUSB3.sys driver (or a derivative)";
            this.CyUSBDeviceBox.UseVisualStyleBackColor = true;
            this.CyUSBDeviceBox.CheckedChanged += new System.EventHandler(this.CyUSBDeviceBox_CheckedChanged);
            // 
            // HIDDeviceBox
            // 
            this.HIDDeviceBox.AutoSize = true;
            this.HIDDeviceBox.Enabled = false;
            this.HIDDeviceBox.Location = new System.Drawing.Point(57, 89);
            this.HIDDeviceBox.Name = "HIDDeviceBox";
            this.HIDDeviceBox.Size = new System.Drawing.Size(175, 17);
            this.HIDDeviceBox.TabIndex = 9;
            this.HIDDeviceBox.Text = "Human Interface Devices (HID)";
            this.HIDDeviceBox.UseVisualStyleBackColor = true;
            this.HIDDeviceBox.CheckedChanged += new System.EventHandler(this.CyUSBDeviceBox_CheckedChanged);
            // 
            // MSCDeviceBox
            // 
            this.MSCDeviceBox.AutoSize = true;
            this.MSCDeviceBox.Enabled = false;
            this.MSCDeviceBox.Location = new System.Drawing.Point(57, 66);
            this.MSCDeviceBox.Name = "MSCDeviceBox";
            this.MSCDeviceBox.Size = new System.Drawing.Size(161, 17);
            this.MSCDeviceBox.TabIndex = 8;
            this.MSCDeviceBox.Text = "Mass Storage Class Devices";
            this.MSCDeviceBox.UseVisualStyleBackColor = true;
            this.MSCDeviceBox.CheckedChanged += new System.EventHandler(this.CyUSBDeviceBox_CheckedChanged);
            // 
            // FOpenDialog
            // 
            this.FOpenDialog.DefaultExt = "iic";
            this.FOpenDialog.Filter = "Intel HEX files (*.hex) | *.hex|Firmware Image files (*.iic) | *.iic";
            this.FOpenDialog.ShowReadOnly = true;
            this.FOpenDialog.Title = "Select file to download . . .";
            // 
            // FSave
            // 
            this.FSave.Title = "Save the script file as";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(674, 555);
            this.Controls.Add(this.Split1);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "USB Control Center";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.Split1.Panel1.ResumeLayout(false);
            this.Split1.Panel2.ResumeLayout(false);
            this.Split1.ResumeLayout(false);
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.TabPages.ResumeLayout(false);
            this.DescrTab.ResumeLayout(false);
            this.DescrTab.PerformLayout();
            this.XferTab.ResumeLayout(false);
            this.XferTab.PerformLayout();
            this.DriversTab.ResumeLayout(false);
            this.DriversTab.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton load_button;
        private System.Windows.Forms.ToolStripButton play_button;
        private System.Windows.Forms.ToolStripButton Abort_endpoint;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.SplitContainer Split1;
        private System.Windows.Forms.TreeView DeviceTreeView;
        private System.Windows.Forms.TabControl TabPages;
        private System.Windows.Forms.TabPage DescrTab;
        private System.Windows.Forms.TabPage XferTab;
        private System.Windows.Forms.TextBox DescText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button FileXferBtn;
        private System.Windows.Forms.Button DataXferBtn;
        private System.Windows.Forms.TextBox NumBytesBox;
        private System.Windows.Forms.TextBox XferTextBox;
        private System.Windows.Forms.ComboBox TargetBox;
        private System.Windows.Forms.ComboBox ReqTypeBox;
        private System.Windows.Forms.ComboBox DirectionBox;
        private System.Windows.Forms.TextBox wValueBox;
        private System.Windows.Forms.TextBox ReqCodeBox;
        private System.Windows.Forms.TextBox wIndexBox;
        private System.Windows.Forms.Label ReqCodeLabel;
        private System.Windows.Forms.Label wIndexLabel;
        private System.Windows.Forms.Label wValueLabel;
        private System.Windows.Forms.Label TargetLabel;
        private System.Windows.Forms.Label ReqTypeLabel;
        private System.Windows.Forms.Label DirectionLabel;
        private System.Windows.Forms.TextBox OutputBox;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem programFX2ToolStripMenuItem;
        public System.Windows.Forms.OpenFileDialog FOpenDialog;
        public System.Windows.Forms.ToolStripStatusLabel StatLabel;
        private System.Windows.Forms.TabPage DriversTab;
        private System.Windows.Forms.CheckBox CyUSBDeviceBox;
        private System.Windows.Forms.CheckBox HIDDeviceBox;
        private System.Windows.Forms.CheckBox MSCDeviceBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox XferDataBox;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.ToolStripButton Reset_device;
        private System.Windows.Forms.ToolStripButton Reconnect_device;
        private System.Windows.Forms.ToolStripButton Reset_endpoint;
        private System.Windows.Forms.SaveFileDialog FSave;
        private System.Windows.Forms.ToolStripButton Reset_Pipe;
        private System.Windows.Forms.ToolStripButton Abort_Pipe;
        private System.Windows.Forms.ToolStripButton URB_Stat;
        private System.Windows.Forms.ToolStripButton Create_script;
        private System.Windows.Forms.GroupBox gb;
        private System.Windows.Forms.Label Addr;
        private System.Windows.Forms.Label Cpu;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.ToolStripButton Script_parameters;
        private System.Windows.Forms.ToolStripMenuItem UsersGuide;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.CheckBox IsPacket;
        private System.Windows.Forms.ToolStripButton Record;
        private System.Windows.Forms.ToolStripButton Stop;
        private System.Windows.Forms.ToolStripButton Pause;
        private System.Windows.Forms.ToolStripMenuItem fX2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ProgramFX2RamMenuItm;
        private System.Windows.Forms.ToolStripMenuItem ProgramFX2smallEEPROMMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ProgramFX264kBEEPROMMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ProgramFX2haltMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ProgramFX2runMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fX3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ProgramFX3Ram;
        private System.Windows.Forms.ToolStripMenuItem ProgramFX3I2CEEPROM;
        private System.Windows.Forms.ToolStripMenuItem sPIFLASHToolStripMenuItem;


    }
}

