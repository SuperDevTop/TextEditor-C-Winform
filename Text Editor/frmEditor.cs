/*
 * Programmer: Hunter Johnson
 * Name: Rich Text Editor
 * Date: November 1, 2016 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Text_Editor
{
    public partial class frmEditor : Form
    {
        List<string> colorList = new List<string>();    // holds the System.Drawing.Color names
        string filenamee;    // file opened inside of RTB
        const int MIDDLE = 382;    // middle sum of RGB - max is 765
        int sumRGB;    // sum of the selected colors RGB
        int pos, line, column;    // for detecting line and column numbers
        private string str_CurPath = "";

        public frmEditor()
        {
            InitializeComponent();
        }

        private void frmEditor_Load(object sender, EventArgs e)
        {
            richTextBox1.AllowDrop = true;     // to allow drag and drop to the RichTextBox
            richTextBox1.AcceptsTab = true;    // allow tab
            richTextBox1.WordWrap = false;    // disable word wrap on start
            richTextBox1.ShortcutsEnabled = true;    // allow shortcuts
            richTextBox1.DetectUrls = true;    // allow detect url
            fontDialog1.ShowColor = true;
            fontDialog1.ShowApply = true;
            fontDialog1.ShowHelp = true;
            colorDialog1.AllowFullOpen = true;
            colorDialog1.AnyColor = true;
            colorDialog1.SolidColorOnly = false;
            colorDialog1.ShowHelp = true;
            colorDialog1.AnyColor = true;
            boldStripButton3.Checked = false;
            italicStripButton.Checked = false;
            wordWrapToolStripMenuItem.Image = null;
            MinimizeBox = false;
            MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            if(Login.myType == "Edit")
            {
                richTextBox1.Enabled = true;
            }
            else
            {
                richTextBox1.Enabled = false;
            }
                        
            // fill font sizes in combo box
            for (int i = 8; i < 80; i += 2)
            {
                fontSizeComboBox.Items.Add(i);
            }

            // fill colors in color drop down list
            foreach (System.Reflection.PropertyInfo prop in typeof(Color).GetProperties())
            {
                if (prop.PropertyType.FullName == "System.Drawing.Color")
                {
                    colorList.Add(prop.Name);     
                }
            }           

            // fill fonts in font combo box
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
            {
                fontStripComboBox.Items.Add(family.Name);
            }

            // determines the line and column numbers of mouse position on the richTextBox
            int pos = richTextBox1.SelectionStart;
            int line = richTextBox1.GetLineFromCharIndex(pos);
            int column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(line);
            lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
        }
        
        //******************************************************************************************************************************
        // ConvertToRGB - Accepts a Color object as its parameter. Gets the RGB values of the object passed to it, calculates the sum. *
        //******************************************************************************************************************************
        private int ConvertToRGB(System.Drawing.Color c)
        {
            int r = c.R, // RED component value
                g = c.G, // GREEN component value
                b = c.B; // BLUE component value
            int sum = 0;

            // calculate sum of RGB
            sum = r + g + b;

            return sum;
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();     // select all text
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // clear
            richTextBox1.Clear();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();     // paste text
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();      // copy text
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();     // cut text
        }

        private void boldStripButton3_Click(object sender, EventArgs e)
        {
           
            if (boldStripButton3.Checked == false)
            {
                boldStripButton3.Checked = true; // BOLD is true
            }
            else if (boldStripButton3.Checked == true)
            {
                boldStripButton3.Checked = false;    // BOLD is false
            }

            if (richTextBox1.SelectionFont == null)
            {
                return;
            }

            // create fontStyle object
            FontStyle style = richTextBox1.SelectionFont.Style;

            // determines the font style
            if (richTextBox1.SelectionFont.Bold)
            {
                style &= ~FontStyle.Bold; 
            }
            else
            {
                style |= FontStyle.Bold;

            }
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, style);     // sets the font style
        }

        private void underlineStripButton_Click(object sender, EventArgs e)
        {
            if (underlineStripButton.Checked == false)
            {
                underlineStripButton.Checked = true;     // UNDERLINE is active
            }
            else if (underlineStripButton.Checked == true)
            {
                underlineStripButton.Checked = false;    // UNDERLINE is not active
            }

            if (richTextBox1.SelectionFont == null)
            {
                return;
            }

            // create fontStyle object
            FontStyle style = richTextBox1.SelectionFont.Style;

            // determines the font style
            if (richTextBox1.SelectionFont.Underline)
            {
                style &= ~FontStyle.Underline;
            }
            else
            {
                style |= FontStyle.Underline;
            }
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, style);    // sets the font style
        }

        private void italicStripButton_Click(object sender, EventArgs e)
        {
            if (italicStripButton.Checked == false)
            {
                italicStripButton.Checked = true;    // ITALICS is active
            }
            else if (italicStripButton.Checked == true)
            {
                italicStripButton.Checked = false;    // ITALICS is not active
            }

            if (richTextBox1.SelectionFont == null)
            {
                return;
            }
            // create fontStyle object
            FontStyle style = richTextBox1.SelectionFont.Style;

            // determines font style
            if (richTextBox1.SelectionFont.Italic)
            {
                style &= ~FontStyle.Italic;
            }
            else
            {
                style |= FontStyle.Italic;
            }

            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, style);    // sets the font style
        }

        private void fontSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionFont == null)
            {
                return;
            }

            // sets the font size when changed
            richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily,Convert.ToInt32(fontSizeComboBox.Text),richTextBox1.SelectionFont.Style);
        }

        private void fontStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionFont == null)
            {
                // sets the Font Family style
                richTextBox1.SelectionFont = new Font(fontStripComboBox.Text, richTextBox1.Font.Size);
            }

            // sets the selected font famly style
            richTextBox1.SelectionFont = new Font(fontStripComboBox.Text, richTextBox1.SelectionFont.Size);
        }

        private void saveStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (str_CurPath != "")
                    richTextBox1.SaveFile(str_CurPath, RichTextBoxStreamType.PlainText);
                else
                    toolStripMenuItem1_Click(sender, e);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void openFileStripButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filenamee = openFileDialog1.FileName;
                // load the file into the richTextBox
                str_CurPath = openFileDialog1.FileName;
                richTextBox1.LoadFile(filenamee, RichTextBoxStreamType.PlainText);    // loads it in regular text format
            }
        }

        private void colorStripDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // creates a KnownColor object
            KnownColor selectedColor;
            selectedColor = (KnownColor)System.Enum.Parse(typeof(KnownColor), e.ClickedItem.Text);    // converts it to a Color Structure
            richTextBox1.SelectionColor = Color.FromKnownColor(selectedColor);    // sets the selected color
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            // highlight button border when buttons are true
            if (richTextBox1.SelectionFont != null)
            {
                boldStripButton3.Checked = richTextBox1.SelectionFont.Bold;
                italicStripButton.Checked = richTextBox1.SelectionFont.Italic;
                underlineStripButton.Checked = richTextBox1.SelectionFont.Underline;
            }
        }

        private void increaseStripButton_Click(object sender, EventArgs e)
        {
            string fontSizeNum = fontSizeComboBox.Text;    // variable to hold selected size         
            try
            {
                int size = Convert.ToInt32(fontSizeNum) + 1;    // convert (fontSizeNum + 1)
                if (richTextBox1.SelectionFont == null)
                {
                    return;
                }
                // sets the updated font size
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily,size,richTextBox1.SelectionFont.Style);
                fontSizeComboBox.Text = size.ToString();    // update font size
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Warning); // show error message
            }
        }

        private void decreaseStripButton_Click(object sender, EventArgs e)
        {
            string fontSizeNum = fontSizeComboBox.Text;    // variable to hold selected size            
            try
            {
                int size = Convert.ToInt32(fontSizeNum) - 1;    // convert (fontSizeNum - 1)
                if (richTextBox1.SelectionFont == null)
                {
                    return;
                }
                // sets the updated font size
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily,size,richTextBox1.SelectionFont.Style);
                fontSizeComboBox.Text = size.ToString();    // update font size
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Information", MessageBoxButtons.OK, MessageBoxIcon.Warning); // show error message
            }
        }

        //*********************************************************************************************
        // richTextBox1_DragEnter - Custom Event. Copies text being dragged into the richTextBox      *
        //*********************************************************************************************
        private void richTextBox1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;    // copies data to the RTB
            else
                e.Effect = DragDropEffects.None;    // doesn't accept data into RTB
        }
        //***************************************************************************************************
        // richTextBox1_DragEnter - Custom Event. Drops the copied text being dragged onto the richTextBox  *
        //***************************************************************************************************
        private void richTextBox1_DragDrop(object sender,System.Windows.Forms.DragEventArgs e)
        {
            // variables
            int i;
            String s;

            // Get start position to drop the text.
            i = richTextBox1.SelectionStart;
            s = richTextBox1.Text.Substring(i);
            richTextBox1.Text = richTextBox1.Text.Substring(0, i);

            // Drop the text on to the RichTextBox.
            richTextBox1.Text += e.Data.GetData(DataFormats.Text).ToString();
            richTextBox1.Text += s;
        }

        private void undoStripButton_Click(object sender, EventArgs e)
        {           
            richTextBox1.Undo();     // undo move
        }

        private void redoStripButton_Click(object sender, EventArgs e)
        {            
            richTextBox1.Redo();    // redo move
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            this.Close();     // close the form
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            richTextBox1.Undo();     // undo move
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            richTextBox1.Redo();     // redo move
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            richTextBox1.Cut();     // cut text
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            richTextBox1.Copy();     // copy text
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {           
            richTextBox1.Paste();    // paste text
        }

        private void selectAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            richTextBox1.SelectAll();    // select all text
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // clear the rich text box
            richTextBox1.Clear();
            richTextBox1.Focus();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // delete selected text
            richTextBox1.SelectedText = "";
            richTextBox1.Focus();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                str_CurPath = openFileDialog1.FileName;
            }
        }

        private void newMenuItem_Click(object sender, EventArgs e)
        {
            
            if (richTextBox1.Text != string.Empty)    // RTB has contents - prompt user to save changes
            {
               // save changes message
               DialogResult result =  MessageBox.Show("Would you like to save your changes? Editor is not empty.", "Save Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if(result == DialogResult.Yes)
                {
                    // save the RTB contents if user selected yes
                    string file;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;
                        // save the contents of the rich text box
                        richTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);
                        file = Path.GetFileName(filename); // get name of file
                        MessageBox.Show("File " + file + " was saved successfully.", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // finally - clear the contents of the RTB 
                    richTextBox1.ResetText();
                    richTextBox1.Focus();
                }
                else if(result == DialogResult.No)
                {
                    // clear the contents of the RTB 
                    richTextBox1.ResetText();
                    richTextBox1.Focus();
                }               
            }
            else // RTB has no contents
            {
                // clear the contents of the RTB 
                richTextBox1.ResetText();
                richTextBox1.Focus();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (str_CurPath != "")
                richTextBox1.SaveFile(str_CurPath, RichTextBoxStreamType.PlainText);
            else
                toolStripMenuItem1_Click(sender, e);
        }

        private void zoomDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            float zoomPercent = Convert.ToSingle(e.ClickedItem.Text.Trim('%')); // convert
            richTextBox1.ZoomFactor = zoomPercent / 100;    // set zoom factor

            if(e.ClickedItem.Image == null)
            {
                // draw bmp in image property of selected item, while its active
                Bitmap bmp = new Bitmap(5, 5);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.FillEllipse(Brushes.Black, 1, 1, 3, 3);
                }
                e.ClickedItem.Image = bmp;    // draw ellipse in image property
            }
            else
            {
                e.ClickedItem.Image = null;
                richTextBox1.ZoomFactor = 1.0f;    // set back to NO ZOOM
            }
        }

        private void uppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = richTextBox1.SelectedText.ToUpper();    // text to CAPS
        }

        private void lowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = richTextBox1.SelectedText.ToLower();    // text to lowercase
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void fontDialog1_HelpRequest(object sender, EventArgs e)
        {
            // display HelpRequest message
            MessageBox.Show("Please choose a font and any other attributes; then hit Apply and OK.", "Font Dialog Help Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {
            fontDialog1.FontMustExist = true;    // error if font doesn't exist
            
            richTextBox1.Font = fontDialog1.Font;    // set selected font (Includes: FontFamily, Size, and, Effect. No need to set them separately)
            richTextBox1.ForeColor = fontDialog1.Color;    // set selected font color
            
            // sets the font for the controls inside richTextBox1
            foreach (Control containedControl in richTextBox1.Controls)
            {
                containedControl.Font = fontDialog1.Font;
            }
        }

        private void deleteStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = ""; // delete selected text
        }

        private void clearFormattingStripButton_Click(object sender, EventArgs e)
        {
            fontStripComboBox.Text = "Font Family";
            fontSizeComboBox.Text = "Font Size";
            string pureText = richTextBox1.Text;    // get the current Plain Text     
            richTextBox1.Clear();    // clear RTB
            richTextBox1.ForeColor = Color.Black;    // ensure the text color is back to Black
            richTextBox1.Font = default(Font);    // set default font
            richTextBox1.Text = pureText;    // Set it back to its orginial text, added as plain text     
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // draws the string onto the print document
            e.Graphics.DrawString(richTextBox1.Text, richTextBox1.Font, Brushes.Black, 100, 20);
            e.Graphics.PageUnit = GraphicsUnit.Inch; 
        }

        private void printStripButton_Click(object sender, EventArgs e)
        {
            // printDialog associates with PrintDocument
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print(); // Print the document
            }
        }

        private void printPreviewStripButton_Click(object sender, EventArgs e)
        {
            printPreviewDialog.Document = printDocument; 
            // Show PrintPreview Dialog 
            printPreviewDialog.ShowDialog();
        }

        private void printStripMenuItem_Click(object sender, EventArgs e)
        {
            // printDialog associates with PrintDocument
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void printPreviewStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog.Document = printDocument;
            // Show PrintPreview Dialog 
            printPreviewDialog.ShowDialog();
        }

        private void colorDialog1_HelpRequest(object sender, EventArgs e)
        {
            // display HelpRequest message
            MessageBox.Show("Please select a color by clicking it. This will change the text color.", "Color Dialog Help Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void colorOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();

            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                // set the selected color to the RTB's forecolor
                richTextBox1.ForeColor = colorDialog1.Color;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {            
            string file;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                // save the contents of the rich text box
                richTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);
                str_CurPath = filename;
            }
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string file;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;

                // save the contents of the rich text box
                richTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);
                str_CurPath = filename;
            }
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void frmEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.SelectedText);
            richTextBox1.SelectedText = "";
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.SelectedText);
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.SelectedText = "";
                richTextBox1.Text += Clipboard.GetText(TextDataFormat.Text).ToString();
            }
        }

        //****************************************************************************************************************************************
        // richTextBox1_KeyUp - Determines which key was released and gets the line and column numbers of the current cursor position in the RTB *
        //**************************************************************************************************************************************** 
        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            // determine key released
            switch (e.KeyCode)
            {
                case Keys.Down:
                    pos = richTextBox1.SelectionStart;    // get starting point
                    line = richTextBox1.GetLineFromCharIndex(pos);    // get line number
                    column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(line);    // get column number
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;
                case Keys.Right:
                    pos = richTextBox1.SelectionStart; // get starting point
                    line = richTextBox1.GetLineFromCharIndex(pos); // get line number
                    column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(line);    // get column number
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;
                case Keys.Up:
                    pos = richTextBox1.SelectionStart; // get starting point
                    line = richTextBox1.GetLineFromCharIndex(pos); // get line number
                    column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(line);    // get column number
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;
                case Keys.Left:
                    pos = richTextBox1.SelectionStart; // get starting point
                    line = richTextBox1.GetLineFromCharIndex(pos); // get line number
                    column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(line);    // get column number
                    lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
                    break;                
            }
        }

        //****************************************************************************************************************************
        // richTextBox1_MouseDown - Gets the line and column numbers of the cursor position in the RTB when the mouse clicks an area *
        //****************************************************************************************************************************
        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int pos = richTextBox1.SelectionStart;    // get starting point
            int line = richTextBox1.GetLineFromCharIndex(pos);    // get line number
            int column = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(line);    // get column number
            lineColumnStatusLabel.Text = "Line " + (line + 1) + ", Column " + (column + 1);
        }        
    }
}
