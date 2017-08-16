using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTool.Forms.Dialogs
{
    public partial class OverwriteDialog : Form
    {
        public OverwriteDialog()
        {
            InitializeComponent();
        }
        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                label1.Text = $"File {value} already exists at the destination.";
            }
        }
        public bool Remember => checkBox.Checked;
        public FileAction Action { get; private set; }

        public new FileAction ShowDialog()
        {
            Action = FileAction.Ask;
            base.ShowDialog();

            return Action;
        }

        private void button_overwrite_Click(object sender, EventArgs e)
        {
            Action |= FileAction.Overwrite;
            Close();
        }

        private void button_skip_Click(object sender, EventArgs e)
        {
            Action |= FileAction.Skip;
            Close();
        }

        private void button_repeat_Click(object sender, EventArgs e)
        {
            Action |= FileAction.Repeat;
            Close();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox.Checked)
                Action &= ~FileAction.Ask; 
            else
                Action |= FileAction.Ask;
        }

        private void OverwriteDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (((int)Action & 0xE) == 0)
                Action |= FileAction.Skip;
        }
    }
}
