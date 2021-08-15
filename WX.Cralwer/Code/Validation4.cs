using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WX.Cralwer.Code
{
    public class Validation4 : Validation
    {
        private Regex regex = new Regex(@"^\d{1}$");

        public Validation4(Validation validation) : base(validation)
        {

        }

        public override string Process(string data)
        {
            if (IsProcess(data))
            {
                string[] items = data.Split('/');
                StringBuilder text = new StringBuilder();
                text.Append(items[0]).Append(">").Append(items[2]);
               
                if (items[3].EndsWith("00"))
                {
                    text.Append("/").Append(items[3].Substring(0, items[3].Length - 2));
                }
                else
                {
                    text.Append("/").Append(items[3]).Append("0");
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
            string[] items = data.Split('/');

            if (items != null && items.Length == 4)
                return regex.IsMatch(items[0]) && string.IsNullOrWhiteSpace(items[1]) && regex_number.IsMatch(items[2]) && regex_number.IsMatch(items[3]) ? true : false;
            else
                return false;
        }
    }
}
