namespace Sys0Decompiler
{
	partial class DecompilerForm
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
			if(disposing && (components != null))
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecompilerForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.tlsFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsSavePref = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsImportSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsExportSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsExit = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsAdvanced = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsOutputJunk = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsDCmd = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsDiacritic = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsHelpMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.tlsGeneralHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.tabCtrlMain = new System.Windows.Forms.TabControl();
			this.tabDecompile = new System.Windows.Forms.TabPage();
			this.grpVarFormat = new System.Windows.Forms.GroupBox();
			this.rdoVAROnly = new System.Windows.Forms.RadioButton();
			this.rdoFullLabel = new System.Windows.Forms.RadioButton();
			this.grpDecompileOut = new System.Windows.Forms.GroupBox();
			this.rdoDecompileOutUTF8 = new System.Windows.Forms.RadioButton();
			this.rdoDecompileOutShiftJIS = new System.Windows.Forms.RadioButton();
			this.grpDecompileSource = new System.Windows.Forms.GroupBox();
			this.rdoDecompileSourceUTF8 = new System.Windows.Forms.RadioButton();
			this.rdoDecompileSourceMSX = new System.Windows.Forms.RadioButton();
			this.rdoDecompileSourceShiftJIS = new System.Windows.Forms.RadioButton();
			this.grpTextMode = new System.Windows.Forms.GroupBox();
			this.rdoRaw = new System.Windows.Forms.RadioButton();
			this.rdoKatakana = new System.Windows.Forms.RadioButton();
			this.rdoHiragana = new System.Windows.Forms.RadioButton();
			this.btnRevCompileToDecompile = new System.Windows.Forms.Button();
			this.grpDecompileSysVer = new System.Windows.Forms.GroupBox();
			this.rdoDecompileSys3 = new System.Windows.Forms.RadioButton();
			this.rdoDecompileSys2 = new System.Windows.Forms.RadioButton();
			this.rdoDecompileSys1 = new System.Windows.Forms.RadioButton();
			this.btnBeginDecompile = new System.Windows.Forms.Button();
			this.grpDecompileOpts = new System.Windows.Forms.GroupBox();
			this.rdoDecompileOptVerbobjs = new System.Windows.Forms.RadioButton();
			this.rdoDecompileOptPages = new System.Windows.Forms.RadioButton();
			this.rdoDecompileOptAll = new System.Windows.Forms.RadioButton();
			this.btnDecompileTargetDir = new System.Windows.Forms.Button();
			this.txtDecompileTargetDir = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnDecompileSrcDir = new System.Windows.Forms.Button();
			this.txtDecompileSrcDir = new System.Windows.Forms.TextBox();
			this.lblDecompileSrcDir = new System.Windows.Forms.Label();
			this.tabCompile = new System.Windows.Forms.TabPage();
			this.grpCOut = new System.Windows.Forms.GroupBox();
			this.rdoCompileOutUTF8 = new System.Windows.Forms.RadioButton();
			this.rdoCompileOutShiftJIS = new System.Windows.Forms.RadioButton();
			this.grpCSource = new System.Windows.Forms.GroupBox();
			this.rdoCompileSourceUTF8 = new System.Windows.Forms.RadioButton();
			this.rdoCompileSourceShiftJIS = new System.Windows.Forms.RadioButton();
			this.btnRevDecompileToCompile = new System.Windows.Forms.Button();
			this.chkDeleteTemp = new System.Windows.Forms.CheckBox();
			this.grpCompileSysVer = new System.Windows.Forms.GroupBox();
			this.rdoCompileSys3 = new System.Windows.Forms.RadioButton();
			this.rdoCompileSys2 = new System.Windows.Forms.RadioButton();
			this.rdoCompileSys1 = new System.Windows.Forms.RadioButton();
			this.btnBeginCompile = new System.Windows.Forms.Button();
			this.grpCompileOpts = new System.Windows.Forms.GroupBox();
			this.rdoCompileOptVerbobjs = new System.Windows.Forms.RadioButton();
			this.rdoCompileOptPages = new System.Windows.Forms.RadioButton();
			this.rdoCompileOptAll = new System.Windows.Forms.RadioButton();
			this.btnCompileTargetDir = new System.Windows.Forms.Button();
			this.txtCompileTargetDir = new System.Windows.Forms.TextBox();
			this.lblCompileTargetDir = new System.Windows.Forms.Label();
			this.btnCompileSrcDir = new System.Windows.Forms.Button();
			this.txtCompileSrcDir = new System.Windows.Forms.TextBox();
			this.lblCompileSrcDir = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.tabCtrlMain.SuspendLayout();
			this.tabDecompile.SuspendLayout();
			this.grpVarFormat.SuspendLayout();
			this.grpDecompileOut.SuspendLayout();
			this.grpDecompileSource.SuspendLayout();
			this.grpTextMode.SuspendLayout();
			this.grpDecompileSysVer.SuspendLayout();
			this.grpDecompileOpts.SuspendLayout();
			this.tabCompile.SuspendLayout();
			this.grpCOut.SuspendLayout();
			this.grpCSource.SuspendLayout();
			this.grpCompileSysVer.SuspendLayout();
			this.grpCompileOpts.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsFileMenu,
            this.tlsAdvanced,
            this.tlsHelpMenu});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(484, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// tlsFileMenu
			// 
			this.tlsFileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsSavePref,
            this.toolStripSeparator2,
            this.tlsImportSettings,
            this.tlsExportSettings,
            this.toolStripSeparator1,
            this.tlsExit});
			this.tlsFileMenu.Name = "tlsFileMenu";
			this.tlsFileMenu.Size = new System.Drawing.Size(37, 20);
			this.tlsFileMenu.Text = "&File";
			// 
			// tlsSavePref
			// 
			this.tlsSavePref.Name = "tlsSavePref";
			this.tlsSavePref.Size = new System.Drawing.Size(155, 22);
			this.tlsSavePref.Text = "&Save Settings";
			this.tlsSavePref.Click += new System.EventHandler(this.TlsSavePref_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
			// 
			// tlsImportSettings
			// 
			this.tlsImportSettings.Name = "tlsImportSettings";
			this.tlsImportSettings.Size = new System.Drawing.Size(155, 22);
			this.tlsImportSettings.Text = "&Import Settings";
			this.tlsImportSettings.Click += new System.EventHandler(this.ImportSettingsToolstrip_Click);
			// 
			// tlsExportSettings
			// 
			this.tlsExportSettings.Name = "tlsExportSettings";
			this.tlsExportSettings.Size = new System.Drawing.Size(155, 22);
			this.tlsExportSettings.Text = "&Export Settings";
			this.tlsExportSettings.Click += new System.EventHandler(this.ExportToolstrip_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
			// 
			// tlsExit
			// 
			this.tlsExit.Name = "tlsExit";
			this.tlsExit.Size = new System.Drawing.Size(155, 22);
			this.tlsExit.Text = "E&xit";
			this.tlsExit.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// tlsAdvanced
			// 
			this.tlsAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsOutputJunk,
            this.tlsDCmd,
            this.tlsDiacritic});
			this.tlsAdvanced.Name = "tlsAdvanced";
			this.tlsAdvanced.Size = new System.Drawing.Size(117, 20);
			this.tlsAdvanced.Text = "Advanced Settings";
			// 
			// tlsOutputJunk
			// 
			this.tlsOutputJunk.Name = "tlsOutputJunk";
			this.tlsOutputJunk.Size = new System.Drawing.Size(219, 22);
			this.tlsOutputJunk.Text = "Output Junk Code";
			this.tlsOutputJunk.Click += new System.EventHandler(this.outputJunkCodeToolStripMenuItem_Click);
			// 
			// tlsDCmd
			// 
			this.tlsDCmd.Name = "tlsDCmd";
			this.tlsDCmd.Size = new System.Drawing.Size(219, 22);
			this.tlsDCmd.Text = "Use 3-Param D Command";
			this.tlsDCmd.Click += new System.EventHandler(this.use3ParamDCommandToolStripMenuItem_Click);
			// 
			// tlsDiacritic
			// 
			this.tlsDiacritic.Checked = true;
			this.tlsDiacritic.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tlsDiacritic.Enabled = false;
			this.tlsDiacritic.Name = "tlsDiacritic";
			this.tlsDiacritic.Size = new System.Drawing.Size(219, 22);
			this.tlsDiacritic.Text = "Apply Single-Byte Diacritics";
			this.tlsDiacritic.Click += new System.EventHandler(this.tlsDiacritic_Clicked);
			// 
			// tlsHelpMenu
			// 
			this.tlsHelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsGeneralHelp,
            this.toolStripSeparator3,
            this.tlsAbout});
			this.tlsHelpMenu.Name = "tlsHelpMenu";
			this.tlsHelpMenu.Size = new System.Drawing.Size(44, 20);
			this.tlsHelpMenu.Text = "&Help";
			// 
			// tlsGeneralHelp
			// 
			this.tlsGeneralHelp.Name = "tlsGeneralHelp";
			this.tlsGeneralHelp.Size = new System.Drawing.Size(180, 22);
			this.tlsGeneralHelp.Text = "General Help";
			this.tlsGeneralHelp.Click += new System.EventHandler(this.GeneralHelpToolstrip_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
			// 
			// tlsAbout
			// 
			this.tlsAbout.Name = "tlsAbout";
			this.tlsAbout.Size = new System.Drawing.Size(180, 22);
			this.tlsAbout.Text = "About";
			this.tlsAbout.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// tabCtrlMain
			// 
			this.tabCtrlMain.Controls.Add(this.tabDecompile);
			this.tabCtrlMain.Controls.Add(this.tabCompile);
			this.tabCtrlMain.Location = new System.Drawing.Point(3, 27);
			this.tabCtrlMain.Name = "tabCtrlMain";
			this.tabCtrlMain.SelectedIndex = 0;
			this.tabCtrlMain.Size = new System.Drawing.Size(481, 323);
			this.tabCtrlMain.TabIndex = 4;
			this.tabCtrlMain.SelectedIndexChanged += new System.EventHandler(this.tabCtrlMain_SelectedIndexChanged);
			// 
			// tabDecompile
			// 
			this.tabDecompile.Controls.Add(this.grpVarFormat);
			this.tabDecompile.Controls.Add(this.grpDecompileOut);
			this.tabDecompile.Controls.Add(this.grpDecompileSource);
			this.tabDecompile.Controls.Add(this.grpTextMode);
			this.tabDecompile.Controls.Add(this.btnRevCompileToDecompile);
			this.tabDecompile.Controls.Add(this.grpDecompileSysVer);
			this.tabDecompile.Controls.Add(this.btnBeginDecompile);
			this.tabDecompile.Controls.Add(this.grpDecompileOpts);
			this.tabDecompile.Controls.Add(this.btnDecompileTargetDir);
			this.tabDecompile.Controls.Add(this.txtDecompileTargetDir);
			this.tabDecompile.Controls.Add(this.label1);
			this.tabDecompile.Controls.Add(this.btnDecompileSrcDir);
			this.tabDecompile.Controls.Add(this.txtDecompileSrcDir);
			this.tabDecompile.Controls.Add(this.lblDecompileSrcDir);
			this.tabDecompile.Location = new System.Drawing.Point(4, 22);
			this.tabDecompile.Name = "tabDecompile";
			this.tabDecompile.Padding = new System.Windows.Forms.Padding(3);
			this.tabDecompile.Size = new System.Drawing.Size(473, 297);
			this.tabDecompile.TabIndex = 0;
			this.tabDecompile.Text = "Decompile";
			this.tabDecompile.UseVisualStyleBackColor = true;
			// 
			// grpVarFormat
			// 
			this.grpVarFormat.Controls.Add(this.rdoVAROnly);
			this.grpVarFormat.Controls.Add(this.rdoFullLabel);
			this.grpVarFormat.Location = new System.Drawing.Point(311, 64);
			this.grpVarFormat.Name = "grpVarFormat";
			this.grpVarFormat.Size = new System.Drawing.Size(156, 95);
			this.grpVarFormat.TabIndex = 15;
			this.grpVarFormat.TabStop = false;
			this.grpVarFormat.Text = "Variable Format";
			// 
			// rdoVAROnly
			// 
			this.rdoVAROnly.AutoSize = true;
			this.rdoVAROnly.Location = new System.Drawing.Point(9, 43);
			this.rdoVAROnly.Name = "rdoVAROnly";
			this.rdoVAROnly.Size = new System.Drawing.Size(115, 17);
			this.rdoVAROnly.TabIndex = 1;
			this.rdoVAROnly.Text = "\"VAR\" Labels Only";
			this.rdoVAROnly.UseVisualStyleBackColor = true;
			// 
			// rdoFullLabel
			// 
			this.rdoFullLabel.AutoSize = true;
			this.rdoFullLabel.Checked = true;
			this.rdoFullLabel.Location = new System.Drawing.Point(9, 20);
			this.rdoFullLabel.Name = "rdoFullLabel";
			this.rdoFullLabel.Size = new System.Drawing.Size(89, 17);
			this.rdoFullLabel.TabIndex = 0;
			this.rdoFullLabel.TabStop = true;
			this.rdoFullLabel.Text = "Full Label Set";
			this.rdoFullLabel.UseVisualStyleBackColor = true;
			// 
			// grpDecompileOut
			// 
			this.grpDecompileOut.Controls.Add(this.rdoDecompileOutUTF8);
			this.grpDecompileOut.Controls.Add(this.rdoDecompileOutShiftJIS);
			this.grpDecompileOut.Location = new System.Drawing.Point(128, 165);
			this.grpDecompileOut.Name = "grpDecompileOut";
			this.grpDecompileOut.Size = new System.Drawing.Size(178, 95);
			this.grpDecompileOut.TabIndex = 14;
			this.grpDecompileOut.TabStop = false;
			this.grpDecompileOut.Text = "Text Output Encoding";
			// 
			// rdoDecompileOutUTF8
			// 
			this.rdoDecompileOutUTF8.AutoSize = true;
			this.rdoDecompileOutUTF8.Location = new System.Drawing.Point(7, 43);
			this.rdoDecompileOutUTF8.Name = "rdoDecompileOutUTF8";
			this.rdoDecompileOutUTF8.Size = new System.Drawing.Size(55, 17);
			this.rdoDecompileOutUTF8.TabIndex = 1;
			this.rdoDecompileOutUTF8.Text = "UTF-8";
			this.rdoDecompileOutUTF8.UseVisualStyleBackColor = true;
			// 
			// rdoDecompileOutShiftJIS
			// 
			this.rdoDecompileOutShiftJIS.AutoSize = true;
			this.rdoDecompileOutShiftJIS.Checked = true;
			this.rdoDecompileOutShiftJIS.Location = new System.Drawing.Point(7, 20);
			this.rdoDecompileOutShiftJIS.Name = "rdoDecompileOutShiftJIS";
			this.rdoDecompileOutShiftJIS.Size = new System.Drawing.Size(64, 17);
			this.rdoDecompileOutShiftJIS.TabIndex = 0;
			this.rdoDecompileOutShiftJIS.TabStop = true;
			this.rdoDecompileOutShiftJIS.Text = "Shift-JIS";
			this.rdoDecompileOutShiftJIS.UseVisualStyleBackColor = true;
			// 
			// grpDecompileSource
			// 
			this.grpDecompileSource.Controls.Add(this.rdoDecompileSourceUTF8);
			this.grpDecompileSource.Controls.Add(this.rdoDecompileSourceMSX);
			this.grpDecompileSource.Controls.Add(this.rdoDecompileSourceShiftJIS);
			this.grpDecompileSource.Location = new System.Drawing.Point(10, 165);
			this.grpDecompileSource.Name = "grpDecompileSource";
			this.grpDecompileSource.Size = new System.Drawing.Size(112, 95);
			this.grpDecompileSource.TabIndex = 13;
			this.grpDecompileSource.TabStop = false;
			this.grpDecompileSource.Text = "Game Encoding";
			// 
			// rdoDecompileSourceUTF8
			// 
			this.rdoDecompileSourceUTF8.AutoSize = true;
			this.rdoDecompileSourceUTF8.Location = new System.Drawing.Point(7, 43);
			this.rdoDecompileSourceUTF8.Name = "rdoDecompileSourceUTF8";
			this.rdoDecompileSourceUTF8.Size = new System.Drawing.Size(55, 17);
			this.rdoDecompileSourceUTF8.TabIndex = 1;
			this.rdoDecompileSourceUTF8.Text = "UTF-8";
			this.rdoDecompileSourceUTF8.UseVisualStyleBackColor = true;
			this.rdoDecompileSourceUTF8.CheckedChanged += new System.EventHandler(this.rdoDecompileSourceUTF8_CheckedChanged);
			// 
			// rdoDecompileSourceMSX
			// 
			this.rdoDecompileSourceMSX.AutoSize = true;
			this.rdoDecompileSourceMSX.Location = new System.Drawing.Point(7, 66);
			this.rdoDecompileSourceMSX.Name = "rdoDecompileSourceMSX";
			this.rdoDecompileSourceMSX.Size = new System.Drawing.Size(99, 17);
			this.rdoDecompileSourceMSX.TabIndex = 2;
			this.rdoDecompileSourceMSX.Text = "MSX (Unstable)";
			this.rdoDecompileSourceMSX.UseVisualStyleBackColor = true;
			this.rdoDecompileSourceMSX.CheckedChanged += new System.EventHandler(this.rdoMSX_CheckedChanged);
			// 
			// rdoDecompileSourceShiftJIS
			// 
			this.rdoDecompileSourceShiftJIS.AutoSize = true;
			this.rdoDecompileSourceShiftJIS.Checked = true;
			this.rdoDecompileSourceShiftJIS.Location = new System.Drawing.Point(7, 20);
			this.rdoDecompileSourceShiftJIS.Name = "rdoDecompileSourceShiftJIS";
			this.rdoDecompileSourceShiftJIS.Size = new System.Drawing.Size(64, 17);
			this.rdoDecompileSourceShiftJIS.TabIndex = 0;
			this.rdoDecompileSourceShiftJIS.TabStop = true;
			this.rdoDecompileSourceShiftJIS.Text = "Shift-JIS";
			this.rdoDecompileSourceShiftJIS.UseVisualStyleBackColor = true;
			this.rdoDecompileSourceShiftJIS.CheckedChanged += new System.EventHandler(this.rdoDecompileSourceShiftJIS_CheckedChanged);
			// 
			// grpTextMode
			// 
			this.grpTextMode.Controls.Add(this.rdoRaw);
			this.grpTextMode.Controls.Add(this.rdoKatakana);
			this.grpTextMode.Controls.Add(this.rdoHiragana);
			this.grpTextMode.Location = new System.Drawing.Point(311, 165);
			this.grpTextMode.Name = "grpTextMode";
			this.grpTextMode.Size = new System.Drawing.Size(156, 95);
			this.grpTextMode.TabIndex = 9;
			this.grpTextMode.TabStop = false;
			this.grpTextMode.Text = "Shift-JIS Source Conversion";
			// 
			// rdoRaw
			// 
			this.rdoRaw.AutoSize = true;
			this.rdoRaw.Location = new System.Drawing.Point(9, 66);
			this.rdoRaw.Name = "rdoRaw";
			this.rdoRaw.Size = new System.Drawing.Size(71, 17);
			this.rdoRaw.TabIndex = 2;
			this.rdoRaw.Text = "Raw Text";
			this.rdoRaw.UseVisualStyleBackColor = true;
			// 
			// rdoKatakana
			// 
			this.rdoKatakana.AutoSize = true;
			this.rdoKatakana.Location = new System.Drawing.Point(9, 43);
			this.rdoKatakana.Name = "rdoKatakana";
			this.rdoKatakana.Size = new System.Drawing.Size(126, 17);
			this.rdoKatakana.TabIndex = 1;
			this.rdoKatakana.Text = "Katakana / Hankaku";
			this.rdoKatakana.UseVisualStyleBackColor = true;
			// 
			// rdoHiragana
			// 
			this.rdoHiragana.AutoSize = true;
			this.rdoHiragana.Checked = true;
			this.rdoHiragana.Location = new System.Drawing.Point(9, 20);
			this.rdoHiragana.Name = "rdoHiragana";
			this.rdoHiragana.Size = new System.Drawing.Size(122, 17);
			this.rdoHiragana.TabIndex = 0;
			this.rdoHiragana.TabStop = true;
			this.rdoHiragana.Text = "Hiragana / Zankaku";
			this.rdoHiragana.UseVisualStyleBackColor = true;
			// 
			// btnRevCompileToDecompile
			// 
			this.btnRevCompileToDecompile.Location = new System.Drawing.Point(10, 267);
			this.btnRevCompileToDecompile.Name = "btnRevCompileToDecompile";
			this.btnRevCompileToDecompile.Size = new System.Drawing.Size(168, 23);
			this.btnRevCompileToDecompile.TabIndex = 12;
			this.btnRevCompileToDecompile.Text = "Copy/Reverse Compile Settings";
			this.btnRevCompileToDecompile.UseVisualStyleBackColor = true;
			this.btnRevCompileToDecompile.Click += new System.EventHandler(this.BtnRevCompileToDecompile_Click);
			// 
			// grpDecompileSysVer
			// 
			this.grpDecompileSysVer.Controls.Add(this.rdoDecompileSys3);
			this.grpDecompileSysVer.Controls.Add(this.rdoDecompileSys2);
			this.grpDecompileSysVer.Controls.Add(this.rdoDecompileSys1);
			this.grpDecompileSysVer.Location = new System.Drawing.Point(10, 64);
			this.grpDecompileSysVer.Name = "grpDecompileSysVer";
			this.grpDecompileSysVer.Size = new System.Drawing.Size(112, 95);
			this.grpDecompileSysVer.TabIndex = 9;
			this.grpDecompileSysVer.TabStop = false;
			this.grpDecompileSysVer.Text = "System Version";
			// 
			// rdoDecompileSys3
			// 
			this.rdoDecompileSys3.AutoSize = true;
			this.rdoDecompileSys3.Location = new System.Drawing.Point(6, 66);
			this.rdoDecompileSys3.Name = "rdoDecompileSys3";
			this.rdoDecompileSys3.Size = new System.Drawing.Size(77, 17);
			this.rdoDecompileSys3.TabIndex = 2;
			this.rdoDecompileSys3.Text = "System 3.0";
			this.rdoDecompileSys3.UseVisualStyleBackColor = true;
			this.rdoDecompileSys3.CheckedChanged += new System.EventHandler(this.rdoDecompileSys3_CheckedChanged);
			// 
			// rdoDecompileSys2
			// 
			this.rdoDecompileSys2.AutoSize = true;
			this.rdoDecompileSys2.Location = new System.Drawing.Point(6, 43);
			this.rdoDecompileSys2.Name = "rdoDecompileSys2";
			this.rdoDecompileSys2.Size = new System.Drawing.Size(68, 17);
			this.rdoDecompileSys2.TabIndex = 1;
			this.rdoDecompileSys2.Text = "System 2";
			this.rdoDecompileSys2.UseVisualStyleBackColor = true;
			this.rdoDecompileSys2.CheckedChanged += new System.EventHandler(this.rdoDecompileSys2_CheckedChanged);
			// 
			// rdoDecompileSys1
			// 
			this.rdoDecompileSys1.AutoSize = true;
			this.rdoDecompileSys1.Checked = true;
			this.rdoDecompileSys1.Location = new System.Drawing.Point(6, 20);
			this.rdoDecompileSys1.Name = "rdoDecompileSys1";
			this.rdoDecompileSys1.Size = new System.Drawing.Size(68, 17);
			this.rdoDecompileSys1.TabIndex = 0;
			this.rdoDecompileSys1.TabStop = true;
			this.rdoDecompileSys1.Text = "System 1";
			this.rdoDecompileSys1.UseVisualStyleBackColor = true;
			this.rdoDecompileSys1.CheckedChanged += new System.EventHandler(this.rdoDecompileSys1_CheckedChanged);
			// 
			// btnBeginDecompile
			// 
			this.btnBeginDecompile.Location = new System.Drawing.Point(381, 267);
			this.btnBeginDecompile.Name = "btnBeginDecompile";
			this.btnBeginDecompile.Size = new System.Drawing.Size(86, 23);
			this.btnBeginDecompile.TabIndex = 11;
			this.btnBeginDecompile.Text = "&Decompile";
			this.btnBeginDecompile.UseVisualStyleBackColor = true;
			this.btnBeginDecompile.Click += new System.EventHandler(this.BtnBeginDecompile_Click);
			// 
			// grpDecompileOpts
			// 
			this.grpDecompileOpts.Controls.Add(this.rdoDecompileOptVerbobjs);
			this.grpDecompileOpts.Controls.Add(this.rdoDecompileOptPages);
			this.grpDecompileOpts.Controls.Add(this.rdoDecompileOptAll);
			this.grpDecompileOpts.Location = new System.Drawing.Point(128, 64);
			this.grpDecompileOpts.Name = "grpDecompileOpts";
			this.grpDecompileOpts.Size = new System.Drawing.Size(178, 95);
			this.grpDecompileOpts.TabIndex = 8;
			this.grpDecompileOpts.TabStop = false;
			this.grpDecompileOpts.Text = "Decompile Options";
			// 
			// rdoDecompileOptVerbobjs
			// 
			this.rdoDecompileOptVerbobjs.AutoSize = true;
			this.rdoDecompileOptVerbobjs.Location = new System.Drawing.Point(7, 66);
			this.rdoDecompileOptVerbobjs.Name = "rdoDecompileOptVerbobjs";
			this.rdoDecompileOptVerbobjs.Size = new System.Drawing.Size(166, 17);
			this.rdoDecompileOptVerbobjs.TabIndex = 2;
			this.rdoDecompileOptVerbobjs.Text = "Only Verb/Options (AG00.dat)";
			this.rdoDecompileOptVerbobjs.UseVisualStyleBackColor = true;
			// 
			// rdoDecompileOptPages
			// 
			this.rdoDecompileOptPages.AutoSize = true;
			this.rdoDecompileOptPages.Location = new System.Drawing.Point(7, 43);
			this.rdoDecompileOptPages.Name = "rdoDecompileOptPages";
			this.rdoDecompileOptPages.Size = new System.Drawing.Size(135, 17);
			this.rdoDecompileOptPages.TabIndex = 1;
			this.rdoDecompileOptPages.Text = "Only Pages (*DISK.dat)";
			this.rdoDecompileOptPages.UseVisualStyleBackColor = true;
			// 
			// rdoDecompileOptAll
			// 
			this.rdoDecompileOptAll.AutoSize = true;
			this.rdoDecompileOptAll.Checked = true;
			this.rdoDecompileOptAll.Location = new System.Drawing.Point(7, 20);
			this.rdoDecompileOptAll.Name = "rdoDecompileOptAll";
			this.rdoDecompileOptAll.Size = new System.Drawing.Size(89, 17);
			this.rdoDecompileOptAll.TabIndex = 0;
			this.rdoDecompileOptAll.TabStop = true;
			this.rdoDecompileOptAll.Text = "Decompile All";
			this.rdoDecompileOptAll.UseVisualStyleBackColor = true;
			// 
			// btnDecompileTargetDir
			// 
			this.btnDecompileTargetDir.Image = ((System.Drawing.Image)(resources.GetObject("btnDecompileTargetDir.Image")));
			this.btnDecompileTargetDir.Location = new System.Drawing.Point(442, 35);
			this.btnDecompileTargetDir.Name = "btnDecompileTargetDir";
			this.btnDecompileTargetDir.Size = new System.Drawing.Size(23, 23);
			this.btnDecompileTargetDir.TabIndex = 6;
			this.btnDecompileTargetDir.UseVisualStyleBackColor = true;
			this.btnDecompileTargetDir.Click += new System.EventHandler(this.BtnDecompileTarget_Click);
			// 
			// txtDecompileTargetDir
			// 
			this.txtDecompileTargetDir.Location = new System.Drawing.Point(102, 37);
			this.txtDecompileTargetDir.Name = "txtDecompileTargetDir";
			this.txtDecompileTargetDir.Size = new System.Drawing.Size(334, 20);
			this.txtDecompileTargetDir.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Target Directory:";
			// 
			// btnDecompileSrcDir
			// 
			this.btnDecompileSrcDir.Image = ((System.Drawing.Image)(resources.GetObject("btnDecompileSrcDir.Image")));
			this.btnDecompileSrcDir.Location = new System.Drawing.Point(442, 8);
			this.btnDecompileSrcDir.Name = "btnDecompileSrcDir";
			this.btnDecompileSrcDir.Size = new System.Drawing.Size(23, 23);
			this.btnDecompileSrcDir.TabIndex = 3;
			this.btnDecompileSrcDir.UseVisualStyleBackColor = true;
			this.btnDecompileSrcDir.Click += new System.EventHandler(this.BtnDecompileSrcDir_Click);
			// 
			// txtDecompileSrcDir
			// 
			this.txtDecompileSrcDir.Location = new System.Drawing.Point(102, 10);
			this.txtDecompileSrcDir.Name = "txtDecompileSrcDir";
			this.txtDecompileSrcDir.Size = new System.Drawing.Size(334, 20);
			this.txtDecompileSrcDir.TabIndex = 2;
			// 
			// lblDecompileSrcDir
			// 
			this.lblDecompileSrcDir.AutoSize = true;
			this.lblDecompileSrcDir.Location = new System.Drawing.Point(7, 13);
			this.lblDecompileSrcDir.Name = "lblDecompileSrcDir";
			this.lblDecompileSrcDir.Size = new System.Drawing.Size(78, 13);
			this.lblDecompileSrcDir.TabIndex = 1;
			this.lblDecompileSrcDir.Text = "Data Directory:";
			// 
			// tabCompile
			// 
			this.tabCompile.Controls.Add(this.grpCOut);
			this.tabCompile.Controls.Add(this.grpCSource);
			this.tabCompile.Controls.Add(this.btnRevDecompileToCompile);
			this.tabCompile.Controls.Add(this.chkDeleteTemp);
			this.tabCompile.Controls.Add(this.grpCompileSysVer);
			this.tabCompile.Controls.Add(this.btnBeginCompile);
			this.tabCompile.Controls.Add(this.grpCompileOpts);
			this.tabCompile.Controls.Add(this.btnCompileTargetDir);
			this.tabCompile.Controls.Add(this.txtCompileTargetDir);
			this.tabCompile.Controls.Add(this.lblCompileTargetDir);
			this.tabCompile.Controls.Add(this.btnCompileSrcDir);
			this.tabCompile.Controls.Add(this.txtCompileSrcDir);
			this.tabCompile.Controls.Add(this.lblCompileSrcDir);
			this.tabCompile.Location = new System.Drawing.Point(4, 22);
			this.tabCompile.Name = "tabCompile";
			this.tabCompile.Padding = new System.Windows.Forms.Padding(3);
			this.tabCompile.Size = new System.Drawing.Size(473, 297);
			this.tabCompile.TabIndex = 1;
			this.tabCompile.Text = "Compile";
			this.tabCompile.UseVisualStyleBackColor = true;
			// 
			// grpCOut
			// 
			this.grpCOut.Controls.Add(this.rdoCompileOutUTF8);
			this.grpCOut.Controls.Add(this.rdoCompileOutShiftJIS);
			this.grpCOut.Location = new System.Drawing.Point(241, 165);
			this.grpCOut.Name = "grpCOut";
			this.grpCOut.Size = new System.Drawing.Size(225, 95);
			this.grpCOut.TabIndex = 26;
			this.grpCOut.TabStop = false;
			this.grpCOut.Text = "Game Output Encoding";
			// 
			// rdoCompileOutUTF8
			// 
			this.rdoCompileOutUTF8.AutoSize = true;
			this.rdoCompileOutUTF8.Location = new System.Drawing.Point(7, 43);
			this.rdoCompileOutUTF8.Name = "rdoCompileOutUTF8";
			this.rdoCompileOutUTF8.Size = new System.Drawing.Size(55, 17);
			this.rdoCompileOutUTF8.TabIndex = 1;
			this.rdoCompileOutUTF8.Text = "UTF-8";
			this.rdoCompileOutUTF8.UseVisualStyleBackColor = true;
			// 
			// rdoCompileOutShiftJIS
			// 
			this.rdoCompileOutShiftJIS.AutoSize = true;
			this.rdoCompileOutShiftJIS.Checked = true;
			this.rdoCompileOutShiftJIS.Location = new System.Drawing.Point(7, 20);
			this.rdoCompileOutShiftJIS.Name = "rdoCompileOutShiftJIS";
			this.rdoCompileOutShiftJIS.Size = new System.Drawing.Size(64, 17);
			this.rdoCompileOutShiftJIS.TabIndex = 0;
			this.rdoCompileOutShiftJIS.TabStop = true;
			this.rdoCompileOutShiftJIS.Text = "Shift-JIS";
			this.rdoCompileOutShiftJIS.UseVisualStyleBackColor = true;
			// 
			// grpCSource
			// 
			this.grpCSource.Controls.Add(this.rdoCompileSourceUTF8);
			this.grpCSource.Controls.Add(this.rdoCompileSourceShiftJIS);
			this.grpCSource.Location = new System.Drawing.Point(10, 165);
			this.grpCSource.Name = "grpCSource";
			this.grpCSource.Size = new System.Drawing.Size(225, 95);
			this.grpCSource.TabIndex = 25;
			this.grpCSource.TabStop = false;
			this.grpCSource.Text = "Text Source Encoding";
			// 
			// rdoCompileSourceUTF8
			// 
			this.rdoCompileSourceUTF8.AutoSize = true;
			this.rdoCompileSourceUTF8.Location = new System.Drawing.Point(7, 43);
			this.rdoCompileSourceUTF8.Name = "rdoCompileSourceUTF8";
			this.rdoCompileSourceUTF8.Size = new System.Drawing.Size(55, 17);
			this.rdoCompileSourceUTF8.TabIndex = 1;
			this.rdoCompileSourceUTF8.Text = "UTF-8";
			this.rdoCompileSourceUTF8.UseVisualStyleBackColor = true;
			this.rdoCompileSourceUTF8.CheckedChanged += new System.EventHandler(this.rdoCompileSourceUTF8_CheckedChanged);
			// 
			// rdoCompileSourceShiftJIS
			// 
			this.rdoCompileSourceShiftJIS.AutoSize = true;
			this.rdoCompileSourceShiftJIS.Checked = true;
			this.rdoCompileSourceShiftJIS.Location = new System.Drawing.Point(7, 20);
			this.rdoCompileSourceShiftJIS.Name = "rdoCompileSourceShiftJIS";
			this.rdoCompileSourceShiftJIS.Size = new System.Drawing.Size(64, 17);
			this.rdoCompileSourceShiftJIS.TabIndex = 0;
			this.rdoCompileSourceShiftJIS.TabStop = true;
			this.rdoCompileSourceShiftJIS.Text = "Shift-JIS";
			this.rdoCompileSourceShiftJIS.UseVisualStyleBackColor = true;
			// 
			// btnRevDecompileToCompile
			// 
			this.btnRevDecompileToCompile.Location = new System.Drawing.Point(10, 267);
			this.btnRevDecompileToCompile.Name = "btnRevDecompileToCompile";
			this.btnRevDecompileToCompile.Size = new System.Drawing.Size(183, 23);
			this.btnRevDecompileToCompile.TabIndex = 24;
			this.btnRevDecompileToCompile.Text = "Copy/Reverse Decompile Settings";
			this.btnRevDecompileToCompile.UseVisualStyleBackColor = true;
			this.btnRevDecompileToCompile.Click += new System.EventHandler(this.BtnRevDecompileToCompile_Click);
			// 
			// chkDeleteTemp
			// 
			this.chkDeleteTemp.AutoSize = true;
			this.chkDeleteTemp.Checked = true;
			this.chkDeleteTemp.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkDeleteTemp.Location = new System.Drawing.Point(241, 271);
			this.chkDeleteTemp.Name = "chkDeleteTemp";
			this.chkDeleteTemp.Size = new System.Drawing.Size(134, 17);
			this.chkDeleteTemp.TabIndex = 23;
			this.chkDeleteTemp.Text = "Delete Temporary Files";
			this.chkDeleteTemp.UseVisualStyleBackColor = true;
			// 
			// grpCompileSysVer
			// 
			this.grpCompileSysVer.Controls.Add(this.rdoCompileSys3);
			this.grpCompileSysVer.Controls.Add(this.rdoCompileSys2);
			this.grpCompileSysVer.Controls.Add(this.rdoCompileSys1);
			this.grpCompileSysVer.Location = new System.Drawing.Point(10, 64);
			this.grpCompileSysVer.Name = "grpCompileSysVer";
			this.grpCompileSysVer.Size = new System.Drawing.Size(225, 95);
			this.grpCompileSysVer.TabIndex = 19;
			this.grpCompileSysVer.TabStop = false;
			this.grpCompileSysVer.Text = "System Version";
			// 
			// rdoCompileSys3
			// 
			this.rdoCompileSys3.AutoSize = true;
			this.rdoCompileSys3.Location = new System.Drawing.Point(6, 66);
			this.rdoCompileSys3.Name = "rdoCompileSys3";
			this.rdoCompileSys3.Size = new System.Drawing.Size(77, 17);
			this.rdoCompileSys3.TabIndex = 2;
			this.rdoCompileSys3.Text = "System 3.0";
			this.rdoCompileSys3.UseVisualStyleBackColor = true;
			// 
			// rdoCompileSys2
			// 
			this.rdoCompileSys2.AutoSize = true;
			this.rdoCompileSys2.Location = new System.Drawing.Point(6, 43);
			this.rdoCompileSys2.Name = "rdoCompileSys2";
			this.rdoCompileSys2.Size = new System.Drawing.Size(68, 17);
			this.rdoCompileSys2.TabIndex = 1;
			this.rdoCompileSys2.Text = "System 2";
			this.rdoCompileSys2.UseVisualStyleBackColor = true;
			// 
			// rdoCompileSys1
			// 
			this.rdoCompileSys1.AutoSize = true;
			this.rdoCompileSys1.Checked = true;
			this.rdoCompileSys1.Location = new System.Drawing.Point(6, 20);
			this.rdoCompileSys1.Name = "rdoCompileSys1";
			this.rdoCompileSys1.Size = new System.Drawing.Size(68, 17);
			this.rdoCompileSys1.TabIndex = 0;
			this.rdoCompileSys1.TabStop = true;
			this.rdoCompileSys1.Text = "System 1";
			this.rdoCompileSys1.UseVisualStyleBackColor = true;
			// 
			// btnBeginCompile
			// 
			this.btnBeginCompile.Location = new System.Drawing.Point(379, 267);
			this.btnBeginCompile.Name = "btnBeginCompile";
			this.btnBeginCompile.Size = new System.Drawing.Size(86, 23);
			this.btnBeginCompile.TabIndex = 22;
			this.btnBeginCompile.Text = "&Compile";
			this.btnBeginCompile.UseVisualStyleBackColor = true;
			this.btnBeginCompile.Click += new System.EventHandler(this.BtnBeginCompile_Click);
			// 
			// grpCompileOpts
			// 
			this.grpCompileOpts.Controls.Add(this.rdoCompileOptVerbobjs);
			this.grpCompileOpts.Controls.Add(this.rdoCompileOptPages);
			this.grpCompileOpts.Controls.Add(this.rdoCompileOptAll);
			this.grpCompileOpts.Location = new System.Drawing.Point(241, 64);
			this.grpCompileOpts.Name = "grpCompileOpts";
			this.grpCompileOpts.Size = new System.Drawing.Size(225, 95);
			this.grpCompileOpts.TabIndex = 18;
			this.grpCompileOpts.TabStop = false;
			this.grpCompileOpts.Text = "Compile Options";
			// 
			// rdoCompileOptVerbobjs
			// 
			this.rdoCompileOptVerbobjs.AutoSize = true;
			this.rdoCompileOptVerbobjs.Location = new System.Drawing.Point(7, 66);
			this.rdoCompileOptVerbobjs.Name = "rdoCompileOptVerbobjs";
			this.rdoCompileOptVerbobjs.Size = new System.Drawing.Size(166, 17);
			this.rdoCompileOptVerbobjs.TabIndex = 2;
			this.rdoCompileOptVerbobjs.Text = "Only Verb/Options (AG00.dat)";
			this.rdoCompileOptVerbobjs.UseVisualStyleBackColor = true;
			// 
			// rdoCompileOptPages
			// 
			this.rdoCompileOptPages.AutoSize = true;
			this.rdoCompileOptPages.Location = new System.Drawing.Point(7, 43);
			this.rdoCompileOptPages.Name = "rdoCompileOptPages";
			this.rdoCompileOptPages.Size = new System.Drawing.Size(135, 17);
			this.rdoCompileOptPages.TabIndex = 1;
			this.rdoCompileOptPages.Text = "Only Pages (*DISK.dat)";
			this.rdoCompileOptPages.UseVisualStyleBackColor = true;
			// 
			// rdoCompileOptAll
			// 
			this.rdoCompileOptAll.AutoSize = true;
			this.rdoCompileOptAll.Checked = true;
			this.rdoCompileOptAll.Location = new System.Drawing.Point(7, 20);
			this.rdoCompileOptAll.Name = "rdoCompileOptAll";
			this.rdoCompileOptAll.Size = new System.Drawing.Size(76, 17);
			this.rdoCompileOptAll.TabIndex = 0;
			this.rdoCompileOptAll.TabStop = true;
			this.rdoCompileOptAll.Text = "Compile All";
			this.rdoCompileOptAll.UseVisualStyleBackColor = true;
			// 
			// btnCompileTargetDir
			// 
			this.btnCompileTargetDir.Image = ((System.Drawing.Image)(resources.GetObject("btnCompileTargetDir.Image")));
			this.btnCompileTargetDir.Location = new System.Drawing.Point(442, 35);
			this.btnCompileTargetDir.Name = "btnCompileTargetDir";
			this.btnCompileTargetDir.Size = new System.Drawing.Size(23, 23);
			this.btnCompileTargetDir.TabIndex = 17;
			this.btnCompileTargetDir.UseVisualStyleBackColor = true;
			this.btnCompileTargetDir.Click += new System.EventHandler(this.BtnCompileTargetDir_Click);
			// 
			// txtCompileTargetDir
			// 
			this.txtCompileTargetDir.Location = new System.Drawing.Point(102, 37);
			this.txtCompileTargetDir.Name = "txtCompileTargetDir";
			this.txtCompileTargetDir.Size = new System.Drawing.Size(334, 20);
			this.txtCompileTargetDir.TabIndex = 16;
			// 
			// lblCompileTargetDir
			// 
			this.lblCompileTargetDir.AutoSize = true;
			this.lblCompileTargetDir.Location = new System.Drawing.Point(7, 40);
			this.lblCompileTargetDir.Name = "lblCompileTargetDir";
			this.lblCompileTargetDir.Size = new System.Drawing.Size(86, 13);
			this.lblCompileTargetDir.TabIndex = 15;
			this.lblCompileTargetDir.Text = "Target Directory:";
			// 
			// btnCompileSrcDir
			// 
			this.btnCompileSrcDir.Image = ((System.Drawing.Image)(resources.GetObject("btnCompileSrcDir.Image")));
			this.btnCompileSrcDir.Location = new System.Drawing.Point(442, 8);
			this.btnCompileSrcDir.Name = "btnCompileSrcDir";
			this.btnCompileSrcDir.Size = new System.Drawing.Size(23, 23);
			this.btnCompileSrcDir.TabIndex = 14;
			this.btnCompileSrcDir.UseVisualStyleBackColor = true;
			this.btnCompileSrcDir.Click += new System.EventHandler(this.BtnCompileSrcDir_Click);
			// 
			// txtCompileSrcDir
			// 
			this.txtCompileSrcDir.Location = new System.Drawing.Point(102, 10);
			this.txtCompileSrcDir.Name = "txtCompileSrcDir";
			this.txtCompileSrcDir.Size = new System.Drawing.Size(334, 20);
			this.txtCompileSrcDir.TabIndex = 13;
			// 
			// lblCompileSrcDir
			// 
			this.lblCompileSrcDir.AutoSize = true;
			this.lblCompileSrcDir.Location = new System.Drawing.Point(7, 13);
			this.lblCompileSrcDir.Name = "lblCompileSrcDir";
			this.lblCompileSrcDir.Size = new System.Drawing.Size(89, 13);
			this.lblCompileSrcDir.TabIndex = 12;
			this.lblCompileSrcDir.Text = "Source Directory:";
			// 
			// DecompilerForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 351);
			this.Controls.Add(this.tabCtrlMain);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "DecompilerForm";
			this.Text = "Sys0Decompiler";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DecompilerForm_FormClosing);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabCtrlMain.ResumeLayout(false);
			this.tabDecompile.ResumeLayout(false);
			this.tabDecompile.PerformLayout();
			this.grpVarFormat.ResumeLayout(false);
			this.grpVarFormat.PerformLayout();
			this.grpDecompileOut.ResumeLayout(false);
			this.grpDecompileOut.PerformLayout();
			this.grpDecompileSource.ResumeLayout(false);
			this.grpDecompileSource.PerformLayout();
			this.grpTextMode.ResumeLayout(false);
			this.grpTextMode.PerformLayout();
			this.grpDecompileSysVer.ResumeLayout(false);
			this.grpDecompileSysVer.PerformLayout();
			this.grpDecompileOpts.ResumeLayout(false);
			this.grpDecompileOpts.PerformLayout();
			this.tabCompile.ResumeLayout(false);
			this.tabCompile.PerformLayout();
			this.grpCOut.ResumeLayout(false);
			this.grpCOut.PerformLayout();
			this.grpCSource.ResumeLayout(false);
			this.grpCSource.PerformLayout();
			this.grpCompileSysVer.ResumeLayout(false);
			this.grpCompileSysVer.PerformLayout();
			this.grpCompileOpts.ResumeLayout(false);
			this.grpCompileOpts.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem tlsFileMenu;
		private System.Windows.Forms.ToolStripMenuItem tlsExit;
		private System.Windows.Forms.ToolStripMenuItem tlsHelpMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem tlsAbout;
		private System.Windows.Forms.TabControl tabCtrlMain;
		private System.Windows.Forms.TabPage tabDecompile;
		private System.Windows.Forms.Button btnBeginDecompile;
		private System.Windows.Forms.GroupBox grpDecompileOpts;
		private System.Windows.Forms.RadioButton rdoDecompileOptVerbobjs;
		private System.Windows.Forms.RadioButton rdoDecompileOptPages;
		private System.Windows.Forms.RadioButton rdoDecompileOptAll;
		private System.Windows.Forms.Button btnDecompileTargetDir;
		private System.Windows.Forms.TextBox txtDecompileTargetDir;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnDecompileSrcDir;
		private System.Windows.Forms.TextBox txtDecompileSrcDir;
		private System.Windows.Forms.Label lblDecompileSrcDir;
		private System.Windows.Forms.TabPage tabCompile;
		private System.Windows.Forms.GroupBox grpDecompileSysVer;
		private System.Windows.Forms.RadioButton rdoDecompileSys3;
		private System.Windows.Forms.RadioButton rdoDecompileSys2;
		private System.Windows.Forms.RadioButton rdoDecompileSys1;
		private System.Windows.Forms.GroupBox grpCompileSysVer;
		private System.Windows.Forms.RadioButton rdoCompileSys3;
		private System.Windows.Forms.RadioButton rdoCompileSys2;
		private System.Windows.Forms.RadioButton rdoCompileSys1;
		private System.Windows.Forms.Button btnBeginCompile;
		private System.Windows.Forms.GroupBox grpCompileOpts;
		private System.Windows.Forms.RadioButton rdoCompileOptVerbobjs;
		private System.Windows.Forms.RadioButton rdoCompileOptPages;
		private System.Windows.Forms.RadioButton rdoCompileOptAll;
		private System.Windows.Forms.Button btnCompileTargetDir;
		private System.Windows.Forms.TextBox txtCompileTargetDir;
		private System.Windows.Forms.Label lblCompileTargetDir;
		private System.Windows.Forms.Button btnCompileSrcDir;
		private System.Windows.Forms.TextBox txtCompileSrcDir;
		private System.Windows.Forms.Label lblCompileSrcDir;
		private System.Windows.Forms.CheckBox chkDeleteTemp;
		private System.Windows.Forms.ToolStripMenuItem tlsGeneralHelp;
		private System.Windows.Forms.ToolStripMenuItem tlsExportSettings;
		private System.Windows.Forms.ToolStripMenuItem tlsImportSettings;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Button btnRevCompileToDecompile;
		private System.Windows.Forms.Button btnRevDecompileToCompile;
		private System.Windows.Forms.ToolStripMenuItem tlsSavePref;
		private System.Windows.Forms.GroupBox grpTextMode;
		private System.Windows.Forms.RadioButton rdoRaw;
		private System.Windows.Forms.RadioButton rdoKatakana;
		private System.Windows.Forms.RadioButton rdoHiragana;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem tlsAdvanced;
		private System.Windows.Forms.ToolStripMenuItem tlsOutputJunk;
		private System.Windows.Forms.ToolStripMenuItem tlsDCmd;
		private System.Windows.Forms.GroupBox grpDecompileSource;
		private System.Windows.Forms.RadioButton rdoDecompileSourceMSX;
		private System.Windows.Forms.RadioButton rdoDecompileSourceShiftJIS;
		private System.Windows.Forms.ToolStripMenuItem tlsDiacritic;
		private System.Windows.Forms.RadioButton rdoDecompileSourceUTF8;
		private System.Windows.Forms.GroupBox grpCOut;
		private System.Windows.Forms.RadioButton rdoCompileOutUTF8;
		private System.Windows.Forms.RadioButton rdoCompileOutShiftJIS;
		private System.Windows.Forms.GroupBox grpCSource;
		private System.Windows.Forms.RadioButton rdoCompileSourceUTF8;
		private System.Windows.Forms.RadioButton rdoCompileSourceShiftJIS;
		private System.Windows.Forms.GroupBox grpDecompileOut;
		private System.Windows.Forms.RadioButton rdoDecompileOutUTF8;
		private System.Windows.Forms.RadioButton rdoDecompileOutShiftJIS;
		private System.Windows.Forms.GroupBox grpVarFormat;
		private System.Windows.Forms.RadioButton rdoVAROnly;
		private System.Windows.Forms.RadioButton rdoFullLabel;
	}
}

