using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace fontx2
{
    public partial class mainForm : Form
    {
        Fontx2 fontx2;

        public mainForm()
        {
            InitializeComponent();
        }

        private void finishMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;
                
                if( File.Exists(filename) == false )
                {
                    MessageBox.Show("指定したファイルが存在しません。");
                    return;
                }
                
                FileStream stream = new FileStream(filename, FileMode.Open);
                byte[] data;
                long length = stream.Length;
                data = new byte[length];
                stream.Read(data, 0, (int)length);
                stream.Close();

                fontx2 = new Fontx2();

                if (!fontx2.parse(data))
                {
                    fontx2 = null;
                    statusLabelFontName.Text = "";
                    statusLabelCodeType.Text = "";
                    MessageBox.Show("表示できません。");
                    return;
                }

                statusLabelFontName.Text = fontx2.FontName;
                statusLabelCodeType.Text = ((fontx2.CodeType == 0)? "ASCII" : "2 byte character");
            }
        }
        
        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            if (fontx2 == null)
            {
                return;
            }
            byte width = fontx2.Width;
            byte height = fontx2.Height;
            string inputText = ((TextBox)sender).Text;
            Encoding enc = Encoding.GetEncoding("Shift_JIS");
            Graphics g = pictureBox.CreateGraphics();
            byte[] inputTextBytes = enc.GetBytes(inputText);
            Hashtable ht = fontx2.BitmapList;

            g.Clear(this.BackColor);

            if (fontx2.CodeType == 0)
            {
                for (int index = 0; index < inputTextBytes.Length; index++)
                {
                    int i = (int)inputTextBytes[index];
                    Bitmap bitmap = (Bitmap)ht[i];
                    g.DrawImage(bitmap, index * width, 0);
                    //g.DrawImage((Bitmap)(ht[inputTextBytes[index]]), index * width, 0);
                }
            }
            else
            {
                int x = 0;
                for (int index = 0; index < inputTextBytes.Length; index += 2)
                {
                    int i = (inputTextBytes[index] << 8) + inputTextBytes[index + 1];
                    if (ht.Contains(i))
                    {
                        g.DrawImage((Bitmap)(ht[i]), x, 0);
                        x += width;
                    }
                }
            }

        }
    }
}