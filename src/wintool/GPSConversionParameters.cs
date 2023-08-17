/*

	GPSConversionParameters.cs - GPS to MSFS Video Conversion Parameter Dialog
    Copyright (C) 2003-2004, Marty Ross


	Present dialog to allow user to select all available options for controlling
	translation of GPS track log data to Microsoft Flight Simulator Video.

	Return these data to the calling program.

*/

using System ;
using System.Drawing ;
using System.Globalization ;
using System.Collections ;
using System.ComponentModel ;
using System.Windows.Forms ;

namespace fsrwintool
{

	public class GPSConversionParameters : System.Windows.Forms.Form
	{

		public GPSConversionParameters()
		{
			Construct() ;
		}

		public GPSConversionParameters(string Name, string Description)
		{
			Construct() ;
			tbName.Text= Name ;
			tbDescription.Text= Description ;
		}

		public string VideoName { get { return(tbName.Text) ; } } 
		public string VideoDescription { get { return(tbDescription.Text) ; } }

		public bool VideoReduceJitter { get { return(cbReduceJitter.Checked) ; } }

		public double VideoMinimumAltitude
		{
			get {
				if (!cbMinimumAlt.Checked) {
					return(double.MinValue) ;
				}
				return(double.Parse(tbMinimumAlt.Text)) ;
			}
		}

		public double VideoStartSecond
		{
			get {
				if (!cbStartSec.Checked) {
					return(0) ;
				}
				return(double.Parse(tbStartSec.Text)) ;
			}
		} 

		public double VideoTotalSeconds
		{
			get {
				if (!cbTotalSec.Checked) {
					return(double.MaxValue) ;
				}
				return(double.Parse(tbTotalSec.Text)) ;
			}
		} 

		public DateTime VideoForceDateTime
		{
			get {
				if (!cbResequenceTo.Checked) {
					return(DateTime.MaxValue) ;
				}
				DateTime fd= new DateTime(
					dtForceDate.Value.Date.Ticks
				      + dtForceTime.Value.TimeOfDay.Ticks
				      - (long) ((VideoStartSecond * 10000000.0) + 0.5)
				) ;
				return(fd) ;
			}
		}

