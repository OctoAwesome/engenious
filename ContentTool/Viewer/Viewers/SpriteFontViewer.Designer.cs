using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ContentTool.Controls;

namespace ContentTool.Viewer.Viewers
{
    partial class SpriteFontViewer
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new SplitContainer();
            this.panel1 = new Panel();
            this.button_save = new Button();
            this.groupBox3 = new GroupBox();
            this.button_remove = new Button();
            this.button_add = new Button();
            this.list_characterRegions = new ListBox();
            this.groupBox1 = new GroupBox();
            this.fontComboBox = new FontComboBox();
            this.checkBox_italics = new CheckBox();
            this.checkBox_bold = new CheckBox();
            this.numericUpDown_size = new NumericUpDown();
            this.label2 = new Label();
            this.label1 = new Label();
            this.groupBox2 = new GroupBox();
            this.checkBox_kering = new CheckBox();
            this.label3 = new Label();
            this.numericUpDown_spacing = new NumericUpDown();
            this.previewBox = new RichTextBox();
            ((ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDown_size)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDown_spacing)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.Location = new Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.previewBox);
            this.splitContainer.Size = new Size(250, 401);
            this.splitContainer.SplitterDistance = 339;
            this.splitContainer.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button_save);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(250, 339);
            this.panel1.TabIndex = 0;
            // 
            // button_save
            // 
            this.button_save.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.button_save.Location = new Point(172, 314);
            this.button_save.Name = "button_save";
            this.button_save.Size = new Size(75, 23);
            this.button_save.TabIndex = 8;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.button_remove);
            this.groupBox3.Controls.Add(this.button_add);
            this.groupBox3.Controls.Add(this.list_characterRegions);
            this.groupBox3.Location = new Point(3, 154);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(244, 154);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Character Regions";
            // 
            // button_remove
            // 
            this.button_remove.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.button_remove.Location = new Point(89, 125);
            this.button_remove.Name = "button_remove";
            this.button_remove.Size = new Size(75, 23);
            this.button_remove.TabIndex = 2;
            this.button_remove.Text = "Remove";
            this.button_remove.UseVisualStyleBackColor = true;
            this.button_remove.Click += ButtonRemoveOnClick;
            // 
            // button_add
            // 
            this.button_add.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.button_add.Location = new Point(6, 125);
            this.button_add.Name = "button_add";
            this.button_add.Size = new Size(75, 23);
            this.button_add.TabIndex = 1;
            this.button_add.Text = "Add";
            this.button_add.UseVisualStyleBackColor = true;
            // 
            // list_characterRegions
            // 
            this.list_characterRegions.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.list_characterRegions.Location = new Point(6, 19);
            this.list_characterRegions.Name = "list_characterRegions";
            this.list_characterRegions.Size = new Size(232, 100);
            this.list_characterRegions.TabIndex = 0;
            this.list_characterRegions.SelectedIndexChanged += ListCharacterRegionsOnSelectedIndexChanged;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.fontComboBox);
            this.groupBox1.Controls.Add(this.checkBox_italics);
            this.groupBox1.Controls.Add(this.checkBox_bold);
            this.groupBox1.Controls.Add(this.numericUpDown_size);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(244, 87);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // fontComboBox
            // 
            this.fontComboBox.FormattingEnabled = true;
            this.fontComboBox.ItemHeight = 25;
            this.fontComboBox.Items.AddRange(new object[] {
            "Agency FB",
            "Algerian",
            "Arial",
            "Arial Black",
            "Arial Narrow",
            "Arial Rounded MT Bold",
            "Baskerville Old Face",
            "Bauhaus 93",
            "Bell MT",
            "Berlin Sans FB",
            "Berlin Sans FB Demi",
            "Bernard MT Condensed",
            "Blackadder ITC",
            "Bodoni MT",
            "Bodoni MT Black",
            "Bodoni MT Condensed",
            "Bodoni MT Poster Compressed",
            "Book Antiqua",
            "Bookman Old Style",
            "Bookshelf Symbol 7",
            "Bradley Hand ITC",
            "Britannic Bold",
            "Broadway",
            "Brush Script MT",
            "Calibri",
            "Calibri Light",
            "Californian FB",
            "Calisto MT",
            "Cambria",
            "Cambria Math",
            "Candara",
            "Castellar",
            "Centaur",
            "Century",
            "Century Gothic",
            "Century Schoolbook",
            "Chiller",
            "Colonna MT",
            "Comic Sans MS",
            "Consolas",
            "Constantia",
            "Cooper Black",
            "Copperplate Gothic Bold",
            "Copperplate Gothic Light",
            "Corbel",
            "Courier New",
            "Curlz MT",
            "Dubai",
            "Dubai Light",
            "Dubai Medium",
            "Ebrima",
            "Edwardian Script ITC",
            "Elephant",
            "Engravers MT",
            "Eras Bold ITC",
            "Eras Demi ITC",
            "Eras Light ITC",
            "Eras Medium ITC",
            "Felix Titling",
            "Footlight MT Light",
            "Forte",
            "Franklin Gothic Book",
            "Franklin Gothic Demi",
            "Franklin Gothic Demi Cond",
            "Franklin Gothic Heavy",
            "Franklin Gothic Medium",
            "Franklin Gothic Medium Cond",
            "Freestyle Script",
            "French Script MT",
            "Gabriola",
            "Gadugi",
            "Garamond",
            "Georgia",
            "Gigi",
            "Gill Sans MT",
            "Gill Sans MT Condensed",
            "Gill Sans MT Ext Condensed Bold",
            "Gill Sans Ultra Bold",
            "Gill Sans Ultra Bold Condensed",
            "Gloucester MT Extra Condensed",
            "Goudy Old Style",
            "Goudy Stout",
            "Haettenschweiler",
            "Harlow Solid Italic",
            "Harrington",
            "High Tower Text",
            "HoloLens MDL2 Assets",
            "Impact",
            "Imprint MT Shadow",
            "Informal Roman",
            "Javanese Text",
            "Jokerman",
            "Juice ITC",
            "Kristen ITC",
            "Kunstler Script",
            "Leelawadee",
            "Leelawadee UI",
            "Leelawadee UI Semilight",
            "Lucida Bright",
            "Lucida Calligraphy",
            "Lucida Console",
            "Lucida Fax",
            "Lucida Handwriting",
            "Lucida Sans",
            "Lucida Sans Typewriter",
            "Lucida Sans Unicode",
            "Magneto",
            "Maiandra GD",
            "Malgun Gothic",
            "Malgun Gothic Semilight",
            "Marlett",
            "Matura MT Script Capitals",
            "Microsoft Himalaya",
            "Microsoft JhengHei",
            "Microsoft JhengHei Light",
            "Microsoft JhengHei UI",
            "Microsoft JhengHei UI Light",
            "Microsoft New Tai Lue",
            "Microsoft Office Preview Font",
            "Microsoft PhagsPa",
            "Microsoft Sans Serif",
            "Microsoft Tai Le",
            "Microsoft Uighur",
            "Microsoft YaHei",
            "Microsoft YaHei Light",
            "Microsoft YaHei UI",
            "Microsoft YaHei UI Light",
            "Microsoft Yi Baiti",
            "MingLiU-ExtB",
            "MingLiU_HKSCS-ExtB",
            "Mistral",
            "Modern No. 20",
            "Mongolian Baiti",
            "Monotype Corsiva",
            "MS Gothic",
            "MS Office Symbol (de-de) Bold",
            "MS Office Symbol (de-de) Light",
            "MS Office Symbol Extralight",
            "MS Office Symbol Regular",
            "MS Office Symbol Semibold",
            "MS Office Symbol Semilight",
            "MS Outlook",
            "MS PGothic",
            "MS Reference Sans Serif",
            "MS Reference Specialty",
            "MS UI Gothic",
            "MT Extra",
            "MV Boli",
            "Myanmar Text",
            "Niagara Engraved",
            "Niagara Solid",
            "Nirmala UI",
            "Nirmala UI Semilight",
            "NSimSun",
            "OCR A Extended",
            "Old English Text MT",
            "Onyx",
            "Palace Script MT",
            "Palatino Linotype",
            "Papyrus",
            "Parchment",
            "Perpetua",
            "Perpetua Titling MT",
            "Playbill",
            "PMingLiU-ExtB",
            "Poor Richard",
            "Pristina",
            "Rage Italic",
            "Ravie",
            "Rockwell",
            "Rockwell Condensed",
            "Rockwell Extra Bold",
            "Script MT Bold",
            "Segoe MDL2 Assets",
            "Segoe Print",
            "Segoe Script",
            "Segoe UI",
            "Segoe UI Black",
            "Segoe UI Emoji",
            "Segoe UI Historic",
            "Segoe UI Light",
            "Segoe UI Semibold",
            "Segoe UI Semilight",
            "Segoe UI Symbol",
            "Showcard Gothic",
            "SimSun",
            "SimSun-ExtB",
            "Sitka Banner",
            "Sitka Display",
            "Sitka Heading",
            "Sitka Small",
            "Sitka Subheading",
            "Sitka Text",
            "Snap ITC",
            "Stencil",
            "Sylfaen",
            "Symbol",
            "Tahoma",
            "TeamViewer12",
            "Tempus Sans ITC",
            "Times New Roman",
            "Trebuchet MS",
            "Tw Cen MT",
            "Tw Cen MT Condensed",
            "Tw Cen MT Condensed Extra Bold",
            "Verdana",
            "Viner Hand ITC",
            "Vivaldi",
            "Vladimir Script",
            "Webdings",
            "Wide Latin",
            "Wingdings",
            "Wingdings 2",
            "Wingdings 3",
            "Yu Gothic",
            "Yu Gothic Light",
            "Yu Gothic Medium",
            "Yu Gothic UI",
            "Yu Gothic UI Light",
            "Yu Gothic UI Semibold",
            "Yu Gothic UI Semilight"});
            this.fontComboBox.Location = new Point(43, 19);
            this.fontComboBox.Name = "fontComboBox";
            this.fontComboBox.Size = new Size(195, 31);
            this.fontComboBox.TabIndex = 9;
            this.fontComboBox.DropDown += new EventHandler(this.fontComboBox_DropDown);
            // 
            // checkBox_italics
            // 
            this.checkBox_italics.AutoSize = true;
            this.checkBox_italics.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic, GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_italics.Location = new Point(63, 64);
            this.checkBox_italics.Name = "checkBox_italics";
            this.checkBox_italics.Size = new Size(53, 17);
            this.checkBox_italics.TabIndex = 8;
            this.checkBox_italics.Text = "Italics";
            this.checkBox_italics.UseVisualStyleBackColor = true;
            this.checkBox_italics.CheckedChanged += new EventHandler(this.checkBox_bold_CheckedChanged);
            // 
            // checkBox_bold
            // 
            this.checkBox_bold.AutoSize = true;
            this.checkBox_bold.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_bold.Location = new Point(6, 64);
            this.checkBox_bold.Name = "checkBox_bold";
            this.checkBox_bold.Size = new Size(51, 17);
            this.checkBox_bold.TabIndex = 7;
            this.checkBox_bold.Text = "Bold";
            this.checkBox_bold.UseVisualStyleBackColor = true;
            this.checkBox_bold.CheckedChanged += new EventHandler(this.checkBox_bold_CheckedChanged);
            // 
            // numericUpDown_size
            // 
            this.numericUpDown_size.Location = new Point(170, 63);
            this.numericUpDown_size.Name = "numericUpDown_size";
            this.numericUpDown_size.Size = new Size(68, 20);
            this.numericUpDown_size.TabIndex = 3;
            this.numericUpDown_size.ValueChanged += NumericUpDownSizeOnValueChanged;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(134, 64);
            this.label2.Name = "label2";
            this.label2.Size = new Size(30, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Size:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Font:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.checkBox_kering);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numericUpDown_spacing);
            this.groupBox2.Location = new Point(5, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(242, 52);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Advanced";
            // 
            // checkBox_kering
            // 
            this.checkBox_kering.AutoSize = true;
            this.checkBox_kering.Location = new Point(135, 20);
            this.checkBox_kering.Name = "checkBox_kering";
            this.checkBox_kering.Size = new Size(84, 17);
            this.checkBox_kering.TabIndex = 7;
            this.checkBox_kering.Text = "Use Kerning";
            this.checkBox_kering.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new Size(49, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Spacing:";
            // 
            // numericUpDown_spacing
            // 
            this.numericUpDown_spacing.Location = new Point(61, 19);
            this.numericUpDown_spacing.Name = "numericUpDown_spacing";
            this.numericUpDown_spacing.Size = new Size(45, 20);
            this.numericUpDown_spacing.TabIndex = 5;
            this.numericUpDown_spacing.ValueChanged += NumericUpDownSpacingOnValueChanged;
            // 
            // previewBox
            // 
            this.previewBox.Dock = DockStyle.Fill;
            this.previewBox.Location = new Point(0, 0);
            this.previewBox.Name = "previewBox";
            this.previewBox.ReadOnly = true;
            this.previewBox.Size = new Size(250, 58);
            this.previewBox.TabIndex = 0;
            this.previewBox.Text = "";
            // 
            // SpriteFontViewer
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new Size(250, 400);
            this.Name = "SpriteFontViewer";
            this.Size = new Size(250, 401);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((ISupportInitialize)(this.numericUpDown_size)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((ISupportInitialize)(this.numericUpDown_spacing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer;
        private Panel panel1;
        private RichTextBox previewBox;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private CheckBox checkBox_kering;
        private Label label3;
        private NumericUpDown numericUpDown_spacing;
        private NumericUpDown numericUpDown_size;
        private Label label2;
        private Label label1;
        private CheckBox checkBox_italics;
        private CheckBox checkBox_bold;
        private GroupBox groupBox3;
        private Button button_remove;
        private Button button_add;
        private ListBox list_characterRegions;
        private Button button_save;
        private FontComboBox fontComboBox;
    }
}
