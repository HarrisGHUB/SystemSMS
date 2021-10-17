using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemSMS
{
    public partial class LogsResults : Form
    {
        
        public LogsResults(DataTable dt)
        {
            InitializeComponent();
            dataGridView1.DataSource = dt;
        }


        private void ComputerPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
