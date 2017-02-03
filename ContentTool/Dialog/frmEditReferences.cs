using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace ContentTool.Dialog
{
    public partial class FrmEditReferences : Form
    {
        public FrmEditReferences()
        {
            InitializeComponent();
        }

        private void frmEditReferences_Load(object sender, EventArgs e)
        {

        }

        public string RootDir { get; internal set; }
        private ObservableCollection<string> _references = new ObservableCollection<string>();
        private List<string> _original;
        public List<string> References
        {
            get
            {
                return _references.ToList();
            }
            internal set
            {
                _original = value;
                _references = new ObservableCollection<string>(value);
                btnOk.Enabled = Changed;
                _references.CollectionChanged += References_CollectionChanged;
                lstReferences.DataSource = _references;
            }
        }

        private void References_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            btnOk.Enabled = Changed;
        }

        private bool Changed
        {
            get
            {
                if (_original.Count != _references.Count)
                    return true;
                for (int i = 0; i < _original.Count; i++)
                    if (_original[i] != _references[i])
                        return true;
                return false;
            }
        }
        private string[] _currentFiles;
        private string[] _relativePaths;
        private string[] CurrentFiles
        {
            get
            {
                return cbRelative.Checked ? _relativePaths : _currentFiles;
            }
            set
            {
                Uri root = new Uri(RootDir + System.IO.Path.DirectorySeparatorChar, UriKind.Absolute);
                _currentFiles = value;
                int i = 0;
                _relativePaths = new string[value.Length];
                foreach (var file in value)
                {
                    Uri uri = new Uri(file, UriKind.Absolute);
                    _relativePaths[i] = root.MakeRelativeUri(uri).ToString();
                    i++;
                }
            }
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = ".Net Assembly(.dll)|*.dll";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CurrentFiles = ofd.FileNames;
                    txtPath.Text = string.Join(";", CurrentFiles);
                }
            }
        }

        private void cbRelative_CheckedChanged(object sender, EventArgs e)
        {
            txtPath.Text = string.Join(";", CurrentFiles);
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            /*if (txtPath.Text.Length > 1)
                CurrentFiles = txtPath.Text.Split(';');
            else
                CurrentFiles = new string[] { };*/
            btnAdd.Enabled = txtPath.Text.Length > 1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            foreach (var path in CurrentFiles)
            {
                _references.Add(path);
            }
            txtPath.Text = "";
            lstReferences.DataSource = null;
            lstReferences.DataSource = _references;
            CurrentFiles = new string[] { };
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (var obj in lstReferences.SelectedItems)
                if (obj != null)
                    _references.Remove(obj.ToString());
            lstReferences.SelectedItem = null;

            lstReferences.DataSource = null;
            lstReferences.DataSource = _references;
            btnRemove.Enabled = lstReferences.SelectedItems.Count > 0;
        }

        private void lstReferences_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = lstReferences.SelectedItems.Count > 0;
        }
    }
}
