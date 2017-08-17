using ContentTool.Commandline;
using ContentTool.Forms;
using ContentTool.Models;
using ContentTool.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTool.Builder;
using engenious.Content.Pipeline;

namespace ContentTool
{
    static class Program
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
                    ContentProject project;
                    if (string.IsNullOrEmpty(arguments.ContentProject)) //TODO perhaps use laodi
                        project =
                            ContentProject.Load(@"D:\Projects\engenious\Sample\Content\Content.ecp");
                    else
                        project = ContentProject.Load(arguments.ContentProject);

                    ContentBuilder builder = new ContentBuilder(project);

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
            else
            {
                //TODO implement CommandLine
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();

                MainShell mainShell = new MainShell();
                MainShellPresenter mainShellPresenter = new MainShellPresenter(mainShell, arguments);
                Application.Run(mainShell);
                
                
            }
            return 0;
        }
    }
}
