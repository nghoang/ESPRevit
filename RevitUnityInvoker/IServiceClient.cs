using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitUnityInvoker
{
    public interface IServiceClient
    {
        void SetServer(string serverip, string serverid);
        void FinishedJoinForm();
        string SyncModel();
    }
}
