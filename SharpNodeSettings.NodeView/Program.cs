using System;
using System.Windows.Forms;
using SharpNodeSettings.View;

namespace SharpNodeSettings.NodeView {
    internal static class Program {
        /// <summary>
        ///     应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormNodeView("127.0.0.1", 12345));
        }
    }
}