		public double VideoTicksPerSecond
		{
			get {
				return((double) ntbTicksPerSec.Value) ;	
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GPSConversionParameters));
			this.TabControl = new System.Windows.Forms.TabControl();
			this.tpNameDesc = new System.Windows.Forms.TabPage();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.tbName = new System.Windows.Forms.TextBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.tpTimeParms = new System.Windows.Forms.TabPage();
			this.ntbTicksPerSec = new System.Windows.Forms.NumericUpDown();
			this.dtForceTime = new System.Windows.Forms.DateTimePicker();
			this.lblTicksPerSec = new System.Windows.Forms.Label();
			this.tbStartSec = new System.Windows.Forms.TextBox();
			this.cbResequenceTo = new System.Windows.Forms.CheckBox();
			this.dtForceDate = new System.Windows.Forms.DateTimePicker();
			this.cbStartSec = new System.Windows.Forms.CheckBox();
			this.cbTotalSec = new System.Windows.Forms.CheckBox();
			this.tbTotalSec = new System.Windows.Forms.TextBox();
			this.tpAltitudes = new System.Windows.Forms.TabPage();
			this.lblMinimumAltSuffix = new System.Windows.Forms.Label();
			this.tbMinimumAlt = new System.Windows.Forms.TextBox();
			this.cbMinimumAlt = new System.Windows.Forms.CheckBox();
			this.tpOptions = new System.Windows.Forms.TabPage();
			this.cbReduceJitter = new System.Windows.Forms.CheckBox();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.hpProvider = new System.Windows.Forms.HelpProvider();
			this.TabControl.SuspendLayout();
			this.tpNameDesc.SuspendLayout();
			this.tpTimeParms.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ntbTicksPerSec)).BeginInit();
			this.tpAltitudes.SuspendLayout();
			this.tpOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// TabControl
			// 
			this.TabControl.AccessibleName = "";
			this.TabControl.Controls.AddRange(new System.Windows.Forms.Control[] {
												     this.tpNameDesc,
												     this.tpTimeParms,
												     this.tpAltitudes,
												     this.tpOptions});
			this.TabControl.Location = new System.Drawing.Point(8, 8);
			this.TabControl.Name = "TabControl";
			this.TabControl.SelectedIndex = 0;
			this.TabControl.Size = new System.Drawing.Size(408, 216);
			this.TabControl.TabIndex = 0;
			// 
			// tpNameDesc
			// 
			this.tpNameDesc.Controls.AddRange(new System.Windows.Forms.Control[] {
												     this.tbDescription,
												     this.tbName,
												     this.lblDescription,
												     this.lblName});
			this.tpNameDesc.Location = new System.Drawing.Point(4, 22);
			this.tpNameDesc.Name = "tpNameDesc";
			this.tpNameDesc.Size = new System.Drawing.Size(400, 190);
			this.tpNameDesc.TabIndex = 0;
			// 
			// tbDescription
			// 
			this.tbDescription.CausesValidation = false;
			this.tbDescription.Location = new System.Drawing.Point(16, 88);
			this.tbDescription.Multiline = true;
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDescription.Size = new System.Drawing.Size(368, 88);
			this.tbDescription.TabIndex = 3;
			this.tbDescription.Text = "";
			// 
			// tbName
			// 
			this.tbName.Location = new System.Drawing.Point(16, 32);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(368, 20);
			this.tbName.TabIndex = 2;
			this.tbName.Text = "";
			this.tbName.TextChanged += new System.EventHandler(this.textChangedEventHandler);
			// 
			// lblDescription
			// 
			this.lblDescription.Location = new System.Drawing.Point(16, 72);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(112, 16);
			this.lblDescription.TabIndex = 1;
			// 
			// lblName
			// 
			this.lblName.Location = new System.Drawing.Point(16, 16);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(112, 16);
			this.lblName.TabIndex = 0;
			// 
			// tpTimeParms
			// 
			this.tpTimeParms.Controls.AddRange(new System.Windows.Forms.Control[] {
												      this.ntbTicksPerSec,
												      this.dtForceTime,
												      this.lblTicksPerSec,
												      this.tbStartSec,
												      this.cbResequenceTo,
												      this.dtForceDate,
												      this.cbStartSec,
												      this.cbTotalSec,
												      this.tbTotalSec});
			this.tpTimeParms.Location = new System.Drawing.Point(4, 22);
			this.tpTimeParms.Name = "tpTimeParms";
			this.tpTimeParms.Size = new System.Drawing.Size(400, 190);
			this.tpTimeParms.TabIndex = 1;
			// 
			// ntbTicksPerSec
			// 
			this.ntbTicksPerSec.Location = new System.Drawing.Point(152, 144);
			this.ntbTicksPerSec.Maximum = new System.Decimal(new int[] {
											   256,
											   0,
											   0,
											   0});
			this.ntbTicksPerSec.Minimum = new System.Decimal(new int[] {
											   1,
											   0,
											   0,
											   0});
			this.ntbTicksPerSec.Name = "ntbTicksPerSec";
			this.ntbTicksPerSec.Size = new System.Drawing.Size(56, 20);
			this.ntbTicksPerSec.TabIndex = 32;
			this.ntbTicksPerSec.Value = new System.Decimal(new int[] {
											 18,
											 0,
											 0,
											 0});
			// 
			// dtForceTime
			// 
			this.dtForceTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dtForceTime.Location = new System.Drawing.Point(272, 104);
			this.dtForceTime.Name = "dtForceTime";
			this.dtForceTime.ShowUpDown = true;
			this.dtForceTime.Size = new System.Drawing.Size(104, 20);
			this.dtForceTime.TabIndex = 30;
			this.dtForceTime.Value = new System.DateTime(2003, 12, 20, 10, 17, 45, 791);
			// 
			// lblTicksPerSec
			// 
			this.lblTicksPerSec.Location = new System.Drawing.Point(24, 144);
			this.lblTicksPerSec.Name = "lblTicksPerSec";
			this.lblTicksPerSec.Size = new System.Drawing.Size(120, 27);
			this.lblTicksPerSec.TabIndex = 0;
			this.lblTicksPerSec.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbStartSec
			// 
			this.tbStartSec.Location = new System.Drawing.Point(152, 24);
			this.tbStartSec.Name = "tbStartSec";
			this.tbStartSec.Size = new System.Drawing.Size(72, 20);
			this.tbStartSec.TabIndex = 28;
			this.tbStartSec.Text = "";
			this.tbStartSec.TextChanged += new System.EventHandler(this.textChangedEventHandler);
			// 
			// cbResequenceTo
			// 
			this.cbResequenceTo.CausesValidation = false;
			this.cbResequenceTo.Location = new System.Drawing.Point(24, 104);
			this.cbResequenceTo.Name = "cbResequenceTo";
			this.cbResequenceTo.Size = new System.Drawing.Size(120, 27);
			this.cbResequenceTo.TabIndex = 24;
			this.cbResequenceTo.CheckedChanged += new System.EventHandler(this.checkedChangedEventHandler);
			// 
			// dtForceDate
			// 
			this.dtForceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtForceDate.Location = new System.Drawing.Point(152, 104);
			this.dtForceDate.Name = "dtForceDate";
			this.dtForceDate.Size = new System.Drawing.Size(112, 20);
			this.dtForceDate.TabIndex = 27;
			this.dtForceDate.Value = new System.DateTime(2003, 12, 20, 10, 17, 45, 821);
			// 
			// cbStartSec
			// 
			this.cbStartSec.CausesValidation = false;
			this.cbStartSec.Location = new System.Drawing.Point(24, 24);
			this.cbStartSec.Name = "cbStartSec";
			this.cbStartSec.Size = new System.Drawing.Size(120, 27);
			this.cbStartSec.TabIndex = 25;
			this.cbStartSec.CheckedChanged += new System.EventHandler(this.checkedChangedEventHandler);
			// 
			// cbTotalSec
			// 
			this.cbTotalSec.CausesValidation = false;
			this.cbTotalSec.Location = new System.Drawing.Point(24, 64);
			this.cbTotalSec.Name = "cbTotalSec";
			this.cbTotalSec.Size = new System.Drawing.Size(120, 27);
			this.cbTotalSec.TabIndex = 26;
			this.cbTotalSec.CheckedChanged += new System.EventHandler(this.checkedChangedEventHandler);
			// 
			// tbTotalSec
			// 
			this.tbTotalSec.Location = new System.Drawing.Point(152, 64);
			this.tbTotalSec.Name = "tbTotalSec";
			this.tbTotalSec.Size = new System.Drawing.Size(56, 20);
			this.tbTotalSec.TabIndex = 29;
			this.tbTotalSec.Text = "";
			this.tbTotalSec.TextChanged += new System.EventHandler(this.textChangedEventHandler);
			// 
			// tpAltitudes
			// 
			this.tpAltitudes.Controls.AddRange(new System.Windows.Forms.Control[] {
												      this.lblMinimumAltSuffix,
												      this.tbMinimumAlt,
												      this.cbMinimumAlt});
			this.tpAltitudes.Location = new System.Drawing.Point(4, 22);
			this.tpAltitudes.Name = "tpAltitudes";
			this.tpAltitudes.Size = new System.Drawing.Size(400, 190);
			this.tpAltitudes.TabIndex = 2;
			// 
			// lblMinimumAltSuffix
			// 
			this.lblMinimumAltSuffix.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblMinimumAltSuffix.Location = new System.Drawing.Point(240, 40);
			this.lblMinimumAltSuffix.Name = "lblMinimumAltSuffix";
			this.lblMinimumAltSuffix.Size = new System.Drawing.Size(88, 24);
			this.lblMinimumAltSuffix.TabIndex = 2;
			this.lblMinimumAltSuffix.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbMinimumAlt
			// 
			this.tbMinimumAlt.Location = new System.Drawing.Point(152, 40);
			this.tbMinimumAlt.Name = "tbMinimumAlt";
			this.tbMinimumAlt.Size = new System.Drawing.Size(80, 20);
			this.tbMinimumAlt.TabIndex = 1;
			this.tbMinimumAlt.Text = "";
			this.tbMinimumAlt.TextChanged += new System.EventHandler(this.textChangedEventHandler);
			// 
			// cbMinimumAlt
			// 
			this.cbMinimumAlt.CausesValidation = false;
			this.cbMinimumAlt.Location = new System.Drawing.Point(24, 40);
			this.cbMinimumAlt.Name = "cbMinimumAlt";
			this.cbMinimumAlt.Size = new System.Drawing.Size(120, 24);
			this.cbMinimumAlt.TabIndex = 0;
			this.cbMinimumAlt.CheckedChanged += new System.EventHandler(this.checkedChangedEventHandler);
			// 
			// tpOptions
			// 
			this.tpOptions.Controls.AddRange(new System.Windows.Forms.Control[] {
												    this.cbReduceJitter});
			this.tpOptions.Location = new System.Drawing.Point(4, 22);
			this.tpOptions.Name = "tpOptions";
			this.tpOptions.Size = new System.Drawing.Size(400, 190);
			this.tpOptions.TabIndex = 3;
			// 
			// cbReduceJitter
			// 
			this.cbReduceJitter.CausesValidation = false;
			this.cbReduceJitter.Checked = true;
			this.cbReduceJitter.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbReduceJitter.Location = new System.Drawing.Point(32, 40);
			this.cbReduceJitter.Name = "cbReduceJitter";
			this.cbReduceJitter.Size = new System.Drawing.Size(144, 24);
			this.cbReduceJitter.TabIndex = 0;
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 230);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(530, 22);
			this.statusBar.SizingGrip = false;
			this.statusBar.TabIndex = 1;
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(440, 32);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 32);
			this.btnOk.TabIndex = 2;
			// 
			// btnCancel
			// 
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(440, 80);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 32);
			this.btnCancel.TabIndex = 3;
			// 
			// GPSConversionParameters
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(530, 252);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
											  this.btnCancel,
											  this.btnOk,
											  this.statusBar,
											  this.TabControl});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(536, 280);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(536, 280);
			this.Name = "GPSConversionParameters";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Load += new System.EventHandler(this.GPSConversionParameters_Load);
			this.TabControl.ResumeLayout(false);
			this.tpNameDesc.ResumeLayout(false);
			this.tpTimeParms.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ntbTicksPerSec)).EndInit();
			this.tpAltitudes.ResumeLayout(false);
			this.tpOptions.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		//
		//
		//

		private void GPSConversionParameters_Load(object sender, System.EventArgs e)
		{
			UpdateDialog(true) ;
		}

		private void checkedChangedEventHandler(object sender, System.EventArgs e)
		{
			UpdateDialog(true) ;
		}

		private void textChangedEventHandler(object sender, System.EventArgs e)
		{
			UpdateDialog(false) ;
		}

		private void tbNonNullText_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			TextBox tb= (TextBox) sender ;
			if (tb.Enabled && (tb.Text.Length == 0)) {
				e.Cancel= true ;
			}
		}

		private void ntbTicksPerSec_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// this is apparently necessary and sufficient to force
			// the value to remain within the required range.
			decimal d= ntbTicksPerSec.Value ;
		}

		private void tcValidating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!UpdateDialog(false)) {
				e.Cancel= true ;
			}
		}

		private void tbNumericEdit_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar)) {
				TextBox tb= (TextBox) sender ;
				string newtext= tb.Text + e.KeyChar ;
				if (!IsValidNumber(newtext)) {
					e.Handled= true ;
				}
			}
		}


		//
		//
		//

		void Construct()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// fields to validate:
			ntbTicksPerSec.Validating+= new CancelEventHandler(ntbTicksPerSec_Validating) ;
			tbMinimumAlt.Validating+= new CancelEventHandler(tbNonNullText_Validating) ;
			tbStartSec.Validating+= new CancelEventHandler(tbNonNullText_Validating) ;
			tbTotalSec.Validating+= new CancelEventHandler(tbNonNullText_Validating) ;
			tbName.Validating+= new CancelEventHandler(tbNonNullText_Validating) ;
			TabControl.Validating+= new CancelEventHandler(tcValidating) ;			

			// fields to restrict keypresses:
			tbStartSec.KeyPress+= new KeyPressEventHandler(tbNumericEdit_KeyPress) ;
			tbTotalSec.KeyPress+= new KeyPressEventHandler(tbNumericEdit_KeyPress) ;
			tbMinimumAlt.KeyPress+= new KeyPressEventHandler(tbNumericEdit_KeyPress) ;


			//
			//	Load text field values...
			//

			Text= FSRWinMsgs.Load("TitleGPSConversionParmDialog") ;

			tpNameDesc.Text= FSRWinMsgs.Load("TabInternalName") ;
			tpAltitudes.Text= FSRWinMsgs.Load("TabAltitudes") ;
			tpTimeParms.Text= FSRWinMsgs.Load("TabTimeParms") ;
			tpOptions.Text= FSRWinMsgs.Load("TabOptions") ;

			lblDescription.Text= FSRWinMsgs.Load("LabelInternalDescription") ;
			lblName.Text= FSRWinMsgs.Load("LabelInternalName") ;
			lblTicksPerSec.Text= FSRWinMsgs.Load("LabelTicksPerSec") ;
			lblMinimumAltSuffix.Text= FSRWinMsgs.Load("LabelMinimumAltSuffix") ;

			cbResequenceTo.Text= FSRWinMsgs.Load("CheckResequenceTo") ;
			cbStartSec.Text= FSRWinMsgs.Load("CheckStartSec") ;
			cbTotalSec.Text= FSRWinMsgs.Load("CheckTotalSec") ;
			cbMinimumAlt.Text= FSRWinMsgs.Load("CheckMinimumAlt") ;
			cbReduceJitter.Text= FSRWinMsgs.Load("CheckReduceJitter") ;

			btnOk.Text= FSRWinMsgs.Load("CmdOk") ;
			btnCancel.Text= FSRWinMsgs.Load("CmdCancel") ;

			dtForceDate.Value= DateTime.Now ;
			dtForceTime.Value= dtForceDate.Value ;



			//
			//	Setup help for controls
			//

