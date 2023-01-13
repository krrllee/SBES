using Common;
using SecurityServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/CityGovernmentService";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            Console.WriteLine("Current user: " + WindowsIdentity.GetCurrent().Name);

            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string srvCertCN = "wcfcomplaintservice";
            X509Certificate2 srvCert = CertificateManager.GetCertificateFromStorage(StoreName.My,
                StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address),
                                      new X509CertificateEndpointIdentity(srvCert));


            ClientProxy proxy = new ClientProxy(binding, endpointAddress);
            proxy.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            proxy.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            proxy.Credentials.ClientCertificate.Certificate = CertificateManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            try
            {
                proxy.CreateNewChannel();


                bool menuType = proxy.IsSupervisor();

                if (menuType)
                {
                    InitializeSupervisorMenu(proxy);
                }
                InitializeUserMenu(proxy);


            }

            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
                Console.WriteLine("[InnerException] {0}", e.InnerException);
                Console.Read();
            }

            finally
            {
                Console.Read();
                proxy.Close();
            }


        }

        private static void InitializeSupervisorMenu(ClientProxy proxy)
        {
            Console.WriteLine("Welcome supervisor. To list all complaints with banned words send 'Complaints'. To exit, send 'Q'");
            string message = "";

            while (message != "Q")
            {
                message = Console.ReadLine();
                if (message == "Complaints")
                {
                    var complaints = proxy.GetComplaintsWithBannedWords();
                    string input = "";
                    foreach (var complaint in complaints)
                    {
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine($"Complaint Id: {complaint.Id}");
                        Console.WriteLine($"Complaint Username {complaint.Username}");
                        Console.WriteLine($"Complaint Message {complaint.Message}");
                        Console.WriteLine($"To ban the user, send BAN. To forgive the user, send FORGIVE");
                        while (!input.Equals("BAN") || !input.Equals("FORGIVE"))
                        {
                            input = Console.ReadLine();
                            if (input.Equals("BAN"))
                            {
                                proxy.BanComplainer(complaint.Username);
                                break;

                            }
                            if (input.Equals("FORGIVE"))
                            {
                                proxy.ForgiveComplainer(complaint.Username);
                                break;
                            }
                            Console.WriteLine("You didn't make a decision, write BAN or FORGIVE");
                        }

                    }
                }
                Console.WriteLine("To list all complaints with banned words send 'Complaints'. To exit, send 'Q'");
            }

        }

        private static void InitializeUserMenu(ClientProxy proxy)
        {
            string message = "";
            while (message != "Q")
            {
                Console.WriteLine("Please send your complaint. (Send 'Q' to exit)");

                message = Console.ReadLine();
                string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
                X509Certificate2 certificateSign = CertificateManager.GetCertificateFromStorage(StoreName.My,
                    StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(message, HashAlgorithm.SHA1, certificateSign);

                proxy.SendComplaint(message, signature);

            }

        }



    }
}
