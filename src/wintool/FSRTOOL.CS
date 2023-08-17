/*

	fsrtool.cs - Main Form for fsrwintool
    Copyright (C) 2003-2004, Marty Ross

*/

using System ;
using System.Text ;
using System.IO ;
using System.Data ;
using System.Drawing ;
using System.Reflection ;
using System.Collections ;
using System.ComponentModel ;
using System.Windows.Forms ;
using fsrlib ;

namespace fsrwintool
{

	public class MainForm : Form, IFSRConverterHandlerClient, IFSRPlayerHandlerClient
	{

		//
		//	implementation of "IFSRPipePlayerClient" interface
		//

		public void WaitForUserAfterPlaying()
		{
			MessageBox.Show(
				this,
				FSRWinMsgs.Load("PromptOKContinue"),
				FSRWinMsgs.Load("TitleVideoPlayback"),
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			) ;
		}

		public void UpdatePlayStatus(string msg)
		{
			WriteStatus(msg) ;
			Update() ;
		}


		//
		//	Implementation of "IFSRConverterHandlerClient" interface
		//

		public bool ConfirmReplace(string fn)
		{
			DialogResult choice= MessageBox.Show(
				this,
				FSRWinMsgs.Format("PromptOKFileReplace", fn),
				FSRWinMsgs.Load("TitleConfirmFileReplacement"),
				(
					MessageBoxButtons.OK
					| MessageBoxButtons.OKCancel
				),
				MessageBoxIcon.Question
			) ;
			if (choice == DialogResult.OK) {
				return(true) ;
			}
			return(false) ;
		}

		public FSRConverterFromGPSStream.AuxiliaryData GetAuxiliaryGPSData()
		{
			GPSConversionParameters dlg= new GPSConversionParameters(
				FSRWinMsgs.Format(
					"MessageDefaultVideoName",
					tbSourceFn.Text
				),
				FSRWinMsgs.Format(
					"MessageDefaultVideoDescription",
					tbSourceFn.Text,
					DateTime.Now
				)
			) ;

			if (dlg.ShowDialog(this) == DialogResult.Cancel) {
				return(null) ;
			}

			return(
				new FSRConverterFromGPSStream.AuxiliaryData(
					dlg.VideoName,
					dlg.VideoDescription,
					dlg.VideoTicksPerSecond,
					dlg.VideoStartSecond,
					dlg.VideoTotalSeconds,
					dlg.VideoForceDateTime,
					dlg.VideoMinimumAltitude,
					dlg.VideoReduceJitter
				)
			) ;

		}

		public void UpdateConversionStatus(string msg)
		{
			WriteStatus(msg) ;
			Update() ;
		}


		//
		//	Implementation of "IFSRPlayerHandlerClient" interface
		//

		public FSRPlayerHandler.PlayMethod GetPlayMethod()
		{
			return(((PlayMethodSelection) cbPlayMethod.SelectedItem).MethodID) ;
		}

		private System.Windows.Forms.Button btnViewLogCmd;
		private System.Windows.Forms.HelpProvider helpProvider1;
		private System.Windows.Forms.ComboBox cbPlayMethod;
		private System.Windows.Forms.Button btnHelpCmd;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components= null ;

