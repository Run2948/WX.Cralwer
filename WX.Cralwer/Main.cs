using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WX.Cralwer.Code;
using System.Linq;
using System.Text.RegularExpressions;

namespace WX.Cralwer
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private Dictionary<string, List<DataItem>> Data = new Dictionary<string, List<DataItem>>();
        private Dictionary<string, List<DataItem>> SavedData = new Dictionary<string, List<DataItem>>();
        private bool status = false;
        private DateTime startTime = DateTime.MinValue;

        private void Main_Load(object sender, EventArgs e)
        {
            timer1.Start();
            chromiumWebBrowser1.Load("https://wx.qq.com");
            Task.Factory.StartNew(OnProcess,TaskCreationOptions.LongRunning);
        }



        private string GetSaveFilePath()
        {
            string path = Path.Combine(Application.StartupPath, "order", DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private void OnProcess()
        {
            while (true)
            {
                if (this.status)
                {
                    try
                    {
                        this.Invoke(new MethodInvoker(() =>
                        {
                           
                            Task<string> source = chromiumWebBrowser1.GetBrowser().MainFrame.GetSourceAsync();
                            string text = source.Result;
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                try
                                {
                                    bool status = false;
                                    string html = text.Replace("<!DOCTYPE html>", "");
                                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                    doc.LoadHtml(html);
                                  
                                    HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[@id=\"chatArea\"]");
                                    HtmlNode node_group = node.SelectSingleNode("//a[@class=\"title_name ng-binding\"]");
                                    HtmlNodeCollection nodes = node.SelectNodes("//*[@ng-repeat=\"message in chatContent\"]");
                                    HtmlNode node_title = node.SelectSingleNode("./div/div/div/a[@class=\"title_name ng-binding\"]");
                                    HtmlNode node_title1 = node.SelectSingleNode("./div/div/div/div/div/a[@class=\"title_name ng-binding\"]");
                                    HtmlNode node_title2 = node.SelectSingleNode("./div/div/a[@class=\"title_name ng-binding\"]");
                                   
                                    if (nodes == null)
                                        return;
                                    for (int i = 0; i < nodes.Count; i++)
                                    {
                                        HtmlNode node_img = nodes[i].SelectSingleNode("./div/div/div/img[@class=\"avatar\"]");

                                        //客户消息ID
                                        HtmlNode node_identity_left = nodes[i].SelectSingleNode("./div/div/div/div/div[@class=\"bubble js_message_bubble ng-scope bubble_default left\"]");
                                        //自己消息ID
                                        HtmlNode node_identity = nodes[i].SelectSingleNode("./div/div/div/div/div[@class=\"bubble js_message_bubble ng-scope bubble_primary right\"]");
                                        HtmlNode node_content = nodes[i].SelectSingleNode("./div/div/div/div/div/div/div/pre[@class=\"js_message_plain ng-binding\"]");
                                        if (node_content == null)
                                            continue;
                                        if (node_img != null && node_content != null && !string.IsNullOrWhiteSpace(node_content.InnerText))
                                        {
                                            DataItem dataItem = new DataItem();
                                            dataItem.GroupName = node_group != null ? node_group.InnerText : string.Empty;
                                            dataItem.Name = node_img.Attributes["title"].Value.Replace("<img","").Replace(">","");
                              
                                            dataItem.Content = node_content.InnerText;
                                            if (node_identity != null)
                                                dataItem.ID = node_identity.Attributes["data-cm"].Value;
                                            if (node_identity_left != null)
                                                dataItem.ID = node_identity_left.Attributes["data-cm"].Value;
                                            dataItem.Status = ValidationContext.Process(dataItem.Content);
                                            if (SavedData.ContainsKey(dataItem.GroupName) && SavedData[dataItem.GroupName].Exists(d=>d.ID == dataItem.ID))
                                            {
                                                continue;
                                            }
                                            if (!Data.ContainsKey(dataItem.GroupName))
                                            {
                                                Data[dataItem.GroupName] = new List<DataItem>() { dataItem };
                                                if (!status)
                                                {
                                                    status = true;
                                                }
                                            }
                                            else
                                            {
                                                if (!Data[dataItem.GroupName].Exists(d => d.ID == dataItem.ID))
                                                {
                                                    Data[dataItem.GroupName].Add(dataItem);
                                                    if (!status)
                                                    {
                                                        status = true;
                                                    }
                                                }
                                            }
                                          
                                        }
                                    }
                                    if (status)//判断有新数据更新列表
                                    {
                                        listView1.Items.Clear();
                                        foreach (var item in Data)
                                        {
                                            foreach (var child in item.Value)
                                            {
                                                ListViewItem lvi = new ListViewItem();
                                                lvi.Tag = child;
                                                lvi.Text = child.GroupName;
                                                lvi.SubItems.AddRange(new string[] { child.Name, child.Content, child.Status });
                                                if (child.Status == Validation.STATUS_NO)
                                                    lvi.BackColor = Color.LightGray;
                                                listView1.Items.Add(lvi);
                                            }
                                            
                                        }
                                        listView1.EnsureVisible(listView1.Items.Count - 1);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }));
                    }
                    catch
                    { 
                    
                    }
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                }
               
            }
        }


        //开始
        private void tsml_Start_Click(object sender, EventArgs e)
        {
            if (!status)
            {
                tsml_Start.Text = "暂停";
                status = true;
                this.startTime = DateTime.Now;
            }
            else
            {
                tsml_Start.Text = "开始";
                status = false;
            }
        }

        private void 测试ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //判断已暂停
            if (!this.status)
            {
                //判断并且数据原有数据，并且有未保存的数据
                if (ValidSaveData())
                {
                    Task.Factory.StartNew(()=> 
                    {
                       
                        Dictionary<string, List<DataItem>> items = GetNoSavedData();
                        foreach (var item in items)
                        {
                            string file_wx = GetFullFileName(item.Key, STATUE_NAME_OLD);//微信原始文件
                            string file_ocr = GetFullFileName(item.Key, STATUE_NAME_OK); //识别文件
                            string file_fail = GetFullFileName(item.Key, STATUE_NAME_NO); //无效文件
                            WriterFile_WX(item.Value, file_wx);
                            WriterFile_NO(item.Value.FindAll(d => d.Status == Validation.STATUS_NO), file_fail);
                            WriterFile_OK(item.Value.FindAll(d => d.Status != Validation.STATUS_NO), file_ocr);
                        }
                       
                        UpdateSavedData(items);
                       
                        this.Invoke(new MethodInvoker(() =>
                        {
                            listView1.Items.Clear();
                            Data.Clear();
                            MessageBox.Show("保存完成");
                        }));
                    });
                }
                else
                {
                    MessageBox.Show("暂时没有数据需要保存");
                }
            }
            else
            {
                MessageBox.Show("请先暂停在保存");
            }
        }

        /// <summary>
        /// 验证并且数据原有数据，并且有未保存的数据
        /// </summary>
        /// <returns></returns>
        private bool ValidSaveData()
        {
            int count = 0;
            foreach (var item in SavedData)
            {
                count += item.Value.Count;
            }
            return Data.Count > count;
        }

        /// <summary>
        /// 获取未保存数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string,List<DataItem>> GetNoSavedData()
        {
            Dictionary<string, List<DataItem>> data = new Dictionary<string, List<DataItem>>();
            foreach (var item in Data)
            {
                if (!SavedData.ContainsKey(item.Key))
                {
                    data[item.Key] = item.Value;
                }
                else
                {
                    foreach (var child in item.Value)
                    {
                        if (!SavedData[item.Key].Exists(d => d.ID == child.ID))
                        {
                            data[item.Key].Add(child);
                        }
                    }
                   
                }
                
            }
            return data;
        }

        public int GetNoSaveDataCount()
        {
            int count = 0;
            foreach (var item in Data)
            {
                if (!SavedData.ContainsKey(item.Key))
                {
                    count += item.Value.Count;
                }
                else
                {
                    foreach (var child in item.Value)
                    {
                        if (!SavedData[item.Key].Exists(d => d.ID == child.ID))
                        {
                            count += 1;
                        }
                    }
                }
            }
            return count;
        }

        private void UpdateSavedData(Dictionary<string, List<DataItem>> items)
        {
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    if (SavedData.ContainsKey(item.Key))
                    {
                        foreach (var child in item.Value)
                        {
                            if (!SavedData[item.Key].Exists(d => d.ID == child.ID))
                                SavedData[item.Key].Add(child);
                        }
                      
                    }
                    else
                    {
                        SavedData[item.Key] = item.Value;
                    }
                }
              
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tssl_group_count.Text = "群组：[" + Data.Keys.Count + "]";
            int ok = 0, no = 0;
            List<string> keys = Data.Keys.ToList();
            foreach (var item in keys)
            {
                ok += Data[item].FindAll(d => d.Status != Validation.STATUS_NO).Count;
                no += Data[item].FindAll(d => d.Status == Validation.STATUS_NO).Count;
            }
            tssl_ok_count.Text = "有效订单数：[" + ok + "]";
            tssl_no_count.Text = "疑似废单数：[" + no + "]";
            tssl_no_save_count.Text = "有：[" + GetNoSaveDataCount() + "]条数据未保存";
            tssl_path.Text = "路径[" + GetSaveFilePath() + "]";
        }

        private string GetFullFileName(string groupName, string statusName)
        {
            DateTime current = DateTime.Now;
            string fileFormat = groupName + "_" + current.ToString("yyyyMMdd") + "_" + this.startTime.ToString("HHmmss") + "_" + current.ToString("HHmmss") + "_" + statusName + ".txt";
            return Path.Combine(GetSaveFilePath(), fileFormat);
        }

        /// <summary>
        /// 写入原始文件
        /// </summary>
        /// <param name="items"></param>
        /// <param name="file"></param>
        private void WriterFile_WX(List<DataItem> items, string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                foreach (var item in items)
                {
                    byte[] data = Encoding.UTF8.GetBytes(item.GetTemplate_WX());
                    fs.Write(data, 0, data.Length);
                }
                fs.Flush();
            }
        }


        /// <summary>
        /// 写入原始文件
        /// </summary>
        /// <param name="items"></param>
        /// <param name="file"></param>
        private void WriterFile_OK(List<DataItem> items, string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                foreach (var item in items)
                {
                    byte[] data = Encoding.UTF8.GetBytes(item.GetTemplate_OK());
                    fs.Write(data, 0, data.Length);
                }
                fs.Flush();
            }
        }

        /// <summary>
        /// 写入原始文件
        /// </summary>
        /// <param name="items"></param>
        /// <param name="file"></param>
        private void WriterFile_NO(List<DataItem> items, string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                foreach (var item in items)
                {
                    byte[] data = Encoding.UTF8.GetBytes(item.GetTemplate_NO());
                    fs.Write(data, 0, data.Length);
                }
                fs.Flush();
            }
        }


        private const string STATUE_NAME_OLD = "原始";
        private const string STATUE_NAME_OK = "有效";
        private const string STATUE_NAME_NO = "废弃";

        private void 修改废单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            DataItem data = listView1.SelectedItems[0].Tag as DataItem;
            Data[data.GroupName].Find(d => d.ID == data.ID).Status = Validation.STATUS_NO;
            listView1.SelectedItems[0].SubItems[3].Text = Validation.STATUS_NO;
            listView1.Refresh();
            MessageBox.Show("修改完成");
        }

        private void 修改订单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            DataItem data = listView1.SelectedItems[0].Tag as DataItem;
            UpdateOrder update = new UpdateOrder();
            update.Tag = data;
            update.Order = data.Content;
            update.Status = data.Status;
            update.SaveCompleted += Update_SaveCompleted;
            update.Show();
        }

        private void Update_SaveCompleted(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            listView1.SelectedItems[0].SubItems[2].Text = (sender as UpdateOrder).Order;
            listView1.SelectedItems[0].SubItems[3].Text = (sender as UpdateOrder).Status;
            DataItem data = (sender as UpdateOrder).Tag as DataItem;
            this.Data[data.GroupName].Find(d=>d.ID == data.ID).Content = (sender as UpdateOrder).Order;
            this.Data[data.GroupName].Find(d => d.ID == data.ID).Status = (sender as UpdateOrder).Status;
            listView1.Refresh();
            MessageBox.Show("修改成功");
 
        }

    }
}
