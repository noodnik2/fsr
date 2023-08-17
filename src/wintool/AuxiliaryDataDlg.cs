using System ;
using System.Drawing ;
using System.Globalization ;
using System.Collections ;
using System.ComponentModel ;
using System.Windows.Forms ;

namespace fsrwintool
{
	/// <summary>
	/// Summary description for AuxiliaryDataDlg.
	/// </summary>
	public class AuxiliaryDataDlg : System.Windows.Forms.Form
	{

		public AuxiliaryDataDlg()
		{
			Construct() ;
		}

		public AuxiliaryDataDlg(string Name, string Description)
		{
			Construct() ;
			tbName.Text= Name ;
			tbDescription.Text= Description ;
		}

		public string VideoName { get { return(tbName.Text) ; } } 
		public string VideoDescription { get { return(tbDescription.Text) ; } }

		public double VideoStartSecond {
			get {
				if (!cbStartSec.Checked) {
					return(0) ;
				}
				return(double.Parse(tbStartSec.Text)) ;
			}
		} 

		public double VideoTotalSeconds {
			get {
				if (!cbTotalSec.Checked) {
					return(double.MaxValue) ;
				}
				return(double.Parse(tbTotalSec.Text)) ;
			}
		} 

		public DateTime VideoForceDateTime {
			get {
				if (!cbForcedDateTime.Checked) {
					return(DateTime.MaxValue) ;
				}
				DateTime fd= new DateTime(
					dtForceDate.Value.Date.Ticks
				      + dtForceTime.Value.TimeOfDay.Ticks
				) ;
				return(fd) ;
			}
		}

