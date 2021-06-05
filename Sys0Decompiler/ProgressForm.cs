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
	public partial class ProgressForm : Form
	{
		public ProgressForm()
		{
			InitializeComponent();
		}

		private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
			}
		}
	}
}
