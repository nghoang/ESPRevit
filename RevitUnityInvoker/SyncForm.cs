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
    public partial class SyncForm : Form
    {
        WebclientX client = new WebclientX();
        public IServiceClient main;
        private string selectedServer = "";
        private string selectedSid = "";

        public SyncForm()
        {
            InitializeComponent();
        }

        public void SetServerForSync(string _selectedServer, string _selectedSid)
        {
            selectedServer = _selectedServer;
            selectedSid = _selectedSid;
        }

        private void button1_Click(object sender, EventArgs e)// logout
        {
            main.SetServer("", "");// clear ip , id
            this.Close();//close the current instance 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            main.SyncModel(); // call the ServiceClient.SyncModel
        }

        private void ClientOptions_Load(object sender, EventArgs e)
        {
            labelServer.Text = "Server IP: "+selectedServer+" ("+selectedSid+")";
        }

    }
}
