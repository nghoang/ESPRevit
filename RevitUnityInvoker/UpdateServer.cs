using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitUnityInvoker
{
    class UpdateServer
    {
        public JoinForm main;
        public int status = 0;//0: stop 1:running
        public void run()
        {
            status = 1;
            while (true)
            {
                if (status == 0)
                    break;

                System.Threading.Thread.Sleep(500);
            }
            status = 0;
        }
    }
}
