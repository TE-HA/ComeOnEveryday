using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComeOnEveryDay
{
    public partial class Form1 : Form
    {
        private Icon blank = new Icon("1.ico");//新建一个ico图片的对象以供下面实现闪烁的效果
        private Icon normal = new Icon("2.ico");
        private Point lastPoint;
        private Point lastPoint2;

        public Form1()
        {
            InitializeComponent();
        }

        public string getHtml(string html)//传入网址
        {
            string pageHtml = "";
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(html); //从指定网站下载数据
            MemoryStream ms = new MemoryStream(pageData);
            using (StreamReader sr = new StreamReader(ms, Encoding.GetEncoding("GB2312")))
            {
                pageHtml = sr.ReadLine();
            }
            return pageHtml;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           /* try
            {*/
                string data = getHtml("http://open.iciba.com/dsapi/");
                JsonParser two = JsonConvert.DeserializeObject<JsonParser>(data);
                //？？？
                string result = string.Format("{0}", two.content);//显示结果
                string result_trans = string.Format("{0}", two.note);//显示结果
               try
                {
                    notifyIcon1.Text = result_trans;
                }
                catch
                {
                    notifyIcon1.Text = CutByteString(result_trans, 64);
                }
                contextMenuStrip1.Items.Add(result);
                notifyIcon1.ShowBalloonTip(100000, "translate", result_trans, ToolTipIcon.Info);
                notifyIcon1.ShowBalloonTip(100000, "sentence", result, ToolTipIcon.Info);
           /* }
            catch
            {
                System.Threading.Thread.Sleep(10000);
                Application.Restart();
            }*/
        }

        public static string CutByteString(string str, int len)
        {
            string result = string.Empty;// 最终返回的结果
            if (string.IsNullOrEmpty(str)) { return result; }
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);// 单字节字符长度
            int charLen = str.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                    { byteCount += 2; }
                    else// 按英文字符计算加1
                    { byteCount += 1; }
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }
                if (pos >= 0)
                { result = str.Substring(0, pos); }
            }
            else
            { result = str; }
            return result;
        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            notifyIcon1.Icon = normal;
            this.lastPoint = System.Windows.Forms.Cursor.Position;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.lastPoint2 = System.Windows.Forms.Cursor.Position;
            if (lastPoint != lastPoint2)
            {
                notifyIcon1.Icon = blank;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}

