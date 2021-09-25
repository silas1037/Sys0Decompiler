using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Sys0Decompiler
{
	/// <summary>
	/// The main form which contains a file explorer
	/// </summary>
	public partial class DecompilerForm : Form
	{

		private const string DIR_SCO = "sco";
		public const string FILE_AG00 = "AG00.DAT";
		public const string FILE_ADISK = "ADISK.DAT";
		public const string FILE_VERBS = "!verbs.adv";
		public const string FILE_OBJECTS = "!objects.adv";

		private SystemVersion decompileSystemVersion;
		private SystemVersion compileSystemVersion;

		private string sessionSaveFile = "sessionSave.pref";

		public enum SJISTextMode
		{
			Raw,
			Hiragana,
			Katakana
		}
		public SJISTextMode CurJPNTextMode { get; set; }

		// Because MSX encoding isn't a standard C# encoding, we have to check across two steps: which mode
		// are we in, and if we're in a standard mode, which SourceEncoding object should we use. During MSX
		// mode, the SourceEncoding object should never be used.
		public enum SourceEncodingMode
		{
			ShiftJIS,
			UTF8,
			MSX
		}
		public SourceEncodingMode CurDecompileSourceMode { get; set; }
		public Encoding DecompileSourceEncoding { get; set; }
		public Encoding DecompileOutputEncoding { get; set; }


		// While we cannot compile to MSX, we maintain the enum for internal consistency.
		public SourceEncodingMode CurCompileOutMode { get; set; }
		public Encoding CompileSourceEncoding { get; set; }
		public Encoding CompileOutputEncoding { get; set; }

		/// <summary>
		/// The set of loaded archive files, must stay in sync with fileOperations.loadedArchiveFiles
		/// </summary>
		ArchiveFileCollection loadedArchiveFiles = null;
		/// <summary>
		/// The file operations object, fileOperations.loadedArchiveFiles must stay in sync with this.loadedArchiveFiles
		/// </summary>
		FileOperations fileOperations = new FileOperations();

		public DecompilerForm()
		{
			InitializeComponent();

			LoadPreferences(sessionSaveFile);
		}

		private void StartProgressForm()
		{
			this.ReportProgressListener += new EventHandler<ReportProgressEventArgs>(ReportProgress);
		}

		private void CloseProgressForm()
		{
			this.ReportProgressListener -= ReportProgress;
			if(this.Enabled == false)
			{
				this.Enabled = true;
				if(this.progressForm != null && !this.progressForm.IsDisposed)
				{
					this.progressForm.Dispose();
					this.progressForm = null;
				}
			}
		}

		ErrorListForm errorListForm = null;

		void UpdateErrorWindow()
		{
			Error(this, new ErrorEventArgs(new Exception("")));
		}

		void Error(object sender, ErrorEventArgs e)
		{
			string errorMessage = e.GetException().Message;
			if(errorListForm == null && !String.IsNullOrEmpty(errorMessage))
			{
				this.errorListForm = new ErrorListForm();
				this.errorListForm.Disposed += new EventHandler(errorListForm_Disposed);
				this.errorListForm.Show();
			}
			if(errorListForm != null)
			{
				this.errorListForm.DisplayMessage(errorMessage);
			}
		}

		void errorListForm_Disposed(object sender, EventArgs e)
		{
			this.errorListForm.Disposed -= errorListForm_Disposed;
			this.errorListForm = null;
		}

		ProgressForm progressForm = null;

		void ReportProgress(object sender, ReportProgressEventArgs e)
		{
			if(this.progressForm == null)
			{
				if(this.Enabled == true)
				{
					this.Enabled = false;
				}
				this.progressForm = new ProgressForm();
				this.progressForm.StartPosition = FormStartPosition.CenterParent;
				this.progressForm.Show();
			}
			this.progressForm.Text = "Progress";
			this.progressForm.label1.Text = e.Message;
			if(e.Value <= 100)
			{
				this.progressForm.progressBar1.Value = e.Value;
			}
			else
			{
				this.progressForm.progressBar1.Value = 100;
			}
			Application.DoEvents();
		}

		public class ReportProgressEventArgs : EventArgs
		{
			public int Value { get; set; }
			public string Message { get; set; }
			public ReportProgressEventArgs(int value, string message)
			{
				this.Value = value;
				this.Message = message;
			}
		}

		public event EventHandler<ReportProgressEventArgs> ReportProgressListener;

		public void OnReportProgress(int value, string message)
		{
			if(this.ReportProgressListener != null)
			{
				this.ReportProgress(this, new ReportProgressEventArgs(value, message));
			}
		}

		internal void OnError(ErrorEventArgs errorEventArgs)
		{
			this.Error(this, errorEventArgs);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void DecompilerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			SavePreferences(sessionSaveFile);
		}

		private void ExportToolstrip_Click(object sender, EventArgs e)
		{
			string fileName = "";

			using(var saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Filter = "Preferences Files (*.pref)|*.pref|All Files (*.*)|*.*";
				if(saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					fileName = saveFileDialog.FileName;
				}
			}

			if(String.IsNullOrEmpty(fileName))
			{
				return;
			}

			SavePreferences(fileName);
		}

		private void ImportSettingsToolstrip_Click(object sender, EventArgs e)
		{
			string fileName = "";

			using(var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "Preferences Files (*.pref)|*.pref|All Files (*.*)|*.*";
				openFileDialog.FileName = "settings.pref";
				if(openFileDialog.ShowDialog() == DialogResult.OK)
				{
					fileName = openFileDialog.FileName;
				}
			}

			if(String.IsNullOrEmpty(fileName))
			{
				return;
			}

			LoadPreferences(fileName);
		}

		private void SavePreferences(string fileName)
		{
			List<string> saveData = new List<string> {
				tabCtrlMain.SelectedIndex.ToString(), txtDecompileSrcDir.Text, txtDecompileTargetDir.Text,
				txtCompileSrcDir.Text, txtCompileTargetDir.Text, Convert.ToString(tlsOutputJunk.Checked),
				Convert.ToString(tlsDCmd.Checked) };

			// Decompile radio buttons.
			// System Version.
			if(rdoDecompileSys1.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoDecompileSys2.Checked)
			{
				saveData.Add("2");
			}
			else if(rdoDecompileSys3.Checked)
			{
				saveData.Add("3");
			}
			else
			{
				// Error. Set to default instead.
				saveData.Add("0");
			}

			// Decompile pages/verbobjs.
			if(rdoDecompileOptAll.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoDecompileOptPages.Checked)
			{
				saveData.Add("2");
			}
			else if(rdoDecompileOptVerbobjs.Checked)
			{
				saveData.Add("3");
			}
			else
			{
				saveData.Add("0");
			}

			// Decompile source encoding.
			if(rdoDecompileSourceShiftJIS.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoDecompileSourceUTF8.Checked)
			{
				saveData.Add("2");
			}
			else if(rdoDecompileSourceMSX.Checked)
			{
				saveData.Add("3");
			}
			else
			{
				saveData.Add("0");
			}

			// Decompile output encoding.
			if(rdoDecompileOutShiftJIS.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoDecompileOutUTF8.Checked)
			{
				saveData.Add("2");
			}
			else
			{
				saveData.Add("0");
			}

			// Decompile charset
			if(rdoHiragana.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoKatakana.Checked)
			{
				saveData.Add("2");
			}
			else if(rdoRaw.Checked)
			{
				saveData.Add("3");
			}
			else
			{
				saveData.Add("0");
			}


			// Compile radio buttons.
			// System Version.
			if(rdoCompileSys1.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoCompileSys2.Checked)
			{
				saveData.Add("2");
			}
			else if(rdoCompileSys3.Checked)
			{
				saveData.Add("3");
			}
			else
			{
				saveData.Add("0");
			}

			// Compile pages/verbobjs.
			if(rdoCompileOptAll.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoCompileOptPages.Checked)
			{
				saveData.Add("2");
			}
			else if(rdoCompileOptVerbobjs.Checked)
			{
				saveData.Add("3");
			}
			else
			{
				// Error. Set to default instead.
				saveData.Add("0");
			}

			// Compile source encoding.
			if(rdoCompileSourceShiftJIS.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoCompileSourceUTF8.Checked)
			{
				saveData.Add("2");
			}
			else
			{
				saveData.Add("0");
			}

			// Compile output encoding.
			if(rdoCompileOutShiftJIS.Checked)
			{
				saveData.Add("1");
			}
			else if(rdoCompileOutUTF8.Checked)
			{
				saveData.Add("2");
			}
			else
			{
				saveData.Add("0");
			}

			// Write save file.
			File.WriteAllLines(fileName, saveData);
		}

		private void LoadPreferences(string fileName)
		{
			if(File.Exists(sessionSaveFile))
			{
				string[] vars = File.ReadAllLines(fileName);

				if(vars.Length == 16)
				{
					int tabIndex;
					bool indexCorrect = Int32.TryParse(vars[0], out tabIndex);

					if(indexCorrect) tabCtrlMain.SelectedIndex = tabIndex;
					else MessageBox.Show("Last session tab index invalid.");

					txtDecompileSrcDir.Text = vars[1];
					txtDecompileTargetDir.Text = vars[2];
					txtCompileSrcDir.Text = vars[3];
					txtCompileTargetDir.Text = vars[4];

					try
					{
						tlsOutputJunk.Checked = Convert.ToBoolean(vars[5]);
						tlsDCmd.Checked = Convert.ToBoolean(vars[6]);
					}
					catch(System.FormatException ex)
					{
						MessageBox.Show("One or more loaded menu item preferences invalid.");
					}

					// Decompile radio buttons. Ignore 0s.
					// System Version
					if(vars[7] == "1")
					{
						rdoDecompileSys1.Checked = true;
					}
					else if(vars[7] == "2")
					{
						rdoDecompileSys2.Checked = true;
					}
					else if(vars[7] == "3")
					{
						rdoDecompileSys3.Checked = true;
					}

					// Decompile pages/verbobjs.
					if(vars[8] == "1")
					{
						rdoDecompileOptAll.Checked = true;
					}
					else if(vars[8] == "2")
					{
						rdoDecompileOptPages.Checked = true;
					}
					else if(vars[8] == "3")
					{
						rdoDecompileOptVerbobjs.Checked = true;
					}

					// Decompile source encoding.
					if(vars[9] == "1")
					{
						rdoDecompileSourceShiftJIS.Checked = true;
					}
					else if(vars[9] == "2")
					{
						rdoDecompileSourceUTF8.Checked = true;
					}
					else if(vars[9] == "3")
					{
						rdoDecompileSourceMSX.Checked = true;
					}
					DecompileSourceUpdate();

					// Decompile output encoding.
					if(vars[10] == "1")
					{
						rdoDecompileOutShiftJIS.Checked = true;
					}
					else if(vars[10] == "2")
					{
						rdoDecompileOutUTF8.Checked = true;
					}

					// Decompile charset.
					if(vars[11] == "1")
					{
						rdoHiragana.Checked = true;
					}
					else if(vars[11] == "2")
					{
						rdoKatakana.Checked = true;
					}
					else if(vars[11] == "3")
					{
						rdoRaw.Checked = true;
					}


					// Compile radio buttons.
					// System Version
					if(vars[12] == "1")
					{
						rdoCompileSys1.Checked = true;
					}
					else if(vars[12] == "2")
					{
						rdoCompileSys2.Checked = true;
					}
					else if(vars[12] == "3")
					{
						rdoCompileSys3.Checked = true;
					}

					// Compile pages/verbobjs.
					if(vars[13] == "1")
					{
						rdoCompileOptAll.Checked = true;
					}
					else if(vars[13] == "2")
					{
						rdoCompileOptPages.Checked = true;
					}
					else if(vars[13] == "3")
					{
						rdoCompileOptVerbobjs.Checked = true;
					}

					// Compile source encoding.
					if(vars[14] == "1")
					{
						rdoCompileSourceShiftJIS.Checked = true;
						rdoCompileOutShiftJIS.Enabled = true;
					}
					else if(vars[14] == "2")
					{
						rdoCompileSourceUTF8.Checked = true;
						rdoCompileOutShiftJIS.Enabled = false;
					}

					// Compile output encoding.
					if(vars[15] == "1")
					{
						rdoCompileOutShiftJIS.Checked = true;
					}
					else if(vars[15] == "2")
					{
						rdoCompileOutUTF8.Checked = true;
					}

					ChangeTab();
				}
				else
				{
					MessageBox.Show("Last session data invalid.", Application.ProductName);
				}
			}
		}

		private void GeneralHelpToolstrip_Click(object sender, EventArgs e)
		{
			MessageBox.Show("This program is used to decompile Alicesoft System 1, System 2, and System 3.0 data " +
				"files, and can compile them into a new format for localization. For System 3.5 games, see " +
				"xsys3c by kichikuou or Sys3Decompiler by SLC.\n\nOnce compiled, source can only be " +
				"played by specialized executibles, such as system3-sdl2 by kichikuou or SysEng by " +
				"RottenBlock. Original Alicesoft executibles will not be able to run modified code." +
				"\n\nSee the included manual, Localization Guide, and Troublshooting Guide for more " +
				"details.", "General Help");
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Created by RottenBlock. UTF-8 Compiler created by kichikuou\n\n" +
				"This program could not have been completed without the open-source efforts of Takeda" +
				"Toshiya, creator of the original 32-bit System 1-3.0 interpreters, SomeLoliCatgirl, " +
				"creator of ALDExplorer and Sys3Decompiler; and kichikuou, creator of system3-sdl2 and" +
				"other AliceSoft tools.\n\nThis program is open-source and distributed under the GNU " +
				"Public License 2.0.");
		}

		private void SetDecompileSystemVersion()
		{
			if(rdoDecompileSys1.Checked)
			{
				decompileSystemVersion = new System1(this);
			}
			else if(rdoDecompileSys2.Checked)
			{
				decompileSystemVersion = new System2(this);
			}
			else if(rdoDecompileSys3.Checked)
			{
				decompileSystemVersion = new System3(this);
			}
		}

		private void SetCompileSystemVersion()
		{
			if(rdoCompileSys1.Checked)
			{
				compileSystemVersion = new System1(this);
			}
			else if(rdoCompileSys2.Checked)
			{
				compileSystemVersion = new System2(this);
			}
			else if(rdoCompileSys3.Checked)
			{
				compileSystemVersion = new System3(this);
			}
		}


		private void CompileMain()
		{
			SetCompileSystemVersion();
			compileSystemVersion.Initialize();
			compileSystemVersion.DecompileMode = SystemVersion.DecompileModeType.NotDecompiling;

			if(rdoCompileSourceShiftJIS.Checked)
			{
				CompileSourceEncoding = Encoding.GetEncoding(932);
			}
			else
			{
				CompileSourceEncoding = Encoding.GetEncoding("utf-8");
			}

			if(rdoCompileOutShiftJIS.Checked)
			{
				CompileOutputEncoding = Encoding.GetEncoding(932);
				CurCompileOutMode = SourceEncodingMode.ShiftJIS;
			}
			else
			{
				CompileOutputEncoding = new System.Text.UTF8Encoding(false);  // UTF-8 without BOM.
				CurCompileOutMode = SourceEncodingMode.UTF8;
			}

			string directoryName = txtCompileSrcDir.Text;

			if(!Directory.Exists(directoryName))
			{
				MessageBox.Show("Source directory does not exist.");
				return;
			}

			// Create the output directory if necessary.
			System.IO.Directory.CreateDirectory(txtCompileTargetDir.Text);

			string scoDir = Path.Combine(directoryName, DIR_SCO);

			StartProgressForm();
			try
			{
				// Start with the AG00 file, which lists verbs and objects.
				string a00File = Path.Combine(txtCompileTargetDir.Text, FILE_AG00);
				List<string> lines = new List<string>();
				lines.Add(""); // Empty line for later.
				int verbCount = 0, objectCount = 0;

				OnReportProgress(0, "Finding verbs and objects...");

				// Confirm the verbs and objects files. Note that there won't be any errors if there aren't any verb
				// or object files, I want the programmer to be free to compile with or without them. 
				// 
				// Encoding page 932 is Shift-JIS, and must be used on both read and write.
				if(File.Exists((Path.Combine(directoryName, FILE_VERBS))))
				{
					lines.AddRange(File.ReadAllLines(directoryName + Path.DirectorySeparatorChar + FILE_VERBS,
						CompileSourceEncoding));
					verbCount = lines.Count - 1;
				}
				if(File.Exists((Path.Combine(directoryName, FILE_OBJECTS))))
				{
					lines.AddRange(File.ReadAllLines(directoryName + Path.DirectorySeparatorChar + FILE_OBJECTS,
						CompileSourceEncoding));
					if(lines[lines.Count - 1] == "") lines.RemoveAt(lines.Count - 1);
					objectCount = lines.Count - verbCount - 1;
				}

				if(lines.Count > 1)
				{
					OnReportProgress(0, "Compiling verbs and objects...");

					// Start the output file with its opening line, containing the vector sizes and two unknown and 
					// probably unused variables that we will set to 0.
					lines[0] = "0," + verbCount + "," + objectCount + ",0";
					File.WriteAllLines(a00File, lines, CompileOutputEncoding);
				}


				// Page file compliation.
				// Sort the file names in order to make sure we have them all present. 
				List<string> pageFiles = Directory
					.EnumerateFiles(directoryName)
					.Where(file => (file.Length > 12 && file.ToLower().IndexOf("page") == file.Length - 12) &&
						file.ToLower().EndsWith(".adv"))
					.ToList();
				pageFiles.Sort();

				int curId = 0, index = 0;
				bool validInt, anyFilesMissing;

				compileSystemVersion.StartCompile();

				foreach(string file in pageFiles)
				{
					OnReportProgress(Convert.ToInt32((Convert.ToDouble(curId) / Convert.ToDouble(pageFiles.Count)) * 100),
						"Compiling pages...");

					// Double-check that the files match the current index.
					validInt = Int32.TryParse(file.Substring(file.Length - 8, 4), out index);

					anyFilesMissing = true;
					if(validInt)
					{
						if(index == curId)
						{
							anyFilesMissing = false;
						}
					}

					if(anyFilesMissing)
					{
						MessageBox.Show("Missing page number " + curId + ". Page files must start at 0 and have " +
							"sequential numbers.");
						return;
					}
					curId++;
				}

				System.IO.Directory.CreateDirectory(scoDir);

				bool success = false;
				string outputFile;
				curId = 0;
				foreach(string file in pageFiles)
				{
					outputFile = Path.Combine(scoDir, "dis" + (curId + 1).ToString("0000") + ".sco");

					// Can we do anything to attach a tag to the first page, which will identify it as using the
					// revised format, showing which decompile mehtod to use and preventing it from being played
					// in the old EXEs?

					success = compileSystemVersion.CompilePage(file, outputFile + ".tmp");

					if(!success)
					{
						compileSystemVersion.CloseOutputFile();
						File.Delete(outputFile + ".tmp");
						return;
					}

					// Delete the old SCO file if it exists.
					if(File.Exists(outputFile))
						File.Delete(outputFile);

					// Rename the temp file.
					File.Move(outputFile + ".tmp", outputFile);

					curId++;
				}


				// Send to DAT file.
				// Create a new DAT file.
				fileOperations.NewFile(ArchiveFileType.DatFile, "ADISK.DAT");
				this.loadedArchiveFiles = fileOperations.loadedArchiveFiles;

				// Import all SCO files from the output folder.
				List<string> scoFiles = Directory
					.EnumerateFiles(scoDir)
					.Where(file => (file.Length > 12 && file.ToLower().IndexOf("dis") == file.Length - 11) &&
						file.ToLower().EndsWith(".sco"))
					.ToList();

				// Load files
				fileOperations.ImportNewFiles(loadedArchiveFiles.ArchiveFiles[0], scoFiles.AsEnumerable(),
					scoDir, false);


				fileOperations.SaveAs(Path.Combine(txtCompileTargetDir.Text, "ADISK.DAT"));
			}
			finally
			{
				// Delete temporary files.
				if(chkDeleteTemp.Checked)
				{
					OnReportProgress(0, "Deleting temporary files.");

					if(Directory.Exists(scoDir))
					{
						DirectoryInfo di = new DirectoryInfo(scoDir);
						foreach(FileInfo file in di.GetFiles())
						{
							file.Delete();
						}
						di.Delete();
					}
				}

				CloseProgressForm();
			}
		}

		private void DecompileMain()
		{
			if(rdoRaw.Checked) CurJPNTextMode = SJISTextMode.Raw;
			else if(rdoHiragana.Checked) CurJPNTextMode = SJISTextMode.Hiragana;
			else CurJPNTextMode = SJISTextMode.Katakana;

			if(rdoDecompileSourceShiftJIS.Checked)
			{
				CurDecompileSourceMode = SourceEncodingMode.ShiftJIS;
				DecompileSourceEncoding = Encoding.GetEncoding(932);
			}
			else if(rdoDecompileSourceUTF8.Checked)
			{
				CurDecompileSourceMode = SourceEncodingMode.UTF8;
				DecompileSourceEncoding = Encoding.GetEncoding("utf-8");
			}
			else
			{
				CurDecompileSourceMode = SourceEncodingMode.MSX;
				DecompileSourceEncoding = null;
			}

			if(rdoDecompileOutShiftJIS.Checked)
			{
				DecompileOutputEncoding = Encoding.GetEncoding(932);
			}
			else
			{
				DecompileOutputEncoding = new System.Text.UTF8Encoding(false);  // UTF-8 without BOM.
			}

			SetDecompileSystemVersion();
			decompileSystemVersion.Initialize();

			// Create the output directory if necessary.
			System.IO.Directory.CreateDirectory(txtDecompileTargetDir.Text);

			if(!Directory.Exists(txtDecompileSrcDir.Text))
			{
				MessageBox.Show("Source directory does not exist.");
				return;
			}

			StartProgressForm();

			if(rdoDecompileOptAll.Checked || rdoDecompileOptVerbobjs.Checked)
			{
				DecompileAG00(txtDecompileSrcDir.Text, txtDecompileTargetDir.Text);
			}
			if(rdoDecompileOptAll.Checked || rdoDecompileOptPages.Checked)
			{
				DecompileDisk(txtDecompileSrcDir.Text, txtDecompileTargetDir.Text);
			}

			CloseProgressForm();
		}

		private void DecompileAG00(string directoryName, string codeDirectoryName)
		{
			OnReportProgress(0, "Decompiling AG00 (Verbs and Objects).");

			decompileSystemVersion.DecompileMode = SystemVersion.DecompileModeType.AG00;

			decompileSystemVersion.DecompileAG00(directoryName, codeDirectoryName);
		}


		private void DecompileDisk(string directoryName, string codeDirectoryName)
		{
			bool loaded = decompileSystemVersion.SetDecompileDisk(Path.Combine(directoryName, FILE_ADISK), 1);

			if(!loaded) return;

			bool fatal_error = false;

			int curPage = -1;

			while(!fatal_error)
			{
				curPage++;

				//if(curPage != 4)
				fatal_error = !decompileSystemVersion.DecompilePage(codeDirectoryName, curPage);
			}

			decompileSystemVersion.CloseDecompileDisk();
		}

		private void BtnDecompileSrcDir_Click(object sender, EventArgs e)
		{
			string directoryName = "";

			using(var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "DAT Files (*.dat)|*.dat|All Files (*.*)|*.*";
				openFileDialog.CheckFileExists = false;
				openFileDialog.CheckPathExists = true;
				openFileDialog.FileName = "PICK A DIRECTORY TO IMPORT FILES FROM";
				if(openFileDialog.ShowDialog() == DialogResult.OK)
				{
					directoryName = Path.GetDirectoryName(openFileDialog.FileName);
				}
			}

			if(String.IsNullOrEmpty(directoryName))
			{
				return;
			}

			txtDecompileSrcDir.Text = directoryName;
		}

		private void BtnDecompileTarget_Click(object sender, EventArgs e)
		{
			string directoryName = "";

			using(var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "ADV Files (*.adv)|*.adv|All Files (*.*)|*.*";
				openFileDialog.CheckFileExists = false;
				openFileDialog.CheckPathExists = true;
				openFileDialog.FileName = "PICK A DIRECTORY TO EXPORT FILES TO";
				if(openFileDialog.ShowDialog() == DialogResult.OK)
				{
					directoryName = Path.GetDirectoryName(openFileDialog.FileName);
				}
			}

			if(String.IsNullOrEmpty(directoryName))
			{
				return;
			}

			txtDecompileTargetDir.Text = directoryName;
		}

		private void BtnCompileSrcDir_Click(object sender, EventArgs e)
		{
			string directoryName = "";

			using(var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "ADV Files (*.adv)|*.adv|All Files (*.*)|*.*";
				openFileDialog.CheckFileExists = false;
				openFileDialog.CheckPathExists = true;
				openFileDialog.FileName = "PICK A DIRECTORY TO IMPORT FILES FROM";
				if(openFileDialog.ShowDialog() == DialogResult.OK)
				{
					directoryName = Path.GetDirectoryName(openFileDialog.FileName);
				}
			}

			if(String.IsNullOrEmpty(directoryName))
			{
				return;
			}

			txtCompileSrcDir.Text = directoryName;
		}

		private void BtnCompileTargetDir_Click(object sender, EventArgs e)
		{
			string directoryName = "";

			using(var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "DAT Files (*.dat)|*.dat|All Files (*.*)|*.*";
				openFileDialog.CheckFileExists = false;
				openFileDialog.CheckPathExists = true;
				openFileDialog.FileName = "PICK A DIRECTORY TO EXPORT FILES TO";
				if(openFileDialog.ShowDialog() == DialogResult.OK)
				{
					directoryName = Path.GetDirectoryName(openFileDialog.FileName);
				}
			}

			if(String.IsNullOrEmpty(directoryName))
			{
				return;
			}

			txtCompileTargetDir.Text = directoryName;
		}

		private void BtnRevCompileToDecompile_Click(object sender, EventArgs e)
		{
			if(txtCompileSrcDir.Text != "") txtDecompileTargetDir.Text = txtCompileSrcDir.Text;
			if(txtCompileTargetDir.Text != "") txtDecompileSrcDir.Text = txtCompileTargetDir.Text;

			if(rdoCompileSys1.Checked) rdoDecompileSys1.Checked = true;
			else if(rdoCompileSys2.Checked) rdoDecompileSys2.Checked = true;
			else if(rdoCompileSys3.Checked) rdoDecompileSys3.Checked = true;

			if(rdoCompileOptAll.Checked) rdoDecompileOptAll.Checked = true;
			else if(rdoCompileOptPages.Checked) rdoDecompileOptPages.Checked = true;
			else if(rdoCompileOptVerbobjs.Checked) rdoDecompileOptVerbobjs.Checked = true;

			if(rdoCompileOutShiftJIS.Checked) rdoDecompileSourceShiftJIS.Checked = true;
			else if(rdoCompileOutUTF8.Checked) rdoDecompileSourceUTF8.Checked = true;
			DecompileSourceUpdate();
		}

		private void BtnRevDecompileToCompile_Click(object sender, EventArgs e)
		{
			if(txtDecompileSrcDir.Text != "") txtCompileTargetDir.Text = txtDecompileSrcDir.Text;
			if(txtDecompileTargetDir.Text != "") txtCompileSrcDir.Text = txtDecompileTargetDir.Text;

			if(rdoDecompileSys1.Checked) rdoCompileSys1.Checked = true;
			else if(rdoDecompileSys2.Checked) rdoCompileSys2.Checked = true;
			else if(rdoDecompileSys3.Checked) rdoCompileSys3.Checked = true;

			if(rdoDecompileOptAll.Checked) rdoCompileOptAll.Checked = true;
			else if(rdoDecompileOptPages.Checked) rdoCompileOptPages.Checked = true;
			else if(rdoDecompileOptVerbobjs.Checked) rdoCompileOptVerbobjs.Checked = true;

			if(rdoDecompileOutShiftJIS.Checked) rdoCompileSourceShiftJIS.Checked = true;
			else if(rdoDecompileOutUTF8.Checked) rdoCompileSourceUTF8.Checked = true;
		}

		private void BtnBeginDecompile_Click(object sender, EventArgs e)
		{
			DecompileMain();
		}

		private void BtnBeginCompile_Click(object sender, EventArgs e)
		{
			CompileMain();
		}

		private void TlsSavePref_Click(object sender, EventArgs e)
		{
			SavePreferences(sessionSaveFile);
		}

		private void outputJunkCodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(!tlsOutputJunk.Checked)
			{
				if(MessageBox.Show("\"Junk code\" is code that exists beyond the first EOF of a code file. " +
					"This feature should only be enabled if the decompiler has discovered a label jump beyond " +
					"the EOF, or if you are attempting to diagnose decompile problems. Be warned that code " +
					"decompiled with this feature will overwrite existing code - it should be sent to a distinct " +
					"folder instead. Enable?", "Warning",
					MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					tlsOutputJunk.Checked = true;
				}
			}
			else
			{
				tlsOutputJunk.Checked = false;
			}
		}

		public bool OutputJunk()
		{
			return tlsOutputJunk.Checked;
		}

		private void use3ParamDCommandToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(!tlsDCmd.Checked)
			{
				if(MessageBox.Show("Most System 2 games use an 8-param D command, while a others use a 3-param D " +
					"command. Enable this feature to use the rarer, 3-param mode.", "Warning",
					MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					tlsDCmd.Checked = true;
				}
			}
			else
			{
				tlsDCmd.Checked = false;
			}
		}

		public int DCommandMode()
		{
			if(tlsDCmd.Checked) return 3;
			else return 8;
		}

		private void rdoDecompileSys1_CheckedChanged(object sender, EventArgs e)
		{
			tlsDCmd.Enabled = false;
		}

		private void rdoDecompileSys2_CheckedChanged(object sender, EventArgs e)
		{
			tlsDCmd.Enabled = true;
		}

		private void rdoDecompileSys3_CheckedChanged(object sender, EventArgs e)
		{
			tlsDCmd.Enabled = false;
		}

		private void tabCtrlMain_SelectedIndexChanged(object sender, EventArgs e)
		{
			ChangeTab();
		}

		private void ChangeTab()
		{
			if(tabCtrlMain.SelectedIndex == 0)
			{
				tlsDCmd.Enabled = rdoDecompileSys2.Checked;
			}
			else
			{
				tlsDCmd.Enabled = rdoCompileSys2.Checked;
			}
		}

		public bool MergeDiacritic()
		{
			return tlsDiacritic.Checked;
		}

		private void tlsDiacritic_Clicked(object sender, EventArgs e)
		{
			if(!tlsDiacritic.Checked)
			{
				tlsDiacritic.Checked = true;
			}
			else
			{
				if(MessageBox.Show("By default, single-byte kana are combined with adjoining diacritic marks." +
					"By disabling this feature. Disable?", "Warning",
					MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					tlsDCmd.Checked = true;
				}
			}
		}

		public bool FullLabels()
		{
			return rdoFullLabel.Checked;
		}

		private void rdoCompileSourceUTF8_CheckedChanged(object sender, EventArgs e)
		{
			if(rdoCompileSourceUTF8.Checked)
			{
				rdoCompileOutShiftJIS.Checked = false;
				rdoCompileOutUTF8.Checked = true;
				rdoCompileOutShiftJIS.Enabled = false;
			}
			else
			{
				rdoCompileOutShiftJIS.Enabled = true;
			}
		}

		private void rdoDecompileSourceShiftJIS_CheckedChanged(object sender, EventArgs e)
		{
			if(rdoDecompileSourceShiftJIS.Checked)
			{
				DecompileSourceUpdate();
			}
		}

		private void rdoDecompileSourceUTF8_CheckedChanged(object sender, EventArgs e)
		{
			if(rdoDecompileSourceUTF8.Checked)
			{
				DecompileSourceUpdate();
			}
		}

		private void rdoMSX_CheckedChanged(object sender, EventArgs e)
		{
			if(rdoDecompileSourceMSX.Checked)
			{
				DecompileSourceUpdate();
			}
		}

		private void DecompileSourceUpdate()
		{
			if(rdoDecompileSourceShiftJIS.Checked)
			{
				rdoDecompileOutShiftJIS.Enabled = true;

				rdoHiragana.Enabled = true;
				rdoKatakana.Enabled = true;
				rdoRaw.Enabled = true;
				tlsDiacritic.Enabled = false;
			}
			else if(rdoDecompileSourceUTF8.Checked)
			{
				rdoDecompileOutShiftJIS.Enabled = false;

				rdoDecompileOutShiftJIS.Checked = false;
				rdoDecompileOutUTF8.Checked = true;

				rdoHiragana.Enabled = false;
				rdoKatakana.Enabled = false;
				rdoRaw.Enabled = false;
				tlsDiacritic.Enabled = false;
			}
			else if(rdoDecompileSourceMSX.Checked)
			{
				rdoDecompileOutShiftJIS.Enabled = true;

				rdoHiragana.Enabled = false;
				rdoKatakana.Enabled = false;
				rdoRaw.Enabled = false;
				tlsDiacritic.Enabled = true;
			}
		}
	}
}
