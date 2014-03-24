using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.IO;//ファイル書き込み
using System.Text.RegularExpressions;//正規表現

namespace ggt_note
{
    public partial class Form1 : Form
    {
        Size smallsize = new Size(300, 80);
        Size bigsize = new Size(300, 300);
        nb this_nb;
        public Form1()
        {
            this.MinimumSize = smallsize;
            this.MaximumSize = smallsize;
            InitializeComponent();
            this.Size = smallsize;

            this_nb = new nb();//表示されているnb
            this_nb.num = -1;
        }

        public struct nb
        {
            public int num;//上から何番目か(0スタート)
            public string name;//単語
            public string[] body;//説明
        }

        private List<nb> analysize()
        {
            //ファイルをひらいて
            string[] text = System.IO.File.ReadAllLines("data.txt");
            //解析して
            List<string> lines = new List<string>(text);
            List<nb> vals = new List<nb>();

            Regex reg = new Regex(@"^\'\'\'(.+)$");
            int cnt = 0;
            string tname = "";
            nb tnb =new nb();
            List<string> tlines = new List<string>();
            foreach(string l in text)
            {
                Match m = reg.Match(l);//あれ？これなに？
                if (m.Success)//正規表現を使って判定
                {//囲いの始め
                    tname = l.Remove(0,3);
                    tlines.Add(tname + "とは");
                    //tname = $1;//m.Groups[0].Value;
                }
                else if (l == "\'\'\'")
                {//囲いの終わり
                    //追加するtnbの生成
                    tnb.num = cnt;
                    tnb.name = tname;
                    tnb.body = tlines.ToArray();
                    vals.Add(tnb);
                    //初期化
                    cnt++;
                    tname = "";
                    tlines.Clear();
                }
                else if (tname != "")
                {//囲われた中身
                    tlines.Add(l);
                }//囲いの外は無視
            }
            //nbのリストを返す
            return vals;
        }

        private List<nb> get_by_name(string name,List<nb> list)
        {//listの中から名前があうものを持ってくる
            List<nb> val = new List<nb>();
            foreach (nb x in list)
            {
                if (x.name == name)
                {
                    val.Add(x);
                }
            }
            return val;
        }

        private nb get_nb(string name , int i,string option)
        {//i番目を基準に表示すべきnbをさがす
            List<nb> nb_list = analysize();
            List<nb> nb_list_name_fixed = get_by_name(name,nb_list);
            
            nb next_nb = new nb();//i番目からさがして最初
            nb back_nb = new nb();
            next_nb.num = -1;
            back_nb.num = -1;
            foreach(nb x in nb_list_name_fixed)
            {
                if (x.num >= i && next_nb.num == -1)
                { next_nb = x; }
                if (x.num < i)
                { back_nb = x; }
            }//たまにこれでnextとbackが拾えない

            if (nb_list_name_fixed.Count == 0)
            {
                nb error = new nb();
                error.num = -1;
                return error;//なかったらエラーとしてnum<0のものをだす
            }
            else
            {
                switch(option){
                    case "last":
                        return nb_list_name_fixed.Last();
                    case "next":
                        if (next_nb.num == -1)
                        { return nb_list_name_fixed[0]; }
                        else
                        { return next_nb; }
                    case "back":
                        if (back_nb.num == -1)
                        { return nb_list_name_fixed.Last(); }
                        else
                        { return back_nb; }
                    default:
                        return nb_list_name_fixed[0];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.MinimumSize = bigsize;
            this.MaximumSize = bigsize;
            this.Size = bigsize;

            this_nb = get_nb(textBox1.Text,0,"last");
            if (this_nb.num == -1)
            {//ない場合
                System.Diagnostics.Process.Start("https://www.google.co.jp/search?q=" + textBox1.Text);
            }
            else
            {//ある場合
                textBox2.Lines = this_nb.body;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.co.jp/search?q=" + textBox1.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.MinimumSize = smallsize;
            this.MaximumSize = smallsize;
            this.Size = smallsize;
        }

        private void button2_Click(object sender, EventArgs e)
        {//<
            this_nb = get_nb(this_nb.name,this_nb.num,"back");
            textBox1.Text = this_nb.name;
            textBox2.Lines = this_nb.body;
        }

        private void button3_Click(object sender, EventArgs e)
        {//>
            this_nb = get_nb(this_nb.name, this_nb.num + 1, "next");
            textBox1.Text = this_nb.name;
            textBox2.Lines = this_nb.body;
        }

        private void button5_Click(object sender, EventArgs e)
        {//登録
            StreamWriter writer = new StreamWriter("data.txt",true);

            writer.WriteLine("");
            writer.WriteLine("\'\'\'" + textBox1.Text);
            foreach(string l in textBox2.Lines)
            {
                writer.WriteLine(l);
            }
            writer.WriteLine("\'\'\'");
            writer.WriteLine("");
            
            writer.Close();
        }
    }
}
