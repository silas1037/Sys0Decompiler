using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sys0Decompiler
{
    public partial class FileFormatSelectionForm : Form
    {
        public FileFormatSelectionForm()
        {
            InitializeComponent();
        }

        public static ArchiveFileType SelectFileType()
        {
            using (var form = new FileFormatSelectionForm())
            {
                var dialogResult = form.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    switch (form.lstFileType.SelectedIndex)
                    {
                        case 0:
                            return ArchiveFileType.AldFile;
                        case 1:
                            return ArchiveFileType.DatFile;
                        case 2:
                            return ArchiveFileType.AlkFile;
                        case 3:
                            return ArchiveFileType.Afa1File;
                        case 4:
                            return ArchiveFileType.Afa2File;
                        case 5:
                            return ArchiveFileType.SofthouseCharaVfs11File;
                        case 6:
                            return ArchiveFileType.SofthouseCharaVfs20File;
                        case 7:
                            return ArchiveFileType.HoneybeeArcFile;
                    }
                }
            }
            return ArchiveFileType.Invalid;
        }

        private void FileFormatSelectionForm_Load(object sender, EventArgs e)
        {
        }

        private void FileFormatSelectionForm_Shown(object sender, EventArgs e)
        {
            lstFileType.SelectedIndex = 0;
            lstFileType.Focus();
        }

        private void lstFileType_MouseDoubleClick(object sender, MouseEventArgs e)
        {
		    
        }

        private void lstFileType_DoubleClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