#if true
			LoadControlHelp(this, FSRWinMsgs.Load("HelpNone")) ;
#else

			string hs ;

			hs= FSRWinMsgs.Load("HelpNone") ;
			foreach (Control c in this.Controls) {
				hpProvider.SetHelpString(c, hs) ;
			}

			hs= FSRWinMsgs.Load("HelpTextName") ;
			hpProvider.SetHelpString(tbName, hs) ;

			hs= FSRWinMsgs.Load("HelpDescription") ;
			hpProvider.SetHelpString(tbDescription, hs) ;

			hs= FSRWinMsgs.Load("HelpStartSec") ;
			hpProvider.SetHelpString(cbStartSec, hs) ;
			hpProvider.SetHelpString(tbStartSec, hs) ;

			hs= FSRWinMsgs.Load("HelpTotalSec") ;
			hpProvider.SetHelpString(cbTotalSec, hs) ;
			hpProvider.SetHelpString(tbTotalSec, hs) ;

			hs= FSRWinMsgs.Load("HelpResequenceTo") ;
			hpProvider.SetHelpString(cbResequenceTo, hs) ;
			hpProvider.SetHelpString(dtForceDate, hs) ;
			hpProvider.SetHelpString(dtForceTime, hs) ;

			hs= FSRWinMsgs.Load("HelpTicksPerSec") ;
			hpProvider.SetHelpString(ntbTicksPerSec, hs) ;

			hs= FSRWinMsgs.Load("HelpMinimumAlt") ;
			hpProvider.SetHelpString(cbMinimumAlt, hs) ;
			hpProvider.SetHelpString(tbMinimumAlt, hs) ;

			hs= FSRWinMsgs.Load("HelpReduceJitter") ;
			hpProvider.SetHelpString(cbReduceJitter, hs) ;
