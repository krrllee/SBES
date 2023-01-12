using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    [ServiceContract]
    public interface ICityGovernment
    {
        [OperationContract]
        void SendComplaint(string message, byte[] sign);

        [OperationContract]
        void BanComplainer(string username);

        [OperationContract]
        void ForgiveComplainer(string username);

        [OperationContract]
        List<Complaint> GetComplaintsWithBannedWords();

        [OperationContract]
        bool IsSupervisor();



    }
}

