using System;
using engenious.Content.Serialization;
using engenious.Content;
using engenious.Graphics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using OpenTK;
using OpenTK.Graphics;
using ContentTool.Builder;
using ContentTool.Items;
using System.Threading;
using System.ComponentModel;

namespace ContentTool
{
    class Program
    {

        public static ToolConfiguration Configuration { get; set; }

        internal static string MakePathRelative(string filename)
        {
            try
            {
                Uri root = new Uri(System.IO.Path.GetFullPath(Arguments.ProjectDir), UriKind.Absolute);
                Uri uri = new Uri(System.IO.Path.GetFullPath(filename), UriKind.Absolute);
                return root.MakeRelativeUri(uri).ToString();
            }
            catch
            {
                return filename;
            }
        }
        public static Arguments Arguments { get; private set; }
        [STAThread()]
        public static int Main(string[] args)
        {

            Arguments = new Arguments();
            Arguments.ParseArguments(args);

            Configuration = ToolConfiguration.Load();

            if (Arguments.Hidden)
            {
                var context = new engenious.GLSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);
                ContentBuilder builder = new ContentBuilder(ContentProject.Load(Arguments.ContentProject));
                if (Arguments.Configuration.HasValue)
                    builder.Project.Configuration = Arguments.Configuration.Value;
                if (Arguments.OutputDirectory != null)
                    builder.Project.OutputDir = Arguments.OutputDirectory;
                builder.BuildMessage += (sender, e) =>
                {
                    if (e.MessageType == engenious.Content.Pipeline.BuildMessageEventArgs.BuildMessageType.Error)
                        Console.Error.WriteLine(Program.MakePathRelative(e.FileName) + e.Message);
                    else
                        Console.WriteLine(Program.MakePathRelative(e.FileName) + e.Message);
                };
                builder.BuildStatusChanged += (sender, buildStep) =>
                {
                    string message = (buildStep & (BuildStep.Build | BuildStep.Clean)).ToString() + " ";
                    bool error = false;
                    if (buildStep.HasFlag(Builder.BuildStep.Abort))
                    {
                        message += "aborted!";
                        error = true;
                    }
                    else if (buildStep.HasFlag(Builder.BuildStep.Finished))
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
                    if (e.BuildStep.HasFlag(Builder.BuildStep.Abort))
                    {
                        message += "failed!";
                        error = true;
                    }
                    else if (e.BuildStep.HasFlag(Builder.BuildStep.Finished))
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
                System.Windows.Forms.Application.EnableVisualStyles();
                using (frmMain mainForm = new frmMain())
                {
                    System.Windows.Forms.Application.Run(mainForm);
                }
            }
            return 0;
        }
    }
}
