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
	public partial class ErrorListForm : Form
	{
		StringBuilder messages = new StringBuilder();
		int lastTickCount = 0;

		public void DisplayMessage(string errorMessage)
		{
			bool doDisplay = false;
			if (String.IsNullOrEmpty(errorMessage))
			{
				doDisplay = true;
			}
			else
			{
				messages.AppendLine(errorMessage);
				if (lastTickCount == 0)
				{
					doDisplay = true;
				}
				int elapsedTime = Environment.TickCount - lastTickCount;
				if (elapsedTime > 500)
				{
					doDisplay = true;
				}
			}
			if (doDisplay)
			{
				this.textBox1.Text = messages.ToString();
				this.textBox1.SelectionStart = this.textBox1.Text.Length;
				this.textBox1.SelectionLength = 0;
				Update();
				lastTickCount = Environment.TickCount;
			}
			else
			{
				if (!this.timer1.Enabled)
				{
					this.timer1.Interval = 1;
					this.timer1.Start();
				}
			}
		}

		void timer1_Tick(object sender, EventArgs e)
		{
			if (this.IsDisposed)
			{
				return;
			}
			this.timer1.Stop();
			DisplayMessage(null);
		}

		public ErrorListForm()
		{
			InitializeComponent();
		}

		private void ErrorListForm_Load(object sender, EventArgs e)
		{

		}
	}
}
