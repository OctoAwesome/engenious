using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace engenious.Pipeline
{
    public class FFmpeg
    {
        private string _ffmpegExe;
        private SynchronizationContext _syncContext;
        public FFmpeg(SynchronizationContext syncContext)
            : this(syncContext,LocateFFmpegExe())
        {

        }
        private static string LocateFFmpegExe()
        {
            string completePath;
            try
            {
                completePath = File.ReadAllText(".ffmpeg");
                if (System.IO.File.Exists(completePath))
                    return completePath;
            }
            catch { }

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string ext = "", searchPath = "";
            var platform = PlatformHelper.RunningPlatform();
            switch (platform)
            {
                case Platform.Windows:
                    ext = ".exe";
                    break;
            }
            completePath = System.IO.Path.Combine(path, "ffmpeg" + ext);
            if (System.IO.File.Exists(completePath))
                return completePath;
            switch (platform)
            {
                case Platform.Linux:
                    completePath = System.IO.Path.Combine("/usr/bin", "ffmpeg" + ext);
                    if (System.IO.File.Exists(completePath))
                        return completePath;
                    break;
                case Platform.Mac:
                    completePath = System.IO.Path.Combine("/Applications", "ffmpeg" + ext);
                    if (System.IO.File.Exists(completePath))
                        return completePath;
                    break;
            }
            return "ffmpeg" + ext;
        }
        public FFmpeg(SynchronizationContext syncContext,string exePath)
        {
            _syncContext = syncContext;
            _ffmpegExe = exePath;
        }
        public System.Diagnostics.Process RunCommand(string arguments, bool throwAll = false)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo(_ffmpegExe, arguments);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            try
            {
                if (p.Start())
                {

                    return p;
                }
            }
            catch (Win32Exception ex)
            {
                if (throwAll || ex.NativeErrorCode != 2) //File not found
                    throw ex;
                _syncContext.Send(new SendOrPostCallback((o) =>
                {
                    using (var ofd = new OpenFileDialog())
                    {
                        ofd.Title = "FFmpeg";
                        ofd.FileName = _ffmpegExe;
                        ofd.Filter = "Executable files|" + (PlatformHelper.RunningPlatform() == Platform.Windows ? "*.exe" : "*.*");
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            _ffmpegExe = ofd.FileName;
                            File.WriteAllText(".ffmpeg", _ffmpegExe);
                            
                        }
                    }
                }),null);
                if (File.Exists(_ffmpegExe))
                    return RunCommand(arguments, true);
                throw ex;

            }
            return null;
        }
    }
}

