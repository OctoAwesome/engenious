using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTool.Models;
using engenious.Pipeline;

namespace ContentTool.Viewer.Viewers
{
    [ViewerInfo(".spritefont", false)]
    public partial class SpriteFontViewer : UserControl, IViewer
    {
        SpriteFontContent spf;

        public SpriteFontViewer()
        {
            InitializeComponent();
            FillComboBox();
        }

        public Control GetViewer(ContentFile file)
        {
            spf = new SpriteFontContent(file.FilePath);

            return this;
        }

        private void fontComboBox_DropDown(object sender, EventArgs e)
        {
            FillComboBox();
        }

        private void FillComboBox()
        {
            string selected = fontComboBox.SelectedItem?.ToString();
            //_cmbFontName.Items.Clear();
            var families = FontFamily.Families.Select(x => x.Name).Distinct().ToArray();
            Array.Sort(families);
            for (int i = fontComboBox.Items.Count - 1; i >= 0; i--)
            {
                if (!families.Contains(fontComboBox.Items[i]?.ToString()))
                    fontComboBox.Items.RemoveAt(i);
            }
            for (int i = 0; i < families.Length; i++)
            {
                if (!fontComboBox.Items.Contains(families[i]))
                {
                    //Debug.WriteLine($"{i} : {families[i]}");
                    fontComboBox.Items.Insert(i, families[i]);
                }
            }
            fontComboBox.SelectedItem = selected;
        }

        private void checkBox_bold_CheckedChanged(object sender, EventArgs e)
        {
            fontComboBox.FontStyle = FontStyle.Regular;
            if (checkBox_bold.Checked)
                fontComboBox.FontStyle |= FontStyle.Bold;
            if (checkBox_italics.Checked)
                fontComboBox.FontStyle |= FontStyle.Italic;
        }
    }
}
