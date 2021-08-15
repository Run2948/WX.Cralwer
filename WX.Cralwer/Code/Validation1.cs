using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WX.Cralwer.Code
{

    public class Validation1:Validation
    {

        private  Regex regex = new Regex(@"^\d{1,3}$");

        public Validation1(Validation validation) :base(validation)
        {

        }

        public override string Process(string data)
        {
            if (IsProcess(data))
            {
                string[] items = data.Split(new char[] { '/' });
                StringBuilder text = new StringBuilder();
                text.Append(items[0]).Append("/");
                if (items[1].EndsWith("00"))
                {
                    text.Append(items[1].Substring(0,items[1].Length - 2));
                }
                else
                {
                    text.Append(items[1]).Append("0");
                }
                return text.ToString();
            }
            else
            {
               return base.Process(data);
            }
        }

        protected override bool IsProcess(string data)
        {
            string[] items = data.Split(new char[] { '/' });
            if (items != null && items.Length == 2)
            {
                bool s_1 = regex.IsMatch(items[0]);
                bool s_2 = regex_number.IsMatch(items[1]);
                return s_1 && s_2;
            }
            else
                return false;
        }

    }
}
