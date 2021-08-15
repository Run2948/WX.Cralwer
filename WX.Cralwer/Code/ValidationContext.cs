using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace WX.Cralwer.Code
{

    /// <summary>
    /// 验证上下文
    /// </summary>
    public static class ValidationContext
    {

        private static Regex regex = new Regex(@"^\d+$",RegexOptions.Compiled);
        private static Validation validation = new Validation1(new Validation2(new Validation3(new Validation4(null))));


        /// <summary>
        /// 验证并处理数据转换
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string Process(string data)
        {
            
            data = FilterStr(data.TrimEnd('/'));
            StringBuilder newData = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                if ((data[i] == '/' && i != data.Length - 1) || regex.IsMatch(data[i].ToString()))
                    newData.Append(data[i].ToString());
                else
                    newData.Append(" ");
            }
            
            string replaceData = newData.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(replaceData))
            {
                string[] items = replaceData.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                StringBuilder result = new StringBuilder();
                foreach (var item in items)
                {
                    string _item = validation.Process(item.TrimEnd('/'));
                    if (_item == Validation.STATUS_NO)
                    {
                        return _item;
                    }
                    if (result.Length == 0)
                    {
                        result.Append(_item);
                    }
                    else
                    {
                        result.Append(" ").Append(_item);
                    }

                }
                return result.ToString();
            }
            else
            {
                return Validation.STATUS_NO;
            }
        }

        private static string FilterStr(string aa)
        {
            string ss = aa.TrimEnd('/');
            char[] list = aa.ToCharArray();

            int index = 0;
            int count = 0;
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int i = 0; i < list.Length; i++)
            {
                if (Regex.IsMatch(list[i].ToString(), @"^\d$"))
                {
                    if (count >= 3)
                    {
                        dic[index] = count;
                    }
                    index = 0;
                    count = 0;
                }
                else
                {
                    if (index == 0)
                        index = i;
                    count++;
                }
            }
            string ssss = "";
            int prevIndex = 0;
            foreach (var item in dic)
            {
                if (prevIndex == 0)
                    ssss = ss.Substring(prevIndex, item.Key);
                else
                {
                    int length = (item.Key - prevIndex);
                    ssss = ssss + " " + ss.Substring(prevIndex, length);
                }
                prevIndex = item.Key + item.Value;
            }
            ssss = ssss + " " + ss.Substring(prevIndex);
            return ssss;
        }


    }
}
