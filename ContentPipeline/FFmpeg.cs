using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using engenious.Helper;

namespace engenious.Pipeline
{
    public class FFmpeg
    {
        private string _ffmpegExe;
        private readonly SynchronizationContext _syncContext;
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
                if (File.Exists(completePath))
                    return completePath;
            }
            catch
            {
                // ignored
            }

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string ext =string.Empty;
            var platform = PlatformHelper.RunningPlatform();
            switch (platform)
            {
                case Platform.Windows:
                    ext = ".exe";
                    break;
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            completePath = Path.Combine(path, "ffmpeg" + ext);
            if (File.Exists(completePath))
                return completePath;
            switch (platform)
            {
                case Platform.Linux:
                    completePath = Path.Combine("/usr/bin", "ffmpeg" + ext);
                    if (File.Exists(completePath))
                        return completePath;
                    break;
                case Platform.Mac:
                    completePath = Path.Combine("/Applications", "ffmpeg" + ext);
                    if (File.Exists(completePath))
                        return completePath;
                    break;
                case Platform.Windows:
                    completePath = Environment.GetEnvironmentVariable("FFMPEG");
                    if (File.Exists(completePath))
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
        public Process RunCommand(string arguments, bool throwAll = false)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(_ffmpegExe, arguments);
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
                    throw;
                
                _syncContext?.Send(o =>
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
                },null);
                if (File.Exists(_ffmpegExe))
                    return RunCommand(arguments, true);
                throw;

            }
            return null;
        }
    }
}

