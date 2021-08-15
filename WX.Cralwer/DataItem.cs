namespace WX.Cralwer
{
    public class DataItem
    {

        /// <summary>
        /// 群名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 下单
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        public string GetTemplate_WX()
        {
            return this.Name + " " + this.Content + System.Environment.NewLine;
        }

        public string GetTemplate_OK()
        {
            return this.Name + " " + this.Status + System.Environment.NewLine;
        }


        public string GetTemplate_NO()
        {
            return this.Name + " " + this.Content + System.Environment.NewLine;
        }
    }
}