#endif

		}

		bool UpdateDialog(bool bChangeFocus)
		{
			Control ctl ;
			string msg= NeedInfo(out ctl) ;

			WriteStatus((msg != null)? msg: FSRWinMsgs.Load("StatusReady")) ;

			dtForceDate.Enabled= cbResequenceTo.Checked ;
			dtForceTime.Enabled= cbResequenceTo.Checked ;
			tbStartSec.Enabled= cbStartSec.Checked ;
			tbTotalSec.Enabled= cbTotalSec.Checked ;
			tbMinimumAlt.Enabled= cbMinimumAlt.Checked ;
			btnOk.Enabled= (msg == null) ;

			if (ctl != null) {
				if (bChangeFocus) {
					ActiveControl= ctl ;
				}
			}

			return(msg == null) ;	// returns true iff dialog is complete
		}

		void WriteStatus(string msg)
		{
			statusBar.Text= msg ;
		}

		string NeedInfo(out Control ctl)
		{
			ctl= null ;
			if (tbName.Text.Length == 0) {
				ctl= tbName ;
				return(FSRWinMsgs.Load("StatusEnterVideoDescription")) ;
			}
			if (cbStartSec.Checked) {
				if (tbStartSec.Text.Length == 0) {
					ctl= tbStartSec ;
					return(FSRWinMsgs.Load("StatusEnterStartSecond")) ;
				}
			}
			if (cbTotalSec.Checked) {
				if (tbTotalSec.Text.Length == 0) {
					ctl= tbTotalSec ;
					return(FSRWinMsgs.Load("StatusEnterTotalSeconds")) ;
				}
			}
			if (cbMinimumAlt.Checked) {
				if (tbMinimumAlt.Text.Length == 0) {
					ctl= tbMinimumAlt ;
					return(FSRWinMsgs.Load("StatusEnterMinimumAltitude")) ;
				}
			}
			return(null) ;
		}

		bool IsValidNumber(String textnum)
		{
			double dnum ;
			if (
				double.TryParse(
					textnum,
					(
						NumberStyles.AllowDecimalPoint
					      |	NumberStyles.AllowTrailingWhite
					),
					NumberFormatInfo.CurrentInfo,
					out dnum
				)
			) {
				return(true) ;
			}
			return(false) ;
		}

		void LoadControlHelp(Control cparent, string hsNone)
		{
			foreach (Control c in cparent.Controls) {
				string name= "CPHelpCtl_" + c.Name ; 
				string hs= FSRWinMsgs.Load(name) ;
				hpProvider.SetHelpString(c, (hs == null)? hsNone: hs) ;
				LoadControlHelp(c, hsNone) ;
			}
		}

		//
		//
		//

		private System.Windows.Forms.TabControl TabControl;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.TabPage tpNameDesc;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.NumericUpDown ntbTicksPerSec;
		private System.Windows.Forms.DateTimePicker dtForceTime;
		private System.Windows.Forms.TextBox tbStartSec;
		private System.Windows.Forms.CheckBox cbResequenceTo;
		private System.Windows.Forms.DateTimePicker dtForceDate;
		private System.Windows.Forms.CheckBox cbStartSec;
		private System.Windows.Forms.CheckBox cbTotalSec;
		private System.Windows.Forms.TextBox tbTotalSec;
		private System.Windows.Forms.Label lblTicksPerSec;
		private System.Windows.Forms.TabPage tpTimeParms;
		private System.Windows.Forms.TabPage tpAltitudes;
		private System.Windows.Forms.CheckBox cbMinimumAlt;
		private System.Windows.Forms.TextBox tbMinimumAlt;
		private System.Windows.Forms.Label lblMinimumAltSuffix;
		private System.Windows.Forms.TabPage tpOptions;
		private System.Windows.Forms.CheckBox cbReduceJitter;
		private System.Windows.Forms.HelpProvider hpProvider;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

	}
}
