using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WX.Cralwer.Code
{


    public class Validation2:Validation
    {

        public Validation2(Validation validation):base(validation)
        {

        }

        private Regex regex_1 = new Regex(@"^\d{1}$");
        private Regex regex_2 = new Regex(@"^\d{1,2}$");


        protected override bool IsProcess(string data)
        {
            string[] items = data.Split(new char[] { '/' });

            if (items != null && items.Length == 3)
            {
                bool s_1 = regex_1.IsMatch(items[0]);
                bool s_2 = regex_2.IsMatch(items[1]);
                bool s_3 = regex_number.IsMatch(items[2]);
                return s_1 && s_2 && s_3;
            }
            else
                return false;
        }

        public override string Process(string data)
        {
            if (IsProcess(data))
            {
                string[] items = data.Split(new char[] { '/' });
                StringBuilder text = new StringBuilder();
                text.Append(items[0]).Append(".").Append(items[1]).Append("/");
                if (items[2].EndsWith("00"))
                {
                    text.Append(items[2].Substring(0,items[2].Length - 2));
                }
                else
                {
                    text.Append(items[2]).Append("0");
                }
                return text.ToString();
            }
            else
            {
                return base.Process(data);
            }
        }
    }
}
