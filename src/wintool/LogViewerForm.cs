using System ;
using System.Drawing ;
using System.Collections ;
using System.ComponentModel ;
using System.Windows.Forms ;

namespace fsrwintool
{

	public class LogViewerForm : System.Windows.Forms.Form
	{

		public LogViewerForm(string log)
		{
			InitializeComponent();
			LoadResources() ;
			txtLog.Text= log ;
		}

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

		void LoadResources()
		{
			Text= FSRWinMsgs.Load("TitleViewLog") ;
			btnOKCmd.Text= FSRWinMsgs.Load("CmdOk") ;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LogViewerForm));
			this.txtLog = new System.Windows.Forms.TextBox();
			this.btnOKCmd = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtLog
			// 
			this.txtLog.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.txtLog.Location = new System.Drawing.Point(16, 24);
			this.txtLog.MaxLength = 0;
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(376, 200);
			this.txtLog.TabIndex = 0;
			this.txtLog.Text = "";
			this.txtLog.WordWrap = false;
			// 
			// btnOKCmd
			// 
			this.btnOKCmd.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btnOKCmd.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOKCmd.Location = new System.Drawing.Point(408, 24);
			this.btnOKCmd.Name = "btnOKCmd";
			this.btnOKCmd.Size = new System.Drawing.Size(75, 32);
			this.btnOKCmd.TabIndex = 4;
			// 
			// LogViewerForm
			// 
			this.AcceptButton = this.btnOKCmd;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnOKCmd;
			this.ClientSize = new System.Drawing.Size(496, 242);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
											  this.btnOKCmd,
											  this.txtLog});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(216, 112);
			this.Name = "LogViewerForm";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);

		}
		#endregion

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtLog ;
		private System.Windows.Forms.Button btnOKCmd ;

	}
}
