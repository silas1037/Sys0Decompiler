using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.IO;

namespace Sys0Decompiler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            //ExplorerForm explorerForm = null;
			DecompilerForm decompilerForm = null;

			//explorerForm = new ExplorerForm();
			decompilerForm = new DecompilerForm();
			Application.Run(decompilerForm);
            return;
        }
    }
}
