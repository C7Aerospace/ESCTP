using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SCTPClient
{
    static class Program
    {
        public static Mainform mf;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mf = new Mainform();
            Application.Run(mf);
        }
    }
}
