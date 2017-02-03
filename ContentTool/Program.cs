using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ContentTool.Builder;
using ContentTool.Items;
using engenious;
using engenious.Content.Pipeline;

namespace ContentTool
{
    internal class Program
    {

        public static ToolConfiguration Configuration { get; set; }

        internal static string MakePathRelative(string filename)
        {
            try
            {
                Uri root = new Uri(Path.GetFullPath(Arguments.ProjectDir), UriKind.Absolute);
                Uri uri = new Uri(Path.GetFullPath(filename), UriKind.Absolute);
                return root.MakeRelativeUri(uri).ToString();
            }
            catch
            {
                return filename;
            }
        }
        public static Arguments Arguments { get; private set; }
        [STAThread]
        public static int Main(string[] args)
        {

            Arguments = new Arguments();
            Arguments.ParseArguments(args);

            Configuration = ToolConfiguration.Load();

            if (Arguments.Hidden)
            {
                var context = new GlSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);
                ContentBuilder builder = new ContentBuilder(ContentProject.Load(Arguments.ContentProject));
                if (Arguments.Configuration.HasValue)
                    builder.Project.Configuration = Arguments.Configuration.Value;
                if (Arguments.OutputDirectory != null)
                    builder.Project.OutputDir = Arguments.OutputDirectory;
                builder.BuildMessage += (sender, e) =>
                {
                    if (e.MessageType == BuildMessageEventArgs.BuildMessageType.Error)
                        Console.Error.WriteLine(MakePathRelative(e.FileName) + e.Message);
                    else
                        Console.WriteLine(MakePathRelative(e.FileName) + e.Message);
                };
                builder.BuildStatusChanged += (sender, buildStep) =>
                {
                    string message = (buildStep & (BuildStep.Build | BuildStep.Clean)).ToString() + " ";
                    bool error = false;
                    if (buildStep.HasFlag(BuildStep.Abort))
                    {
                        message += "aborted!";
                        error = true;
                    }
                    else if (buildStep.HasFlag(BuildStep.Finished))
                    {
                        message += "finished!";
                        if (builder.FailedBuilds != 0)
                        {
                            message += " " + builder.FailedBuilds.ToString() + " files failed to build!";
                            error = true;
                        }
                    }
                    if (error)
                        Console.Error.WriteLine(message);
                    else
                        Console.WriteLine(message);
                };
                builder.ItemProgress += (sender, e) =>
                {
                    string message = e.Item + " " + (e.BuildStep & (BuildStep.Build | BuildStep.Clean)).ToString().ToLower() + "ing ";

                    bool error = false;
                    if (e.BuildStep.HasFlag(BuildStep.Abort))
                    {
                        message += "failed!";
                        error = true;
                    }
                    else if (e.BuildStep.HasFlag(BuildStep.Finished))
                    {
                        message += "finished!";
                    }
                    if (error)
                        Console.Error.WriteLine(message);
                    else
                        Console.WriteLine(message);
                };
                builder.Build();
                while (builder.IsBuilding)
                {
                    context.RunOnCurrentThread();
                }
                builder.Join();
                if (builder.FailedBuilds != 0)
                    return -1;
            }
            else {
                Application.EnableVisualStyles();
                using (FrmMain mainForm = new FrmMain())
                {
                    Application.Run(mainForm);
                }
            }
            return 0;
        }
    }
}
