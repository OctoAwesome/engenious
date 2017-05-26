using System;
using System.Windows.Forms;

namespace ContentTool.old.Dialog
{
    public partial class NewFolderDialog : Form
    {
        public string FileName { get; set; }

        public NewFolderDialog()
        {
            InitializeComponent();
            button_ok.Enabled = false;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            FileName = textBox_name.Text;
            Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            FileName = textBox_name.Text;
            Close();
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            button_ok.Enabled = textBox_name.Text != "";
        }
    }
}
