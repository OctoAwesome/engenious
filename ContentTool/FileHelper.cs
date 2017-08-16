using ContentTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTool
{
    [Flags]
    public enum FileAction
    {
        Ask = 1,
        Overwrite = 2,
        Repeat = 4,
        Skip = 8
    }
    class FileHelper
    {
        public static FileAction CopyDirectory(string src, string dest, ContentFolder fld, FileAction action = FileAction.Ask)
        {
            var foldername = Path.GetFileName(src);
            dest = Path.Combine(dest,foldername);
            Directory.CreateDirectory(dest);

            var newfld = (ContentFolder)(fld.Content.FirstOrDefault(x => x.Name == foldername && x is ContentFolder));
            if (newfld == null)
            {
                newfld = new ContentFolder(foldername, fld);
                fld.Content.Add(newfld);
            }
            fld = newfld;

            action = CopyFiles(Directory.GetFiles(src), dest, fld, action);
            foreach (var dir in Directory.GetDirectories(src))
            {
                action = CopyDirectory(dir, dest, fld, action);
            }
            return action;
        }
        public static FileAction CopyFiles(string[] files, string dir, ContentFolder fld, FileAction action = FileAction.Ask)
        {
            foreach (var src in files)
            {
                var filename = Path.GetFileName(src);
                var dest = Path.Combine(dir, filename);
                if (Path.GetFullPath(dest) != Path.GetFullPath(src))
                {
                    //TODO: warning file exists

                    bool repeat = true, next = false;
                    while (repeat)
                    {
                        repeat = false;
                        if (File.Exists(dest))
                        {
                            if (action.HasFlag(FileAction.Ask))
                            {
                                using (var ask = new Forms.Dialogs.OverwriteDialog())
                                {
                                    ask.FileName = filename;
                                    action = ask.ShowDialog();
                                }
                            }
                            switch ((FileAction)((int)action & 0xE))
                            {
                                case FileAction.Overwrite:
                                    File.Copy(src, dest, true);
                                    break;
                                case FileAction.Skip:
                                    next = true;
                                    break;
                                case FileAction.Repeat:
                                    repeat = true;
                                    break;
                            }
                        }
                        else
                        {
                            File.Copy(src, dest);
                        }
                    }
                    if (next)
                        continue;

                }

                if (!fld.Content.Any(x=>x.Name == filename && x is ContentFile))
                    fld.Content.Add(new ContentFile(filename, fld));
            }
            return action;
        }
    }
}
