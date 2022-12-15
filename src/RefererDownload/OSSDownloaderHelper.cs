using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RefererDownload
{
    public static class OSSDownloaderHelper
    {
        public string DownFile(string address, string fileName)
        {
            if (File.Exists(fileName))
            {
                var r = MessageBox.Show("文件已存在,是否替换?", "操作提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (r != MessageBoxResult.Yes)
                {
                    //Txb_tip.Text = "取消替换下载,清修改文件名";
                    return "取消替换下载,清修改文件名";
                }
            }
            ChangeStatus(false);
            using (WebClient c = new WebClient())
            {
                try
                {
                    c.Headers.Set("referer", Txt_ref.Text);
                    c.Headers.Set("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    c.Headers.Set("accept-language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");
                    c.Headers.Set("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/597.21 (KHTML, like Gecko) Chrome/89.0.3389.114 Safari/657.63");
                    c.DownloadProgressChanged += (sender, e) =>
                    {
                        Prb_down.Minimum = 0;
                        Prb_down.Maximum = e.TotalBytesToReceive;
                        Prb_down.Value = e.BytesReceived;
                        Txb_tip.Text = e.ProgressPercentage + "%";
                    };
                    c.DownloadFileCompleted += (sender, e) =>
                    {
                        if (e.Cancelled)
                        {
                            //Txb_tip.Text = "取消下载";
                        }

                        if (e.Error != null)
                        {
                            Exception ex = e.Error;
                            while (ex.InnerException != null) ex = ex.InnerException;
                            //Txb_tip.Text = ex.Message;
                        }
                        else
                        {
                            Txb_tip.Text = "下载完成";
                            Init();
                        }
                        ChangeStatus();
                    };
                    FunStop = () =>
                    {
                        c.CancelAsync();
                    };
                    Txb_tip.Text = "开始下载...";
                    Btn_Stop.Visibility = Visibility.Visible;
                    c.DownloadFileAsync(new Uri(address), fileName);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    Txb_tip.Text = ex.Message;
                    ChangeStatus();
                }
            }
        }
        void Init()
        {
            try
            {
                var dir = Path.Combine(Config.AppDataPath, "config.data");
                if (Txt_url.Text == "")
                {
                    var str = File.ReadAllText(dir);
                    var arr = str.Split(';');
                    Txt_url.Text = arr[0];
                    Txt_ref.Text = arr[1];
                    Txt_dir.Text = arr[2];
                }
                else
                {
                    string str = $"{Txt_url.Text};{Txt_ref.Text};{Txt_dir.Text}";
                    File.WriteAllText(dir, str);
                }
            }
            catch { }
        }
    }
}
