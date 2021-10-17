using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.IO.Ports;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Net.Sockets;

namespace SystemSMS
{
    public partial class Form1 : Form
    {
        private SerialPort _serialPort;
        DataTable dt = new DataTable();
        int b = 80;
        Button[] buttons = new Button[100];
        string[] arr=new string[500];
        string pcid;    
        string strr;
        string msgvar;
        string ip;
        string chname;
        string no;
        string msg;
        string jobid;

        public Form1()
        {
            InitializeComponent();
        }

        //
        private void systemdate()
        {
            DateTime dt = DateTime.Now;
            strr = Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm:ss");
            //datetxt.Text = strr;
        }
        //
        public string LocalIPAddress()
        {
           
            //IPHostEntry host;
            string localIP = "";
             
            // host = Dns.GetHostByAddress("172.168.201.59");
            // string hn=host.ToString();
             //string hostname = ip.HostName;
            //host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (IPAddress ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        localIP = ip.ToString();
            //        break;
            //    }
            //}
            return localIP;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           //LocalIPAddress();
            int a = 0;
            systemdate();

            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
                                
            
            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;

            string qry = @"SELECT
systemsms_pc.pcID,
systemsms_pc.pcIP,
systemsms_pc.pcHostName,
systemsms_pc.isActive
FROM
systemsms_pc ";

            DataTable dt=DBHandler.GetData(qry);
            foreach (DataRow dr in dt.Rows)
            {
                
                PingReply reply = pingSender.Send(dr[1].ToString(), timeout, buffer, options);


                string qry1 = @"insert into systemsms_pinginfo(
systemsms_pinginfo.pingIP,
systemsms_pinginfo.hostName,
systemsms_pinginfo.pingTime,
systemsms_pinginfo.pingResult)  VALUES('" + dr[1].ToString() + "','" + dr[2].ToString() + "','" + strr + "','"+reply.Status+"')";
               //dt = DBHandler.InsertData(qry1);
                pcid = dr[0].ToString();                    
                msgvar = "Host Name:" + dr[2].ToString() + "\r" + "IP:" + dr[1].ToString() + "\r" + "System Shutdown !";


              
                 //List<Button> buttons = new List<Button>();
                //for (int i = 0; i < 10; ++i)
                //{
                //    var button = new Button();
                //    button.Location = new Point(button.Width * i + 4, 0);
                //    Controls.Add(button);
                //}

                 

                 if (a < 8)
                 {
                    

                     buttons[a] = new Button();
                     buttons[a].Size = new Size(150, 50);
                     buttons[a].Text = dr[1].ToString();
                     buttons[a].Location = new Point(buttons[a].Width * a + 15, b);
                     if (reply.Status.ToString() == "Success")
                     {
                         buttons[a].BackColor = Color.Green;

                     }
                     else { buttons[a].BackColor = Color.Red; }
                     this.Controls.Add(buttons[a]);
                     a++;
                 }
                 else
                 {


                     b = b + 80;
                     a = 0;
                     buttons[a] = new Button();
                     buttons[a].Size = new Size(150, 50);
                     buttons[a].Text = dr[1].ToString();
                    
                     buttons[a].Location = new Point(buttons[a].Width * a + 15, b);
                     if (reply.Status.ToString() == "Success")
                     {
                         buttons[a].BackColor = Color.Green;

                     }
                     else { buttons[a].BackColor = Color.Red; }
                     this.Controls.Add(buttons[a]);
                     a++;
                 }
                 
               
                // a++;
                if (reply.Status != IPStatus.Success)
                {
                    //abc();
                    //Console.WriteLine("Address: {0}", reply.Address.ToString());
                    //Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                    //Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                    //Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                    //Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                }
            }

            //ComputerPanel cp = new ComputerPanel();
            //cp.Show();
        }

        //
        private void abc()
        {
            //string number = textBoxNumber.Text;
            //string message = textBoxMessage.Text;

            //FOR SELECT PHONE NUMBERS 


            string qry = @"SELECT
systemsms_person.personNumber
FROM
systemsms_person
INNER JOIN systemsms_subscrip ON systemsms_person.personID = systemsms_subscrip.personID
where systemsms_subscrip.pcID='1' and systemsms_person.isActive='"+pcid+"'";

           DataTable dt = DBHandler.GetData(qry);
            

            string[] arr = new string[dt.Rows.Count];
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                arr[a] = dt.Rows[a][0].ToString();
            }

            //foreach (DataRow dr in dt1.Rows)
            //{
            //    string[] arr = new string[dr.ItemArray.Length];
            //    for (int x = 0; x < dr.ItemArray.Length; x++)
            //    {
            //        arr[x] = dr[2].ToString();
            //    }
            //}


            


            //FOR INSERT IN SMS_JOBS TABLE FOR SENDING MSG

            dt.Reset();
            for (int a = 0; a < arr.Length; a++)
            {
                string qry1 = @"INSERT INTO systemsms_jobs(systemsms_jobs.userid,systemsms_jobs.insertDate,systemsms_jobs.deliverScheduleTime,systemsms_jobs.deliverTime,systemsms_jobs.delivered, systemsms_jobs.telNo, systemsms_jobs.message, systemsms_jobs.insertid) 
VALUES ('1','" + strr + "','" + strr + "','" + strr + "','0','" + arr[a] + "','" + msgvar + "','1') ";

                dt = DBHandler.GetData(qry1);

            }





            dt.Reset();
            string qry2 = "select jobsID,userid,delivered,telNo,message from systemsms_jobs where systemsms_jobs.delivered='0'";
            dt = DBHandler.GetData(qry2);

            foreach (DataRow dr in dt.Rows)
            {
                jobid = dr[0].ToString();
                no = dr[3].ToString();
                msg = dr[4].ToString();
                sent_sms();
            }
        }

        //
        private void sent_sms()
        {
            //systemdate();
            //string number = "+923452793400";
            string number = no;
            string message = msg;
            //string message ="This " +"\n"+ "is" + "\n"+ "SMS";
            //Replace "COM7"withcorresponding port name
            _serialPort = new SerialPort("COM3", 115200);

            Thread.Sleep(1000);

            _serialPort.Open();

            Thread.Sleep(1000);

            _serialPort.Write("AT+CMGF=1\r");

            //DisplayReceivedData();

            Thread.Sleep(1000);

            _serialPort.Write("AT+CMGS=\"" + number + "\"\r\n");

            Thread.Sleep(1000);

            _serialPort.Write(message + "\x1A");

            Thread.Sleep(1000);

            //MessageBox.Show("Sent !!!");
            //labelStatus.Text = "Status: Message sent";

            _serialPort.Close();

            string qry1 = "update systemsms_jobs set systemsms_jobs.deliverTime='" + strr + "', systemsms_jobs.delivered='1' where systemsms_jobs.jobsID='" + jobid + "'";
            DBHandler.InsertData(qry1);

            Thread.Sleep(60000);
            //MessageBox.Show("Sent");
            //return;
        }


        //Show logs 
        private void button1_Click(object sender, EventArgs e)
        {
            string date = Convert.ToDateTime(dateTimePicker1.Value).ToString("yyyy-MM-dd");

            string qry = @"SELECT
systemsms_pinginfo.pingIP,
systemsms_pinginfo.hostName,
systemsms_pinginfo.pingTime,
systemsms_pinginfo.pingResult
FROM
systemsms_pinginfo
where pingTime BETWEEN '"+date+" 00:00:00' and '"+date+" 23:59:59'";
            dt = DBHandler.GetData(qry);
            LogsResults lr = new LogsResults(dt);
            lr.Show();
            

        }


    }
}
