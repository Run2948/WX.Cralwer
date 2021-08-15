using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WX.Cralwer
{
    public partial class UpdateOrder : Form
    {
        public UpdateOrder()
        {
            InitializeComponent();
        }

        public string Order { get; set; }

        public string Status { get; set; }

        public bool Save = false;

        private void UpdateOrder_Load(object sender, EventArgs e)
        {
            textBox1.Text = this.Order;
            textBox2.Text = this.Status;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Order = textBox1.Text.Trim();
            this.Status = textBox2.Text.Trim();
            if (string.IsNullOrWhiteSpace(Order))
            {
                MessageBox.Show("请输入订单数据");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.Status))
            {
                MessageBox.Show("请输入识别数据");
                return;
            }
            if (this.SaveCompleted != null)
            {
                this.SaveCompleted(this, new EventArgs());
            }
        }

        public event EventHandler SaveCompleted;

        public void Clear()
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