		public MainForm()
		{
			Construct() ;
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
				// TODO: ensure the Help window is closed here
				m_helpFileResource.Dispose() ;	// delete the temporary help file
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.tbSourceFn = new System.Windows.Forms.TextBox();
			this.gbSource = new System.Windows.Forms.GroupBox();
			this.btnSourceFileOpen = new System.Windows.Forms.Button();
			this.btnDestFileOpen = new System.Windows.Forms.Button();
			this.tbDestFn = new System.Windows.Forms.TextBox();
			this.gbDest = new System.Windows.Forms.GroupBox();
			this.cbPlayMethod = new System.Windows.Forms.ComboBox();
			this.rbToSim = new System.Windows.Forms.RadioButton();
			this.rbToFile = new System.Windows.Forms.RadioButton();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.btnConvertCmd = new System.Windows.Forms.Button();
			this.btnExitCmd = new System.Windows.Forms.Button();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.btnViewLogCmd = new System.Windows.Forms.Button();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.btnHelpCmd = new System.Windows.Forms.Button();
			this.gbSource.SuspendLayout();
			this.gbDest.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbSourceFn
			// 
			this.tbSourceFn.Location = new System.Drawing.Point(40, 24);
			this.tbSourceFn.Name = "tbSourceFn";
			this.tbSourceFn.Size = new System.Drawing.Size(248, 20);
			this.tbSourceFn.TabIndex = 0;
			this.tbSourceFn.Text = "";
			this.tbSourceFn.TextChanged += new System.EventHandler(this.tbSourceFn_TextChanged);
			// 
			// gbSource
			// 
			this.gbSource.Controls.AddRange(new System.Windows.Forms.Control[] {
												   this.btnSourceFileOpen,
												   this.tbSourceFn});
			this.gbSource.Location = new System.Drawing.Point(16, 16);
			this.gbSource.Name = "gbSource";
			this.gbSource.Size = new System.Drawing.Size(344, 64);
			this.gbSource.TabIndex = 0;
			this.gbSource.TabStop = false;
			// 
			// btnSourceFileOpen
			// 
			this.btnSourceFileOpen.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnSourceFileOpen.Image")));
			this.btnSourceFileOpen.Location = new System.Drawing.Point(304, 24);
			this.btnSourceFileOpen.Name = "btnSourceFileOpen";
			this.btnSourceFileOpen.Size = new System.Drawing.Size(24, 23);
			this.btnSourceFileOpen.TabIndex = 2;
			this.btnSourceFileOpen.Click += new System.EventHandler(this.btnSourceFileOpen_Click);
			// 
			// btnDestFileOpen
			// 
			this.btnDestFileOpen.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnDestFileOpen.Image")));
			this.btnDestFileOpen.Location = new System.Drawing.Point(304, 24);
			this.btnDestFileOpen.Name = "btnDestFileOpen";
			this.btnDestFileOpen.Size = new System.Drawing.Size(24, 23);
			this.btnDestFileOpen.TabIndex = 3;
			this.btnDestFileOpen.Click += new System.EventHandler(this.btnDestFileOpen_Click);
			// 
			// tbDestFn
			// 
			this.tbDestFn.Location = new System.Drawing.Point(40, 24);
			this.tbDestFn.Name = "tbDestFn";
			this.tbDestFn.Size = new System.Drawing.Size(248, 20);
			this.tbDestFn.TabIndex = 0;
			this.tbDestFn.Text = "";
			this.tbDestFn.TextChanged += new System.EventHandler(this.tbDestFn_TextChanged);
			// 
			// gbDest
			// 
			this.gbDest.Controls.AddRange(new System.Windows.Forms.Control[] {
												 this.cbPlayMethod,
												 this.rbToSim,
												 this.rbToFile,
												 this.btnDestFileOpen,
												 this.tbDestFn});
			this.gbDest.Location = new System.Drawing.Point(16, 104);
			this.gbDest.Name = "gbDest";
			this.gbDest.Size = new System.Drawing.Size(344, 88);
			this.gbDest.TabIndex = 1;
			this.gbDest.TabStop = false;
			// 
			// cbPlayMethod
			// 
			this.cbPlayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPlayMethod.Enabled = false;
			this.cbPlayMethod.Location = new System.Drawing.Point(216, 56);
			this.cbPlayMethod.Name = "cbPlayMethod";
			this.cbPlayMethod.Size = new System.Drawing.Size(72, 21);
			this.cbPlayMethod.TabIndex = 8;
			// 
			// rbToSim
			// 
			this.rbToSim.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rbToSim.Location = new System.Drawing.Point(16, 56);
			this.rbToSim.Name = "rbToSim";
			this.rbToSim.Size = new System.Drawing.Size(200, 24);
			this.rbToSim.TabIndex = 5;
			this.rbToSim.CheckedChanged += new System.EventHandler(this.rbToSim_CheckedChanged);
			// 
			// rbToFile
			// 
			this.rbToFile.Checked = true;
			this.rbToFile.Location = new System.Drawing.Point(16, 24);
			this.rbToFile.Name = "rbToFile";
			this.rbToFile.Size = new System.Drawing.Size(24, 24);
			this.rbToFile.TabIndex = 4;
			this.rbToFile.TabStop = true;
			// 
			// statusBar1
			// 
			//this.statusBar1.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			//	| System.Windows.Forms.AnchorStyles.Right);
			//this.statusBar1.Dock = System.Windows.Forms.DockStyle.None;
			//this.statusBar1.Location = new System.Drawing.Point(0, 206);
			this.statusBar1.Name = "statusBar1";
			//this.statusBar1.Size = new System.Drawing.Size(466, 22);
			//this.statusBar1.SizingGrip = false;
			this.statusBar1.TabIndex = 2;
			// 
			// btnConvertCmd
			// 
			this.btnConvertCmd.Enabled = false;
			this.btnConvertCmd.Location = new System.Drawing.Point(376, 24);
			this.btnConvertCmd.Name = "btnConvertCmd";
			this.btnConvertCmd.Size = new System.Drawing.Size(75, 24);
			this.btnConvertCmd.TabIndex = 3;
			this.btnConvertCmd.Click += new System.EventHandler(this.btnConvertCmd_Click);
			// 
			// btnExitCmd
			// 
			this.btnExitCmd.CausesValidation = false;
			this.btnExitCmd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnExitCmd.Location = new System.Drawing.Point(376, 88);
			this.btnExitCmd.Name = "btnExitCmd";
			this.btnExitCmd.Size = new System.Drawing.Size(75, 24);
			this.btnExitCmd.TabIndex = 4;
			this.btnExitCmd.Click += new System.EventHandler(this.btnExitCmd_Click);
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(376, 168);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(80, 23);
			this.progressBar1.Step = 5;
			this.progressBar1.TabIndex = 5;
			this.progressBar1.Visible = false;
			// 
			// btnViewLogCmd
			// 
			this.btnViewLogCmd.CausesValidation = false;
			this.btnViewLogCmd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnViewLogCmd.Location = new System.Drawing.Point(376, 56);
			this.btnViewLogCmd.Name = "btnViewLogCmd";
			this.btnViewLogCmd.Size = new System.Drawing.Size(75, 24);
			this.btnViewLogCmd.TabIndex = 6;
			this.btnViewLogCmd.Click += new System.EventHandler(this.btnViewLogCmd_Click);
			// 
			// btnHelpCmd
			// 
			this.btnHelpCmd.Location = new System.Drawing.Point(376, 120);
			this.btnHelpCmd.Name = "btnHelpCmd";
			this.btnHelpCmd.Size = new System.Drawing.Size(75, 24);
			this.btnHelpCmd.TabIndex = 7;
			this.btnHelpCmd.Click += new System.EventHandler(this.btnHelp_Click);
			// 
			// MainForm
			// 
			this.AcceptButton = this.btnConvertCmd;
			//this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(466, 228);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
											  this.btnHelpCmd,
											  this.btnViewLogCmd,
											  this.progressBar1,
											  this.btnExitCmd,
											  this.btnConvertCmd,
											  this.statusBar1,
											  this.gbDest,
											  this.gbSource});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(472, 256);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(472, 256);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
			this.Text = "fsrtool";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.gbSource.ResumeLayout(false);
			this.gbDest.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		struct PlayMethodSelection
		{
			internal PlayMethodSelection(
				FSRPlayerHandler.PlayMethod MethodID,
				string MethodName
			) {
				this.MethodID= MethodID ;
				this.MethodName= MethodName ;
			}
			public override string ToString()
			{
				return(MethodName) ;
			}
			internal FSRPlayerHandler.PlayMethod MethodID ;
			internal string MethodName ;
		}

