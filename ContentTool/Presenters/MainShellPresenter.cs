using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using ContentTool.Builder;
using ContentTool.Forms;
using ContentTool.Models;
using ContentTool.Viewer;

namespace ContentTool.Presenters
{
    internal class MainShellPresenter
    {
        private readonly IMainShell _shell;

        private ContentBuilder _builder;

        private readonly ViewerManager _viewerManager;
        private readonly Arguments _arguments;

        public MainShellPresenter(IMainShell shell, Arguments arguments)
        {
            _shell = shell;
            _arguments = arguments;

            shell.CloseProjectClick += i => CloseProject();

            shell.ShowInExplorerItemClick += Shell_ShowInExplorerItemClick;
            shell.SaveProjectClick += i => SaveProject();
            shell.OpenProjectClick += (s, e) =>
            {
                if (CloseProject()) OpenProject();
            };
            shell.NewProjectClick += (s, e) =>
            {
                if (CloseProject()) NewProject();
            };
            shell.BuildItemClick += Shell_BuildItemClick;
            shell.RebuildClick += ShellOnRebuildClick;
            shell.CleanClick += ShellOnCleanClick;
            shell.OnItemSelect += Shell_OnItemSelect;

            shell.UndoClick += ShellOnUndoClick;
            shell.RedoClick += ShellOnRedoClick;

            shell.RenameItemClick += i => shell.RenameItem(i);
            shell.RemoveItemClick += Shell_RemoveItemClick;
            shell.OnAboutClick += (s, e) => shell.ShowAbout();

            shell.AddExistingItemClick += Shell_AddExistingItemClick;
            shell.AddExistingFolderClick += Shell_AddExistingFolderClick;
            shell.AddNewFolderClick += Shell_AddNewFolderClick;

            shell.OnShellLoad += Shell_OnShellLoad;

            shell.IntegrateCSClick += ShellOnIntegrateCsClick;

            _viewerManager = new ViewerManager();
        }