		public double VideoTicksPerSecond {
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AuxiliaryDataDlg));
			this.lblVideoName = new System.Windows.Forms.Label();
			this.tbName = new System.Windows.Forms.TextBox();
			this.lblVideoDescription = new System.Windows.Forms.Label();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.btnAdvanced = new System.Windows.Forms.Button();
			this.gbVideoDesc = new System.Windows.Forms.GroupBox();
			this.gbAdvParms = new System.Windows.Forms.GroupBox();
			this.ntbTicksPerSec = new System.Windows.Forms.NumericUpDown();
			this.cbTicksPerSec = new System.Windows.Forms.CheckBox();
			this.dtForceTime = new System.Windows.Forms.DateTimePicker();
			this.tbStartSec = new System.Windows.Forms.TextBox();
			this.cbForcedDateTime = new System.Windows.Forms.CheckBox();
			this.dtForceDate = new System.Windows.Forms.DateTimePicker();
			this.cbStartSec = new System.Windows.Forms.CheckBox();
			this.cbTotalSec = new System.Windows.Forms.CheckBox();
			this.tbTotalSec = new System.Windows.Forms.TextBox();
			this.gbAdvParms.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ntbTicksPerSec)).BeginInit();
			this.SuspendLayout();
			// 
			// lblVideoName
			// 
			this.lblVideoName.Location = new System.Drawing.Point(40, 40);
			this.lblVideoName.Name = "lblVideoName";
			this.lblVideoName.Size = new System.Drawing.Size(100, 24);
			this.lblVideoName.TabIndex = 0;
			this.lblVideoName.Text = "Name:";
			// 
			// tbName
			// 
			this.tbName.Location = new System.Drawing.Point(40, 64);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(384, 20);
			this.tbName.TabIndex = 1;
			this.tbName.Text = "";
			this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
			// 
			// lblVideoDescription
			// 
			this.lblVideoDescription.Location = new System.Drawing.Point(40, 96);
			this.lblVideoDescription.Name = "lblVideoDescription";
			this.lblVideoDescription.Size = new System.Drawing.Size(176, 23);
			this.lblVideoDescription.TabIndex = 2;
			this.lblVideoDescription.Text = "Description:";
			// 
			// tbDescription
			// 
			this.tbDescription.AutoSize = false;
			this.tbDescription.Location = new System.Drawing.Point(40, 120);
			this.tbDescription.Multiline = true;
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDescription.Size = new System.Drawing.Size(384, 88);
			this.tbDescription.TabIndex = 3;
			this.tbDescription.Text = "";
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Enabled = false;
			this.btnOk.Location = new System.Drawing.Point(456, 24);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 32);
			this.btnOk.TabIndex = 5;
			this.btnOk.Text = "&OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(456, 72);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 32);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "&Cancel";
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 412);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(552, 22);
			this.statusBar.TabIndex = 7;
			// 
			// btnAdvanced
			// 
			this.btnAdvanced.Location = new System.Drawing.Point(456, 200);
			this.btnAdvanced.Name = "btnAdvanced";
			this.btnAdvanced.TabIndex = 8;
			this.btnAdvanced.Text = "< Advanced";
			this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
			// 
			// gbVideoDesc
			// 
			this.gbVideoDesc.Location = new System.Drawing.Point(24, 16);
			this.gbVideoDesc.Name = "gbVideoDesc";
			this.gbVideoDesc.Size = new System.Drawing.Size(416, 208);
			this.gbVideoDesc.TabIndex = 4;
			this.gbVideoDesc.TabStop = false;
			this.gbVideoDesc.Text = "Video File Information:";
			// 
			// gbAdvParms
			// 
			this.gbAdvParms.Controls.AddRange(new System.Windows.Forms.Control[] {
												     this.ntbTicksPerSec,
												     this.cbTicksPerSec,
												     this.dtForceTime,
												     this.tbStartSec,
												     this.cbForcedDateTime,
												     this.dtForceDate,
												     this.cbStartSec,
												     this.cbTotalSec,
												     this.tbTotalSec});
			this.gbAdvParms.Location = new System.Drawing.Point(24, 240);
			this.gbAdvParms.Name = "gbAdvParms";
			this.gbAdvParms.Size = new System.Drawing.Size(416, 160);
			this.gbAdvParms.TabIndex = 14;
			this.gbAdvParms.TabStop = false;
			this.gbAdvParms.Text = "Advanced Parameters:";
			// 
			// ntbTicksPerSec
			// 
			this.ntbTicksPerSec.Location = new System.Drawing.Point(192, 120);
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
			this.ntbTicksPerSec.TabIndex = 23;
			this.ntbTicksPerSec.Value = new System.Decimal(new int[] {
											 18,
											 0,
											 0,
											 0});
			this.ntbTicksPerSec.ValueChanged += new System.EventHandler(this.ntbTicksPerSec_ValueChanged);
			// 
			// cbTicksPerSec
			// 
			this.cbTicksPerSec.Location = new System.Drawing.Point(56, 120);
			this.cbTicksPerSec.Name = "cbTicksPerSec";
			this.cbTicksPerSec.Size = new System.Drawing.Size(120, 27);
			this.cbTicksPerSec.TabIndex = 21;
			this.cbTicksPerSec.Text = "Ticks Per Second:";
			this.cbTicksPerSec.CheckedChanged += new System.EventHandler(this.cbTicksPerSec_CheckedChanged);
			// 
			// dtForceTime
			// 
			this.dtForceTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dtForceTime.Location = new System.Drawing.Point(288, 24);
			this.dtForceTime.Name = "dtForceTime";
			this.dtForceTime.ShowUpDown = true;
			this.dtForceTime.Size = new System.Drawing.Size(104, 20);
			this.dtForceTime.TabIndex = 20;
			this.dtForceTime.Value = new System.DateTime(2003, 12, 20, 10, 17, 45, 791);
			// 
			// tbStartSec
			// 
			this.tbStartSec.Location = new System.Drawing.Point(192, 56);
			this.tbStartSec.Name = "tbStartSec";
			this.tbStartSec.Size = new System.Drawing.Size(72, 20);
			this.tbStartSec.TabIndex = 18;
			this.tbStartSec.Text = "";
			this.tbStartSec.TextChanged += new System.EventHandler(this.tbStartSec_TextChanged);
			// 
			// cbForcedDateTime
			// 
			this.cbForcedDateTime.Location = new System.Drawing.Point(56, 24);
			this.cbForcedDateTime.Name = "cbForcedDateTime";
			this.cbForcedDateTime.Size = new System.Drawing.Size(120, 27);
			this.cbForcedDateTime.TabIndex = 14;
			this.cbForcedDateTime.Text = "Force Time:";
			this.cbForcedDateTime.CheckedChanged += new System.EventHandler(this.cbForcedDateTime_CheckedChanged);
			// 
			// dtForceDate
			// 
			this.dtForceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtForceDate.Location = new System.Drawing.Point(192, 24);
			this.dtForceDate.Name = "dtForceDate";
			this.dtForceDate.Size = new System.Drawing.Size(120, 20);
			this.dtForceDate.TabIndex = 17;
			this.dtForceDate.Value = new System.DateTime(2003, 12, 20, 10, 17, 45, 821);
			// 
			// cbStartSec
			// 
			this.cbStartSec.Location = new System.Drawing.Point(56, 56);
			this.cbStartSec.Name = "cbStartSec";
			this.cbStartSec.Size = new System.Drawing.Size(120, 27);
			this.cbStartSec.TabIndex = 15;
			this.cbStartSec.Text = "Starting Second:";
			this.cbStartSec.CheckedChanged += new System.EventHandler(this.cbStartSec_CheckedChanged);
			// 
			// cbTotalSec
			// 
			this.cbTotalSec.Location = new System.Drawing.Point(56, 88);
			this.cbTotalSec.Name = "cbTotalSec";
			this.cbTotalSec.Size = new System.Drawing.Size(120, 27);
			this.cbTotalSec.TabIndex = 16;
			this.cbTotalSec.Text = "Total Seconds:";
			this.cbTotalSec.CheckedChanged += new System.EventHandler(this.cbTotalSec_CheckedChanged);
			// 
			// tbTotalSec
			// 
			this.tbTotalSec.Location = new System.Drawing.Point(192, 88);
			this.tbTotalSec.Name = "tbTotalSec";
			this.tbTotalSec.Size = new System.Drawing.Size(56, 20);
			this.tbTotalSec.TabIndex = 19;
			this.tbTotalSec.Text = "";
			this.tbTotalSec.TextChanged += new System.EventHandler(this.tbTotalSec_TextChanged);
			// 
			// AuxiliaryDataDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(552, 434);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
											  this.btnAdvanced,
											  this.statusBar,
											  this.btnCancel,
											  this.btnOk,
											  this.tbDescription,
											  this.lblVideoDescription,
											  this.tbName,
											  this.lblVideoName,
											  this.gbVideoDesc,
											  this.gbAdvParms});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AuxiliaryDataDlg";
			this.Text = "FSR Auxiliary Data";
			this.Load += new System.EventHandler(this.AuxiliaryDataDlg_Load);
			this.gbAdvParms.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ntbTicksPerSec)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		private void AuxiliaryDataDlg_Load(object sender, System.EventArgs e)
		{
			UpdateDialog(true) ;
		}

		private void cbForcedDateTime_CheckedChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(true) ;
		}

		private void cbStartSec_CheckedChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(true) ;
		}

		private void cbTotalSec_CheckedChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(true) ;
		}

		private void cbTicksPerSec_CheckedChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(true) ;
		}

		private void tbStartSec_TextChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(false) ;
		}

		private void tbTotalSec_TextChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(false) ;
		}

		private void tbName_TextChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(false) ;
		}

		private void ntbTicksPerSec_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// this is apparently necessary and sufficient to force
			// the value to remain within the required range.
			decimal d= ntbTicksPerSec.Value ;
		}

		private void ntbTicksPerSec_ValueChanged(object sender, System.EventArgs e)
		{
			UpdateDialog(false) ;
		}

		private void btnAdvanced_Click(object sender, System.EventArgs e)
		{
			m_bAdvancedMode= !m_bAdvancedMode ;
			UpdateDialog(false) ;
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

			ntbTicksPerSec.Validating+= new CancelEventHandler(ntbTicksPerSec_Validating) ;
			tbStartSec.KeyPress+= new KeyPressEventHandler(tbNumericEdit_KeyPress) ;
			tbTotalSec.KeyPress+= new KeyPressEventHandler(tbNumericEdit_KeyPress) ;

			dtForceDate.Value= DateTime.Now ;
			dtForceTime.Value= dtForceDate.Value ;

			m_iNormalHeight= (
				gbVideoDesc.Location.Y 
			      + gbVideoDesc.Size.Height
			      + statusBar.Height
			      + 40
			) ;
			m_iAdvancedHeight= (
				m_iNormalHeight
			      + gbAdvParms.Size.Height
			      + 16
			) ;

		}

		void UpdateDialog(bool bChangeFocus)
		{
			Control ctl ;
			string msg= NeedInfo(out ctl) ;

			dtForceDate.Enabled= cbForcedDateTime.Checked ;
			dtForceTime.Enabled= cbForcedDateTime.Checked ;
			tbStartSec.Enabled= cbStartSec.Checked ;
			tbTotalSec.Enabled= cbTotalSec.Checked ;
			ntbTicksPerSec.Enabled= cbTicksPerSec.Checked ;
			btnOk.Enabled= (msg == null) ;

			WriteStatus((msg != null)? msg: FSRWinMsgs.StatusReady) ;

			if (ctl != null) {
				if (bChangeFocus) {
					ActiveControl= ctl ;
				}
			}

			if (m_bAdvancedMode != m_bAdvancedModeState) {
				if (m_bAdvancedMode) {
					Size= new Size(
						Size.Width,
						m_iAdvancedHeight
					) ;
					btnAdvanced.Text= FSRWinMsgs.CmdAdvancedOpen ;
				}
				else {
					Size= new Size(
						Size.Width,
						m_iNormalHeight
					) ;
					btnAdvanced.Text= FSRWinMsgs.CmdAdvancedClose ;
				}
				m_bAdvancedModeState= m_bAdvancedMode ;
			}
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
				return(FSRWinMsgs.StatusEnterVideoDescription) ;
			}
			if (cbStartSec.Checked) {
				if (tbStartSec.Text.Length == 0) {
					ctl= tbStartSec ;
					return(FSRWinMsgs.StatusEnterStartSecond) ;
				}
#if false
				if (!IsValidNumber(tbStartSec.Text)) {
					ctl= tbStartSec ;
					return(FSRWinMsgs.StatusInvalidNumber(cbStartSec.Text)) ;
				}
#endif
			}
			if (cbTotalSec.Checked) {
				if (tbTotalSec.Text.Length == 0) {
					ctl= tbTotalSec ;
					return(FSRWinMsgs.StatusEnterTotalSeconds) ;
				}
#if false
				if (!IsValidNumber(tbTotalSec.Text)) {
					ctl= tbTotalSec ;
					return(FSRWinMsgs.StatusInvalidNumber(cbTotalSec.Text)) ;
				}
#endif
			}
			if (cbTicksPerSec.Checked) {
#if false
				if (ntbTicksPerSec.Text.Length == 0) {
					ctl= ntbTicksPerSec ;
					return(FSRWinMsgs.StatusEnterTicksPerSec) ;
				}
				if (!IsValidNumber(ntbTicksPerSec.Text)) {
					ctl= ntbTicksPerSec ;
					return(FSRWinMsgs.StatusInvalidNumber(cbTicksPerSec.Text)) ;
				}
#endif
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


		//
		//
		//

		private bool m_bAdvancedMode= false ;
		private bool m_bAdvancedModeState= true ; // must be different than "m_bAdvancedMode"
		private int m_iNormalHeight ;
		private int m_iAdvancedHeight ;

		private System.Windows.Forms.Label lblVideoName;
		private System.Windows.Forms.Label lblVideoDescription;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.CheckBox cbForcedDateTime;
		private System.Windows.Forms.TextBox tbStartSec;
		private System.Windows.Forms.DateTimePicker dtForceDate;
		private System.Windows.Forms.CheckBox cbStartSec;
		private System.Windows.Forms.CheckBox cbTotalSec;
		private System.Windows.Forms.TextBox tbTotalSec;
		private System.Windows.Forms.DateTimePicker dtForceTime;
		private System.Windows.Forms.GroupBox gbAdvParms;
		private System.Windows.Forms.GroupBox gbVideoDesc;
		private System.Windows.Forms.Button btnAdvanced;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.CheckBox cbTicksPerSec;
		private System.Windows.Forms.NumericUpDown ntbTicksPerSec;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		

	}
}
