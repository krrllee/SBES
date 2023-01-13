using Common;
using SecurityServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class ClientProxy : ChannelFactory<ICityGovernment>, ICityGovernment, IDisposable
    {
        ICityGovernment factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
        }

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
        }

        public void CreateNewChannel()
        {
            factory = this.CreateChannel();

        }

        public void BanComplainer(string username)
        {
            try
            {
                factory.BanComplainer(username);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ForgiveComplainer(string username)
        {

            try
            {
                factory.ForgiveComplainer(username);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public List<Complaint> GetComplaintsWithBannedWords()
        {
            var retList = new List<Complaint>(1);

            try
            {
                retList = factory.GetComplaintsWithBannedWords();
                if (retList.Count == 0)
                {
                    Console.WriteLine("There are currently no complaints to be shown");
                }
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine(e.Message);
            }
            return retList;
        }

        public bool IsSupervisor()
        {
            bool ret = false;
            try
            {
                ret = factory.IsSupervisor();
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine(e.Message);
                ret = false;
            }

            return ret;
        }

        public void SendComplaint(string message, byte[] sign)
        {
            try
            {
                factory.SendComplaint(message, sign);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }
}
