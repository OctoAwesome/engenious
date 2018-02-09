using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using ContentTool.Models;
using ContentTool.Models.History;
using engenious.Pipeline;

namespace ContentTool.Viewer.Viewers
{
    [ViewerInfo(".spritefont", false)]
    public partial class SpriteFontViewer : UserControl, IViewer
    {
        private SpriteFontContent _spf;
        private readonly Dictionary<CharacterRegion, string> _specialRegions;

        public SpriteFontViewer()
        {
            InitializeComponent();
            FillComboBox();

            _specialRegions = new Dictionary<CharacterRegion, string>
            {
                {new CharacterRegion(32, 126), "latin alphabet"},
                {new CharacterRegion(228, 228), "character \"ä\""},
                {new CharacterRegion(246, 246), "character \"ö\""},
                {new CharacterRegion(252, 252), "character \"ü\""},
                {new CharacterRegion(196, 196), "character \"Ä\""},
                {new CharacterRegion(214, 214), "character \"Ö\""},
                {new CharacterRegion(220, 220), "character \"Ü\""},
                {new CharacterRegion(223, 223), "character \"ß\""}
            };
        }

        private bool _historyChanging;

        public Control GetViewerControl(ContentFile file)
        {
            History = new History();
            History.HistoryChanged += (sender, args) => Reload();
            ContentFile = file;
            _spf = new SpriteFontContent(file.FilePath);

            Reload();

            return this;
        }

        private void Reload()
        {
            _historyChanging = true;
            fontComboBox.SelectedItem = _spf.FontName;
            checkBox_bold.Checked = _spf.Style.HasFlag(FontStyle.Bold);
            checkBox_italics.Checked = _spf.Style.HasFlag(FontStyle.Italic);
            numericUpDown_spacing.Value = _spf.Spacing;
            checkBox_kering.Checked = _spf.UseKerning;
            numericUpDown_size.Value = _spf.Size;
            var sel = list_characterRegions.SelectedIndex;
            list_characterRegions.Items.Clear();
            foreach (var region in _spf.CharacterRegions)
            {
                if (_specialRegions.TryGetValue(region, out var spec))
                    list_characterRegions.Items.Add($"{spec} ({region.Start} - {region.End})");
                else
                    list_characterRegions.Items.Add($"{region.Start} - {region.End}");
            }
            list_characterRegions.SelectedIndex = sel;
            button_remove.Enabled = false;

            UnsavedChanges = false;
            _historyChanging = false;
        }

        private void ListCharacterRegionsOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            button_remove.Enabled = list_characterRegions.SelectedIndex != -1;
        }

        private void ButtonRemoveOnClick(object sender, EventArgs eventArgs)
        {
            if (_historyChanging) return;
            if (list_characterRegions.SelectedIndex == -1)
                return;
            var oldIndex = list_characterRegions.SelectedIndex;
            list_characterRegions.Items.RemoveAt(list_characterRegions.SelectedIndex);
            var oldRegion = _spf.CharacterRegions[oldIndex];
            _spf.CharacterRegions.RemoveAt(oldIndex);
            History.Push(new HistoryCollectionChange<CharacterRegion>(_spf.CharacterRegions,
                NotifyCollectionChangedAction.Remove, new List<CharacterRegion>() {oldRegion}, null, oldIndex));
        }

        private void fontComboBox_DropDown(object sender, EventArgs e)
        {
            FillComboBox();
        }

        public void Save()
        {
            _spf.Save(ContentFile.FilePath);
            UnsavedChanges = false;
        }

        public void Discard()
        {
            UnsavedChanges = false;
        }

        public IHistory History { get; private set; }

        public bool UnsavedChanges { get; private set; }
        public ContentFile ContentFile { get; private set; }

        private void FillComboBox()
        {
            var selected = fontComboBox.SelectedItem?.ToString();
            //_cmbFontName.Items.Clear();
            var families = FontFamily.Families.Select(x => x.Name).Distinct().ToArray();
            Array.Sort(families);
            for (var i = fontComboBox.Items.Count - 1; i >= 0; i--)
            {
                if (!families.Contains(fontComboBox.Items[i]?.ToString()))
                    fontComboBox.Items.RemoveAt(i);
            }
            for (var i = 0; i < families.Length; i++)
            {
                if (!fontComboBox.Items.Contains(families[i]))
                {
                    //Debug.WriteLine($"{i} : {families[i]}");
                    fontComboBox.Items.Insert(i, families[i]);
                }
            }
            fontComboBox.SelectedItem = selected;
        }

        private void NumericUpDownSpacingOnValueChanged(object sender, EventArgs eventArgs)
        {
            if (_historyChanging) return;
            var old = _spf.Spacing;
            _spf.Spacing = (int) numericUpDown_spacing.Value;
            History.Push(new HistoryPropertyChange(_spf, nameof(_spf.Spacing), old, _spf.Spacing));
            UnsavedChanges = true;
        }

        private void NumericUpDownSizeOnValueChanged(object sender, EventArgs eventArgs)
        {
            if (_historyChanging) return;
            var old = _spf.Size;
            _spf.Size = (int) numericUpDown_size.Value;
            History.Push(new HistoryPropertyChange(_spf, nameof(_spf.Size), old, _spf.Size));
            UnsavedChanges = true;
        }

        private void checkBox_bold_CheckedChanged(object sender, EventArgs e)
        {
            if (_historyChanging) return;
            var old = _spf.Style;
            var fontStyle = FontStyle.Regular;
            if (checkBox_bold.Checked)
                fontStyle |= FontStyle.Bold;
            if (checkBox_italics.Checked)
                fontStyle |= FontStyle.Italic;

            _spf.Style = fontComboBox.FontStyle = fontStyle;
            History.Push(new HistoryPropertyChange(_spf, nameof(_spf.Style), old, _spf.Style));
            UnsavedChanges = true;
        }
    }
}