using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrawlerLib.Net;

namespace RevitUnityInvoker
{
    public partial class JoinForm : Form //what is partial mean here?? one class one file, if we want to use one class in different file, ues partial
    {
        WebclientX client = new WebclientX();
        List<string> sids;
        List<string> ips;
        public IServiceClient main= null;

        public JoinForm()// why no void here? it is a constructor and no return value. (void: no need to return value )for initialization?(right click to go to defination) what work sequence of this code?
        {
            InitializeComponent(); //create instance ,we will go here
        }

        private void JoinForm_Load(object sender, EventArgs e)// if it shows, we will get here
        {
            
            
        }

        private void SetServer()
        {
            AppConst.SERVER_DOMAIN = "http://" + textBoxServerIP.Text;
        }

        private void LoadHosts()
        {
            try
            {
                //request to PHP server to get available host. 
                string availableHostList = client.GetMethod(AppConst.SERVER_DOMAIN + AppConst.SERVER_PATH + "?act=ListAvailableHost");//77,111.111.111.11;77,111.111.111.11;...,
                sids = new List<string>();// create list of id
                ips = new List<string>();// create list of ip
                comboBoxServers.Items.Clear();// clear comboBoxServers
                if (availableHostList == "")
                {
                    MessageBox.Show("There is no host");
                }
                else
                {
                    string[] items = availableHostList.Split(';');
                    //77,111.111.111.11
                    //44,222.222.222.222
                    foreach (string item in items)//loop
                    {
                        string[] subitems = item.Split(',');
                        //77
                        //111.111.111.11
                        sids.Add(subitems[0]);
                        ips.Add(subitems[1]);
                        comboBoxServers.Items.Add(subitems[1]);//add ip to comboBoxServer
                    }
                    comboBoxServers.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)// if we click, we will get here
        {
            if (comboBoxServers.Items.Count == 0)
            {
                MessageBox.Show("There is no host");
                buttonSync.Enabled = false;
                buttonLogout.Enabled = false;
                return;
            }

            buttonSync.Enabled = true;
            buttonLogout.Enabled = true;

            //Here I callback to main process to assign selected host id and host ip. Please take a look at SetServer to see how it works.
            main.SetServer(ips[comboBoxServers.SelectedIndex], sids[comboBoxServers.SelectedIndex]);

            //Here I callback to main process to let it know that I finished the host selection. Take a look at FinishedJoinForm function in main process to see how it proceeds the next step
            main.FinishedJoinForm();
            
            
            //this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            buttonSync.Enabled = false;
            buttonLogout.Enabled = false;
            SetServer();
            LoadHosts();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            main.SyncModel();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            main.SetServer("", "");

            buttonSync.Enabled = false;
            buttonLogout.Enabled = false;
            this.Close();
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    main.SyncModel();
        //}
    }
}
