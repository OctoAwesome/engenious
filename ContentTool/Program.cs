using CommandLine;
using ContentTool.Commandline;
using ContentTool.Forms;
using ContentTool.Models;
using ContentTool.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTool
{
    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {

            Console.WriteLine(@"D:\Projects\engenious\Sample\Content\simple.glsl(13) : error C2143: syntax error : missing';' before '}'");

            //return -1;
            //TODO implement CommandLine
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainShell mainShell = new MainShell();
            MainShellPresenter mainShellPresenter = new MainShellPresenter(mainShell);
            mainShellPresenter.OpenProject(@"D:\Projects\engenious\Sample\Content\Content.ecp");
            Application.Run(mainShell);

            return 0;
        }
    }
}
