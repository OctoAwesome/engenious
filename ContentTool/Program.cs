using System;
using System.Windows.Forms;
using ContentTool.Builder;
using ContentTool.Forms;
using ContentTool.Models;
using ContentTool.Presenters;
using engenious.Content.Pipeline;

namespace ContentTool
{
    internal static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {

            //Console.WriteLine(@"D:\Projects\engenious\Sample\Content\simple.glsl(13) : error C2143: syntax error : missing';' before '}'");

            
            var arguments = new Arguments();
            arguments.ParseArguments(args);
            
            if (arguments.Hidden)
            {
                try
                {
                    var project = ContentProject.Load(string.IsNullOrEmpty(arguments.ContentProject) ? @"D:\Projects\engenious\Sample\Content\Content.ecp" : arguments.ContentProject);

                    var builder = new ContentBuilder(project);

                    builder.BuildMessage += eventArgs =>
                    {
                        if (eventArgs.MessageType != BuildMessageEventArgs.BuildMessageType.Information)
                            Console.Error.WriteLine(eventArgs.Message);
                    };
                    
                    
                    builder.Build(builder.Project);
                    builder.Join();
                    
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    return -1;
                }
            }
            //TODO implement CommandLine
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            var mainShell = new MainShell();
            var mainShellPresenter = new MainShellPresenter(mainShell, arguments);
            Application.Run(mainShell);
            return 0;
        }
    }
}