        private void ShellOnIntegrateCsClick(object sender, EventArgs eventArgs)
        {
            string csproj = _shell.ShowIntegrateDialog();
            if (csproj == null || _shell.Project?.ContentProjectPath == null) return;

            var csProjDir = Path.GetDirectoryName(csproj) ?? "";
            string target = "";

            string curDir = csProjDir;
            string engVersion = "*";
            if (File.Exists(Path.Combine(curDir, "packages.config")))
            {
                var pDoc = XDocument.Load(Path.Combine(curDir, "packages.config"));
                var pEng = pDoc.Descendants().FirstOrDefault(x => x.Name.LocalName == "packages").Descendants()
                    .FirstOrDefault(x =>
                        x.Name.LocalName == "package" && x.Attribute(XName.Get("id"))?.Value == "engenious");
                engVersion = pEng.Attribute(XName.Get("version"))?.Value ?? "*";
            }
            bool isPackageDir = false;
            while (!(isPackageDir = File.Exists(Path.Combine(curDir, "packages", "repositories.config"))))
            {
                if (Directory.GetFiles(curDir, "*.sln").FirstOrDefault() != null)
                    break;
                curDir = Path.GetDirectoryName(curDir);
            }
            if (isPackageDir)
            {
                var dirs = Directory.GetDirectories(Path.Combine(curDir, "packages"), "engenious." + engVersion);
                if (dirs.Length == 0)
                {
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
                    target = Path.Combine(target ?? "", "build", "engenious.targets");
                    if (!File.Exists(target))
                    {
                        MessageBox.Show("engenious.target not found!", "Error!", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (dirs.Length == 1)
                {
                    target = Path.Combine(dirs[0], "build", "engenious.targets");
                }
                else
                {
                    //TODO find correct version
                    target = Path.Combine(dirs[0], "build", "engenious.targets");
                }
            }


            var doc = XDocument.Load(csproj);
            var projectNode = doc.Descendants().FirstOrDefault(x => x.Name.LocalName.ToLower() == "project");

            if (projectNode == null)
                return;

            var imports = projectNode.Descendants().Where(x => x.Name.LocalName.ToLower() == "import");

            if (imports.FirstOrDefault(x =>
            {
                var relPath = x.Attribute("Project")?.Value;
                if (relPath == null) return false;
                relPath = relPath.Replace('\\', Path.DirectorySeparatorChar);
                var path = Path.Combine(csProjDir, relPath);
                if (Path.GetFileName(path) == "engenious.targets")
                    return File.Exists(path);
                return false;
            }) == null)
            {
                //Add target import

                var lastImport = imports.LastOrDefault() ?? projectNode.Descendants().LastOrDefault();

                if (lastImport == null)
                    return;

                var importPath = FileHelper.GetRelativePath(Path.GetFullPath(csProjDir)+Path.DirectorySeparatorChar, Path.GetFullPath(target));
                if (importPath == null)
                    return;
                importPath = importPath.Replace(Path.DirectorySeparatorChar, '\\');
                var import = new XElement(XName.Get("Import"));
                //<Import Project="..\packages\engenious.0.1.8\build\engenious.targets" Condition="Exists('..\packages\engenious.0.1.8\build\engenious.targets')" />
                import.SetAttributeValue(XName.Get("Project"), importPath);
                import.SetAttributeValue(XName.Get("Condition"),$"Exists('{importPath}')");
                lastImport.AddAfterSelf(import);
            }

            var contentItem = projectNode.Descendants().Where(x =>
                x.Name.LocalName.ToLower() == "itemgroup").SelectMany(x => x.Descendants()).FirstOrDefault(x =>
                {
                    var contentProjValue = x.Attribute("Include")?.Value;
                    if (string.IsNullOrWhiteSpace(contentProjValue)) return false;
                    contentProjValue = contentProjValue.Replace('\\', Path.DirectorySeparatorChar);
                    contentProjValue = Path.Combine(csProjDir, contentProjValue);
                    return Path.GetFullPath(contentProjValue) == Path.GetFullPath(_shell.Project?.ContentProjectPath);
                }
            );
            if (contentItem == null)
            {
                //Add Content project
                var lastItemGroup = projectNode.Descendants().LastOrDefault(x =>
                    x.Name.LocalName.ToLower() == "itemgroup");

                // Add itemgroup
                var contentItemGroup = new XElement(XNamespace.Get("ItemGroup"));
                contentItem = new XElement(XName.Get("EngeniousContentReference"));
                var engeniousContentPath = FileHelper.GetRelativePath(Path.GetFullPath(csProjDir)+Path.DirectorySeparatorChar, Path.GetFullPath(_shell.Project?.ContentProjectPath));
                if (engeniousContentPath == null)
                    return;
                engeniousContentPath = engeniousContentPath.Replace(Path.DirectorySeparatorChar, '\\');
                contentItem.SetAttributeValue(XName.Get("Include"), engeniousContentPath);
                contentItemGroup.AddFirst(contentItem);
                lastItemGroup.AddAfterSelf(contentItemGroup);
            }
            else
            {
                //Content project as EngeniousContentReference
                if (contentItem.Name.LocalName.ToLower() != "engeniouscontentreference")
                {
                    contentItem.Name = XName.Get("EngeniousContentReference");
                }
            }
            doc.Save(csproj);//TODO remove empty namespaces?
        }

        private void Shell_RemoveItemClick(ContentItem item)
        {
            if (MessageBox.Show($"Do you really want to remove {item.Name}?", "Remove Item", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            var folder = item.Parent as ContentFolder;
            if (folder != null)
            {
                //TODO remove from disk
                folder.Content.Remove(item);
            }
        }

        private void Shell_OnShellLoad(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_arguments.ContentProject)) //TODO perhaps use laodi
                OpenProject(@"D:\Projects\engenious\Sample\Content\Content.ecp");
            else
                OpenProject(_arguments.ContentProject);
        }

        private void Shell_AddNewFolderClick(ContentFolder folder)
        {
            if (folder == null)
                return;
            var numbers = folder.Content.Where(t => t.Name.StartsWith("New Folder")).Select(t =>
            {
                if (t.Name == "New Folder")
                    return 1;

                var str = t.Name.Substring("New Folder ".Length);
                if (int.TryParse(str, out var num))
                    return num;
                return -1;
            }).ToArray();
            var max = numbers.Length == 0 ? -1 : numbers.Max();
            var newFolder = new ContentFolder("New Folder" + (max == -1 ? string.Empty : $" {max + 1}"), folder);
            Directory.CreateDirectory(newFolder.FilePath);
            folder.Content.Add(newFolder);
            _shell.RenameItem(newFolder);
        }


        private void Shell_AddExistingFolderClick(ContentFolder fld)
        {
            var dest = fld.FilePath;
            var src = _shell.ShowFolderSelectDialog();
            if (src == null)
                return;

            _shell.ShowLoading();
            _shell.SuspendRendering();

            var progress = new Action<int>(i => _shell.WaitProgress(i));
            var t = new Thread(() =>
            {
                FileHelper.CopyDirectory(src, dest, fld, FileAction.Ask, progress);
                _shell.Invoke(new MethodInvoker(() =>
                {
                    _shell.ResumeRendering();
                    _shell.HideLoading();
                }));
            });
            t.Start();
        }


        private void Shell_AddExistingItemClick(ContentItem item)
        {
            var fld = (item as ContentFolder) ?? (item?.Parent as ContentFolder) ?? _shell.Project;


            string[] files = _shell.ShowFileSelectDialog();
            if (files == null)
                return;

            var dir = fld.FilePath;
            _shell.SuspendRendering();
            FileHelper.CopyFiles(files, dir, fld);
            _shell.ResumeRendering();
        }

        private void ShellOnRedoClick(object sender, EventArgs eventArgs)
        {
            var history = _shell.Project?.History;
            if (history == null || !history.CanRedo)
                return;
            history.Redo();
        }

        private void ShellOnUndoClick(object sender, EventArgs eventArgs)
        {
            var history = _shell.Project?.History;
            if (history == null || !history.CanUndo)
                return;
            history.Undo();
        }

        private void Shell_OnItemSelect(ContentItem item)
        {
            if (item.Error.HasFlag(ContentErrorType.NotFound) && _shell.ShowNotFoundDelete())
            {
                var p = (ContentFolder) item.Parent;
                p.Content.Remove(item);
            }

            var file = item as ContentFile;
            if (file != null)
                _shell.ShowViewer(_viewerManager.GetViewer(file), file);
            else
                _shell.HideViewer();
        }

        private void Shell_BuildItemClick(ContentItem item)
        {
            if (_builder == null)
            {
                _builder = new ContentBuilder(_shell.Project);
                _builder.BuildMessage += a => _shell.Invoke(((MethodInvoker) (() => _shell.WriteLineLog(a.Message))));
            }
            _shell.ShowLog();

            if (_shell.CurrentViewer != null && _shell.CurrentViewer.UnsavedChanges)
                _shell.CurrentViewer.Save(); //TODO: always save together with project?
            _builder.Build(item);
        }

        private void ShellOnCleanClick(object sender, EventArgs eventArgs)
        {
            if (_builder == null)
            {
                _builder = new ContentBuilder(_shell.Project);
                _builder.BuildMessage += a => _shell.Invoke(((MethodInvoker) (() => _shell.WriteLineLog(a.Message))));
            }
            _shell.ShowLog();

            _builder.Clean();
        }

        private void ShellOnRebuildClick(object sender, EventArgs eventArgs)
        {
            if (_builder == null)
            {
                _builder = new ContentBuilder(_shell.Project);
                _builder.BuildMessage += a => _shell.Invoke(((MethodInvoker) (() => _shell.WriteLineLog(a.Message))));
            }
            _shell.ShowLog();

            if (_shell.CurrentViewer != null && _shell.CurrentViewer.UnsavedChanges)
                _shell.CurrentViewer.Save(); //TODO: always save together with project?
            _builder.Rebuild();
        }

        public bool CloseProject()
        {
            if (_shell.Project == null)
                return true;

            if (_shell.Project.HasUnsavedChanges)
            {
                if (!_shell.ShowCloseWithoutSavingConfirmation())
                    return false;
            }

            _shell.Project = null;
            _shell.ClearLog();
            return true;
        }

        public void NewProject()
        {
            string path = _shell.ShowSaveAsDialog();
            if (path == null) return;
            _shell.Project = new ContentProject("Content", path, Path.GetDirectoryName(path));
            SaveProject();
        }

        public void OpenProject(string path = null)
        {
            if (path == null)
                path = _shell.ShowOpenDialog();
            if (path == null)
                return;

            _shell.ShowLoading("Loading Project...");

            var t = new Thread(() =>
            {
                try
                {
                    var proj = ContentProject.Load(path);

                    _shell.Invoke(new MethodInvoker(() =>
                    {
                        _shell.Project = proj;
                        _shell.WriteLineLog("Opened " + path);
                    }));
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    _shell.Invoke(new MethodInvoker(() => _shell.HideLoading()));
                }
            });
            t.Start();
        }

        public void SaveProject(string path = null)
        {
            if (path == null)
                _shell.Project.Save();
            else
                _shell.Project.Save(path);
        }

        private void Shell_ShowInExplorerItemClick(Models.ContentItem item)
        {
            var path = item.FilePath;
            if (item is ContentFile)
                path = Path.GetDirectoryName(path);
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void Shell_CloseProjectClick(ContentItem item)
        {
            CloseProject();
        }
    }
}