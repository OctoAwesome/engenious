using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ContentTool.Controls
{
    public class FontComboBox : ComboBox
    {
        #region  Private Member Declarations  

        private readonly Dictionary<string, Font> _fontCache;
        private int _itemHeight;
        private int _previewFontSize;
        private StringFormat _stringFormat;
        private FontStyle _fontStyle;

        #endregion  Private Member Declarations  

        #region  Public Constructors  

        public FontComboBox()
        {
            _fontCache = new Dictionary<string, Font>();

            DrawMode = DrawMode.OwnerDrawVariable;
            Sorted = true;
            PreviewFontSize = 12;


            CalculateLayout();
            CreateStringFormat();
        }

        #endregion  Public Constructors  

        #region  Events  

        public event EventHandler PreviewFontSizeChanged;

        #endregion  Events  

        #region  Protected Overridden Methods  

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            base.OnSelectedValueChanged(e);
            if (SelectedItem == null)
                return;
            var fnt = (Font) GetFont(SelectedItem.ToString())?.Clone();
            if (fnt != null)
            {
                Font = fnt;
                //Height = TextRenderer.MeasureText(SelectedItem.ToString(), Font).Height + 4;
            }
        }

        protected override void Dispose(bool disposing)
        {
            ClearFontCache();

            _stringFormat?.Dispose();

            base.Dispose(disposing);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (e.Index > -1 && e.Index < Items.Count)
            {
                e.DrawBackground();

                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                    e.DrawFocusRectangle();

                using (var textBrush = new SolidBrush(e.ForeColor))
                {
                    string fontFamilyName;

                    fontFamilyName = Items[e.Index].ToString();
                    e.Graphics.DrawString(fontFamilyName, GetFont(fontFamilyName),
                        textBrush, e.Bounds, _stringFormat);
                }
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            CalculateLayout();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            LoadFontFamilies();

            base.OnGotFocus(e);
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            if (e.Index > -1 && e.Index < Items.Count)
            {
                e.ItemHeight = _itemHeight;
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            CreateStringFormat();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (Items.Count == 0)
            {
                int selectedIndex;

                LoadFontFamilies();

                selectedIndex = FindStringExact(Text);
                if (selectedIndex != -1)
                    SelectedIndex = selectedIndex;
            }
        }

        #endregion  Protected Overridden Methods  

        #region  Public Methods  

        public virtual void LoadFontFamilies()
        {
            if (Items.Count == 0)
            {
                Cursor.Current = Cursors.WaitCursor;

                foreach (var fontFamily in FontFamily.Families)
                    Items.Add(fontFamily.Name);

                Cursor.Current = Cursors.Default;
            }
        }

        #endregion  Public Methods  

        #region  Public Properties  

        [Browsable(false), DesignerSerializationVisibility
         (DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new DrawMode DrawMode
        {
            get { return base.DrawMode; }
            set { base.DrawMode = value; }
        }

        [Category("Appearance"), DefaultValue(12)]
        public int PreviewFontSize
        {
            get { return _previewFontSize; }
            set
            {
                _previewFontSize = value;

                OnPreviewFontSizeChanged(EventArgs.Empty);
            }
        }

        [Category("Appearance"), DefaultValue(FontStyle.Regular)]
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value;
                Font = new Font(Font, value);
            }
        }

        [Browsable(false), DesignerSerializationVisibility
         (DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new bool Sorted
        {
            get { return base.Sorted; }
            set { base.Sorted = value; }
        }

        #endregion  Public Properties  

        #region  Private Methods  

        private void CalculateLayout()
        {
            ClearFontCache();

            using (var font = new Font(Font.FontFamily, PreviewFontSize))
            {
                Size textSize;

                textSize = TextRenderer.MeasureText("yY", font);
                _itemHeight = textSize.Height + 2;
            }
        }

        private bool IsUsingRTL(Control control)
        {
            bool result;

            if (control.RightToLeft == RightToLeft.Yes)
                result = true;
            else if (control.RightToLeft == RightToLeft.Inherit && control.Parent != null)
                result = IsUsingRTL(control.Parent);
            else
                result = false;

            return result;
        }

        #endregion  Private Methods  

        #region  Protected Methods  

        protected virtual void ClearFontCache()
        {
            if (_fontCache != null)
            {
                foreach (var key in _fontCache.Keys)
                    _fontCache[key].Dispose();
                _fontCache.Clear();
            }
        }

        protected void CreateStringFormat()
        {
            _stringFormat?.Dispose();

            _stringFormat = new StringFormat(StringFormatFlags.NoWrap);
            _stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            _stringFormat.HotkeyPrefix = HotkeyPrefix.None;
            _stringFormat.Alignment = StringAlignment.Near;
            _stringFormat.LineAlignment = StringAlignment.Center;

            if (IsUsingRTL(this))
                _stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
        }

        protected virtual Font GetFont(string fontFamilyName)
        {
            lock (_fontCache)
            {
                if (_fontCache.ContainsKey(fontFamilyName))
                    return _fontCache[fontFamilyName];
                var font = GetFont(fontFamilyName, FontStyle.Regular)
                           ?? GetFont(fontFamilyName, FontStyle.Bold)
                           ?? GetFont(fontFamilyName, FontStyle.Italic)
                           ?? GetFont(fontFamilyName, FontStyle.Bold | FontStyle.Italic)
                           ?? (Font) Font.Clone();

                _fontCache.Add(fontFamilyName, font);
            }

            return _fontCache[fontFamilyName];
        }

        protected virtual Font GetFont(string fontFamilyName, FontStyle fontStyle)
        {
            Font font;

            try
            {
                font = new Font(fontFamilyName, PreviewFontSize, fontStyle);
            }
            catch
            {
                font = null;
            }

            return font;
        }

        protected virtual void OnPreviewFontSizeChanged(EventArgs e)
        {
            PreviewFontSizeChanged?.Invoke(this, e);

            CalculateLayout();
        }

        #endregion  Protected Methods  
    }
}