		void Construct()
		{

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent() ;


			//
			//
			//

			Text= FSRWinMsgs.Load("TitleFSRTool") ;

#if	DEBUG
			Text+= " [Debug]" ;
#endif

			Text+= " " + Assembly.GetExecutingAssembly().GetName().Version ;

			m_logbuffer= new LogBuffer() ;
			Logger.Out= m_logbuffer.Out ;
			Logger.Enabled= true ;
			Logger.Mask= (int) Logger.LogMasks.All ;
			Logger.Mask&= ~((int) Logger.LogMasks.Debug) ;	 // log everything except debug

#if	DEBUG
			Logger.Mask|= (int) Logger.LogMasks.Debug ;	// unless compiled for debug
#endif

			//
			//	Initialize FSR file converter and player
			//

			FSRProgressUpdateDelegate progressUpdateDelegate= new FSRProgressUpdateDelegate(
				UpdateProgress
			) ;
			m_playerHandler= new FSRPlayerHandler(this, progressUpdateDelegate) ;
			m_converterHandler= new FSRConverterHandler(this, progressUpdateDelegate) ;
			m_bCancelFlag= false ;
			m_bOperationInProgress= false ;

			// load control names/text
			btnConvertCmd.Text= FSRWinMsgs.Load("CmdConvert") ;
			btnExitCmd.Text= FSRWinMsgs.Load("CmdExit") ;
			btnHelpCmd.Text= FSRWinMsgs.Load("CmdHelp") ;
			btnViewLogCmd.Text= FSRWinMsgs.Load("CmdViewLog") ;
			rbToSim.Text= FSRWinMsgs.Load("RadioToSimulator") ;
			gbSource.Text= FSRWinMsgs.Load("GroupInput") ;
			gbDest.Text= FSRWinMsgs.Load("GroupOutput") ;

			// load "pop-up" help strings for controls when defined in resources
			LoadControlHelp(this, FSRWinMsgs.Load("HelpNone")) ;

			//	Extract the embedded help file, if present
			m_helpFileResource= new ResourceFile("fsrwintool.chm") ;
			m_helpFileResource.Open() ;

			// make "Help" button visible iff we located/extracted the help file
			btnHelpCmd.Visible= (m_helpFileResource.Name != null) ;

			// set "Namespace" in "Help Provider" in order to launch help when user presses F1
			if (m_helpFileResource.Name != null) {
				helpProvider1.HelpNamespace= m_helpFileResource.Name ;
			}

			// load "play methods" pull-down listbox
			FSRPlayerHandler.PlayMethod[] pma= FSRPlayerHandler.GetPlayMethods() ;
			m_playmethods= new PlayMethodSelection[pma.Length] ;
			for (int i= 0; i< pma.Length; i++) {
				string MethodKey= "PMSEL_" + Enum.GetName(pma[i].GetType(), pma[i]) ;
				string MethodName= FSRWinMsgs.Load(MethodKey) ;
				m_playmethods[i]= new PlayMethodSelection(pma[i], MethodName) ;
			}
			cbPlayMethod.DataSource= m_playmethods ;

		}


