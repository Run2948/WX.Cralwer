using System.Text.RegularExpressions;

namespace WX.Cralwer.Code
{

    /// <summary>
    /// 验证
    /// </summary>
    public abstract class Validation
    {

        public const string STATUS_NO = "疑似废单";

        protected private Regex regex_number = new Regex(@"^\d+$");

        protected Validation nextHandler;

        public Validation(Validation validation)
        {
            this.nextHandler = validation;
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string Process(string data)
        {
            if (this.nextHandler != null)
            {
                return this.nextHandler.Process(data);
            }
            else
            {
                return STATUS_NO;
            }
        }



        /// <summary>
        /// 处理
        /// </summary>
        /// <returns></returns> 
        protected abstract bool IsProcess(string data);

        
    }
}
