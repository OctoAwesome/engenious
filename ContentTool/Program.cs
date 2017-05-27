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
        static void Main(string[] args)
        {
            //TODO implement CommandLine
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainShell mainShell = new MainShell();
            MainShellPresenter mainShellPresenter = new MainShellPresenter(mainShell);
            mainShellPresenter.OpenProject(@"D:\Projects\AntMe\antme\src\AntMe.Client.ThreeD\Content\Content.ecp");
            Application.Run(mainShell);
        }
    }
}