		//
		//	application UI event handling
		//

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			UpdateStatus() ;
			RecalcViewLogButton() ;

		}

		void btnSourceFileOpen_Click(object sender, System.EventArgs e)
		{
			openFileDialog1.Filter= ConstructFileOpenFilter(
				rbToSim.Checked
			      ? GetAllValidInputTypesForPlayOperation()
			      : GetAllValidInputTypesForConversionOperation()
			) ;
			openFileDialog1.Title= FSRWinMsgs.Load("StatusSelectInput") ;
			openFileDialog1.CheckFileExists= true ;
			openFileDialog1.ValidateNames= true ;

			WriteStatus(FSRWinMsgs.Load("StatusSelectInput")) ;
			DialogResult dr= openFileDialog1.ShowDialog(this) ;
			if (dr == DialogResult.OK) { 
				tbSourceFn.Text= openFileDialog1.FileName ;
			}
		}

		void btnDestFileOpen_Click(object sender, System.EventArgs e)
		{
			saveFileDialog1.Filter= ConstructFileOpenFilter(
				GetAllValidOutputTypesForConversionOperation()
			) ;
			saveFileDialog1.Title= FSRWinMsgs.Load("StatusSelectOutput") ;
			saveFileDialog1.OverwritePrompt= false ;
			saveFileDialog1.ValidateNames= true ;

			WriteStatus(FSRWinMsgs.Load("StatusSelectOutput")) ;
			DialogResult dr= saveFileDialog1.ShowDialog(this) ;
			if (dr == DialogResult.OK) {
				tbDestFn.Text= saveFileDialog1.FileName ;
			}
		}

		void btnExitCmd_Click(object sender, System.EventArgs e) {
			Application.Exit() ;
		}

		private void btnHelp_Click(object sender, System.EventArgs e)
		{
			Help.ShowHelp(this, m_helpFileResource.Name) ;
		}

		void btnConvertCmd_Click(object sender, System.EventArgs e)
		{

			if (m_bOperationInProgress) {
				m_bCancelFlag= true ;
				return ;
			}
			m_bCancelFlag= false ;
			m_bOperationInProgress= true ;

			btnExitCmd.Enabled= false ;
			string origName= btnConvertCmd.Text ;
			btnConvertCmd.Text= FSRWinMsgs.Load("CmdCancel") ;
			progressBar1.Show() ;

			Update() ;

			Cursor oldCursor= Cursor.Current ;
			Cursor.Current= Cursors.WaitCursor ;

			WriteStatus(FSRWinMsgs.Load("StatusBusy")) ;

			m_logbuffer.Clear() ;
			RecalcViewLogButton() ;

			if (rbToSim.Checked) {
				m_playerHandler.Play(tbSourceFn.Text) ;
			}
			else {
				m_converterHandler.Convert(
					tbSourceFn.Text,
					tbDestFn.Text
				) ;
			}

			Cursor.Current= oldCursor ;
			btnConvertCmd.Text= origName ;
			progressBar1.Hide() ;

			btnExitCmd.Enabled= true ;

			m_bOperationInProgress= false ;

			RecalcViewLogButton() ;

		}

		private void btnViewLogCmd_Click(object sender, System.EventArgs e)
		{
			LogViewerForm lvf= new LogViewerForm(m_logbuffer.ToString()) ;
			lvf.ShowDialog(this) ;
		}

		void rbToSim_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbToSim.Checked) {
				tbDestFn.Enabled= false ;
				btnDestFileOpen.Enabled= false ;
				cbPlayMethod.Enabled= true ;
			}
			else {
				tbDestFn.Enabled= true ;
				btnDestFileOpen.Enabled= true ;
				cbPlayMethod.Enabled= false ;
			}
			RecalcConvertCmdButton() ;
			UpdateStatus() ;
		}

		private void tbSourceFn_TextChanged(object sender, System.EventArgs e)
		{
			RecalcConvertCmdButton() ;
			UpdateStatus() ;
		}

		private void tbDestFn_TextChanged(object sender, System.EventArgs e)
		{
			RecalcConvertCmdButton() ;
			UpdateStatus() ;
		}


		//
		//
		//

		void LoadControlHelp(Control cparent, string hsNone)
		{
			foreach (Control c in cparent.Controls) {
				string name= "HelpCtl_" + c.Name ; 
				string hs= FSRWinMsgs.Load(name) ;
				helpProvider1.SetHelpString(c, (hs == null)? hsNone: hs) ;
				LoadControlHelp(c, hsNone) ;
			}
		}


		//
		//
		//

		private void RecalcConvertCmdButton()
		{

			btnConvertCmd.Enabled= IsValidOperation() ;

			if (rbToSim.Checked) {
				btnConvertCmd.Text= FSRWinMsgs.Load("CmdPlay") ;
			}
			else {
				btnConvertCmd.Text= FSRWinMsgs.Load("CmdConvert") ;
			}

		}

		private void RecalcViewLogButton()
		{
			btnViewLogCmd.Enabled= !m_logbuffer.IsEmpty() ;
		}

		bool IsSourceSelected()
		{
			return(tbSourceFn.Text.Length> 0) ;
		}

		bool IsDestSelected()
		{
			return((tbDestFn.Text.Length != 0) || rbToSim.Checked) ;
		}

		bool IsValidOperation()
		{
			if (!IsSourceSelected()) return(false) ;
			if (!IsDestSelected()) return(false) ;
			if (rbToSim.Checked) {
				return(
					FSRPlayerHandler.GetPlayType(tbSourceFn.Text)
				     != FSRPlayerHandler.PlayType.Unknown
				) ;
			}
			return(
				FSRConverterHandler.GetConvertType(
					tbSourceFn.Text,
					tbDestFn.Text
				) != FSRConverterHandler.ConvertType.Unknown
			) ;
		}

		void UpdateStatus()
		{
			if (!IsValidOperation()) {
				if (!IsSourceSelected()) {
					WriteStatus(FSRWinMsgs.Load("StatusSelectInput")) ;
					return ;
				}
				if (!IsDestSelected()) {
					WriteStatus(FSRWinMsgs.Load("StatusSelectOutput")) ;
					return ;
				}
				WriteStatus(FSRWinMsgs.Load("StatusSelectValidOperation")) ;
				return ;
			}
			WriteStatus(FSRWinMsgs.Load("StatusReady")) ;
		}

		void WriteStatus(string msg)
		{
			statusBar1.Text= msg ;
		}


		//
		//
		//

		/// <returns>
		/// List of FSRFile.Type values corresponding to the set of
		/// file types that can be "Played" directly in the simulator.
		/// </returns>
		ArrayList GetAllValidInputTypesForPlayOperation()
		{
			ArrayList itypes= new ArrayList() ;
			FSRFile.Type[] alltypes= FSRFile.GetTypeValues() ;
			foreach (FSRFile.Type ift in alltypes) {
				if (ift == FSRFile.Type.Unknown) continue ;
				if (
					FSRPlayerHandler.GetPlayType(ift)
					!= FSRPlayerHandler.PlayType.Unknown
				) {
					itypes.Sort() ;
					if (itypes.BinarySearch(ift)< 0) {
						itypes.Add(ift) ;
					}
				}
			}
			return(itypes) ;
		}

		/// <returns>
		/// List of FSRFile.Type values corresponding to the set
		/// of file types that can be converted to the type of file
		/// which may be currently selected for output.
		/// </returns>
		ArrayList GetAllValidInputTypesForConversionOperation()
		{
			return(
				GetAllValidTypesForConversionOperation(
					tbDestFn.Text,
					new CanConvertDelegate(CanConvertToFrom)
				)
			) ;
		}

		/// <returns>
		/// List of FSRFile.Type values corresponding to the set of
		/// file types that can be produced by conversion from the
		/// type of file which may be currently selected for input.
		/// </returns>
		ArrayList GetAllValidOutputTypesForConversionOperation()
		{
			return(
				GetAllValidTypesForConversionOperation(
					tbSourceFn.Text,
					new CanConvertDelegate(CanConvertFromTo)
				)
			) ;
		}

		/// <summary>
		/// Construct a list of supported file types for a conversion
		/// operation.  We (may) know one of the file names participating
		/// in the conversion; we use this information (if available) to
		/// construct the list.  If we can't determine the type of the
		/// "other" file, we must return the entire list of all file
		/// types that can participate in the indicated conversion
		/// operation.
		/// </summary>
		/// <param name="fn">
		/// Name of the "other" file participating in the conversion.
		/// </param>
		/// <param name="canconvert">
		/// Delegate method appropriate to direction of conversion.
		/// </param>
		/// <returns>
		/// List of FSRFile.Type values corresponding to set of file
		/// types that are supported for indicated conversion operation.
		/// </returns>
		ArrayList GetAllValidTypesForConversionOperation(
			string fn,
			CanConvertDelegate canconvert
		) {
			FSRFile.Type ift= FSRFile.GetType(fn) ;
			FSRFile.Type[] alltypes= FSRFile.GetTypeValues() ;
			FSRFile.Type[] itypes= (
				(ift == FSRFile.Type.Unknown)
			      ? alltypes
			      : new FSRFile.Type[1] { ift }
			) ;

			ArrayList otypes= new ArrayList() ;
			foreach (FSRFile.Type ift0 in itypes) {
				if (ift0 == FSRFile.Type.Unknown) continue ;
				foreach (FSRFile.Type oft in alltypes) {
					if (oft == FSRFile.Type.Unknown) continue ;
					if (canconvert(ift0, oft)) {
						otypes.Sort() ;
						if (otypes.BinarySearch(oft)< 0) {
							otypes.Add(oft) ;
						}
					}
				}
			}

			return(otypes) ;
		}

		delegate bool CanConvertDelegate(FSRFile.Type f1, FSRFile.Type f2) ;

		bool CanConvertFromTo(FSRFile.Type from, FSRFile.Type to)
		{
			return(
				FSRConverterHandler.GetConvertType(from, to)
			     != FSRConverterHandler.ConvertType.Unknown
			) ;
		}
	
		bool CanConvertToFrom(FSRFile.Type from, FSRFile.Type to)
		{
			return(
				FSRConverterHandler.GetConvertType(to, from)
			     != FSRConverterHandler.ConvertType.Unknown
			) ;
		}
	
		/// <param name="types">
		/// List of FSRFile.Type values from which to contruct filter.
		/// </param>
		/// <returns>FileOpen or FileSave dialog "filter" string</returns>
		string ConstructFileOpenFilter(ArrayList types)
		{
			string filter= "" ;
			foreach (object o in types) {
				if (filter.Length> 0) filter+= "|" ;
				filter+= FSRWinMsgs.Load("Filter" + ((FSRFile.Type) o).ToString()) ;
			}
			if (filter.Length> 0) filter+= "|" ;
			filter+= FSRWinMsgs.Load("FilterAll") ;
			return(filter) ;
		}


		//
		//	Implementation for "FSRProgressUpdateDelegate"
		//

		bool UpdateProgress(double fractionComplete)
		{
			progressBar1.Value= (int) ((fractionComplete * 100.0) + 0.5) ;
			Application.DoEvents() ;
			return(m_bCancelFlag) ;
		}


		//
		//
		//

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.RadioButton rbToFile;
		private System.Windows.Forms.RadioButton rbToSim;
		private System.Windows.Forms.Button btnSourceFileOpen;
		private System.Windows.Forms.Button btnDestFileOpen;
		private System.Windows.Forms.Button btnConvertCmd;
		private System.Windows.Forms.Button btnExitCmd;
		private System.Windows.Forms.GroupBox gbSource;
		private System.Windows.Forms.GroupBox gbDest;
		private System.Windows.Forms.TextBox tbSourceFn;
		private System.Windows.Forms.TextBox tbDestFn;
		private System.Windows.Forms.ProgressBar progressBar1;

		private FSRPlayerHandler m_playerHandler ;
		private FSRConverterHandler m_converterHandler ;
		private bool m_bOperationInProgress ;
		private bool m_bCancelFlag ;
		private PlayMethodSelection[] m_playmethods ;

		private LogBuffer m_logbuffer ;

		private ResourceFile m_helpFileResource ;

	}

}
