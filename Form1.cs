using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FlacMove
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog1.FileName;
                File_Move(fName);
            }
            else
                return;
        }

        private void File_Move(string fileName)
        {
            richTextBox1.Clear();
            richTextBox1.AppendText("プロセスを開始します\r\n");
            richTextBox1.Update();
            string fNameOrigin; //拡張子を抜いたファイル名を取得する
            fNameOrigin = fileName.Remove(0, 3);
            fNameOrigin = fNameOrigin.Replace(".flac", "");
            string fNameCue = fNameOrigin + ".cue"; //cueのファイル名を取得する

            int brk = fNameOrigin.IndexOf(" - "); //ファイル名のミュージシャン/アルバム名が分かれる部分を検索
            int fLength = fNameOrigin.Length;

            string musicianName = fNameOrigin.Remove(brk, fLength - (brk)); //ミュージシャン名を抜き出す
            string albumName = fNameOrigin.Remove(0, brk + 3); //アルバム名を抜き出す

            //string newDirectory = "B:\\" + musicianName + "\\" + albumName; //ファイルの格納ディレクトリを作成
            //System.IO.Directory.CreateDirectory(newDirectory);

            StreamReader sr = new StreamReader(@"B:\" + fNameCue, Encoding.GetEncoding("Shift_JIS")); //元々のcueファイルを読み込み、wav --> flacに変更して保存
            string s = sr.ReadToEnd();
            sr.Close();
            s = s.Replace(".wav\" WAVE", ".flac\" FLAC");
            //System.Diagnostics.Debug.Write(s);
            StreamWriter sw = new StreamWriter(@"B:\" + fNameCue, false, Encoding.Default);
            sw.Write(s);
            sw.Close();

            richTextBox1.AppendText("cueファイルを書き換えました。\r\n");
            richTextBox1.Update();
            System.IO.FileInfo fi_flac = new System.IO.FileInfo(@"B:\" + fNameOrigin + ".flac");
            System.IO.FileInfo fi_cue = new System.IO.FileInfo(@"B:\" + fNameOrigin + ".cue");
            System.IO.FileInfo fi_albumart = new System.IO.FileInfo(@"B:\Folder.jpg");

            //Aドライブにコピー
            string ADirectory = @"A:\Music\flac\" + musicianName + "\\" + albumName + "\\";
            System.IO.Directory.CreateDirectory(ADirectory);
            try
            {
                fi_flac.CopyTo(@"A:\Music\flac\" + musicianName + "\\" + albumName + "\\" + fNameOrigin + ".flac", true);
                fi_cue.CopyTo(@"A:\Music\flac\" + musicianName + "\\" + albumName + "\\" + fNameOrigin + ".cue", true);
                fi_albumart.CopyTo(@"A:\Music\flac\" + musicianName + "\\" + albumName + "\\Folder.jpg", true);
                richTextBox1.AppendText("Aドライブにファイルが格納されました\r\n");
                richTextBox1.Update();
            }
            catch (System.IO.FileNotFoundException) 
            {
                MessageBox.Show("ファイルが足りません", "Caution");
                return;
            }
            
            //Bドライブにコピー
            string BDirectory = @"B:\_Music\flac\" + musicianName + "\\" + albumName + "\\";
            System.IO.Directory.CreateDirectory(BDirectory);
            fi_flac.CopyTo(@"B:\_Music\flac\" + musicianName + "\\" + albumName + "\\" + fNameOrigin + ".flac", true);
            fi_cue.CopyTo(@"B:\_Music\flac\" + musicianName + "\\" + albumName + "\\" + fNameOrigin + ".cue", true);
            fi_albumart.CopyTo(@"B:\_Music\flac\" + musicianName + "\\" + albumName + "\\Folder.jpg", true);
            richTextBox1.AppendText("Bドライブにファイルが格納されました\r\n");
            richTextBox1.Update();
            //Bドライブ直下にコピー(mp3に変換、iTunesに登録しやすいように)
            if (checkBox1.Checked == true)
            {
                string BDirectory2 = @"B:\" + musicianName + "\\" + albumName + "\\";
                System.IO.Directory.CreateDirectory(BDirectory2);
                fi_flac.MoveTo(@"B:\" + musicianName + "\\" + albumName + "\\" + fNameOrigin + ".flac");
                fi_cue.MoveTo(@"B:\" + musicianName + "\\" + albumName + "\\" + fNameOrigin + ".cue");
                fi_albumart.MoveTo(@"B:\" + musicianName + "\\" + albumName + "\\Folder.jpg");
                richTextBox1.AppendText("Bドライブ直下にファイルが格納されました\r\n");
                richTextBox1.Update();
            }
            else
            {
                fi_flac.Delete();
                fi_cue.Delete();
                fi_albumart.Delete();
            }
            System.IO.File.Delete(@"B:\" + fNameOrigin + ".wav");
            richTextBox1.AppendText("★すべてのプロセスが終了しました☆\r\n");
        }
    }
}
