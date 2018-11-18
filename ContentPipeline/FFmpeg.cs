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

            //Attempt to fing ffmpeg path in .ffmpeg file (first in local dir then in ContentTool dir)
            string dotFFMpegPath = ".ffmpeg";
            if (!File.Exists(dotFFMpegPath))
                dotFFMpegPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ".ffmpeg");
            if(File.Exists(dotFFMpegPath))
            {
                try
                {
                    completePath = File.ReadAllText(".ffmpeg");
                    if (File.Exists(completePath))
                        return completePath;
                    else
                        Console.WriteLine($"Invalid path {completePath} in .ffmpeg file");
                }
                catch
                {
                    Console.WriteLine($"Could not read .ffmpeg at path {dotFFMpegPath}");
                }
            }
            
            //Attempt to find ffmpeg in local directory
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

            //Attempt to find FFMPEG at OS-specific location
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

            //Return default ffmpeg path
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
                    throw new FileNotFoundException($"Could not find ffmpeg at location: '{_ffmpegExe}'.");
                
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
                throw new FileNotFoundException($"Could not find ffmpeg at location: '{_ffmpegExe}'.");

            }
            return null;
        }
    }
}

