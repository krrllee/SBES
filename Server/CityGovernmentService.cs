using Common;
using SecurityServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class CityGovernmentService : ICityGovernment
    {
        [PrincipalPermission(SecurityAction.Demand, Role = "Nadzor")]
        public void BanComplainer(string username)
        {

            try
            {
                XMLManager.WriteBannedUsers(username);

                try
                {
                    Audit.UserBanned(username);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Nadzor")]
        public void ForgiveComplainer(string username)
        {
            try
            {
                Audit.UserForgiven(username);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Nadzor")]
        public List<Complaint> GetComplaintsWithBannedWords()
        {
            var retList = new List<Complaint>();
            var complaints = XMLManager.ReadComplaints();
            var bannedWords = XMLManager.ReadBannedWords();
            var words = bannedWords.Select(x => x.Word).ToList();

            if (complaints.Count() > 0)
            {
                foreach (var complain in complaints)
                {
                    if (words.Count() > 0)
                    {
                        foreach (var word in words)
                        {
                            if (complain.Message.Contains(word))
                            {
                                retList.Add(complain);
                            }
                        }
                    }
                }
            }


            return FilterComplaintsWithUnbannedUsers(retList);

        }

        public bool IsSupervisor()
        {
            CustomPrincipal principal = new CustomPrincipal(ServiceSecurityContext.Current.PrimaryIdentity);

            return principal.IsInRole("Nadzor");
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Nadzor")]
        private List<Complaint> FilterComplaintsWithUnbannedUsers(List<Complaint> complaints)
        {
            var bannedUsers = XMLManager.ReadBannedUsers();

            foreach (var user in bannedUsers)
            {
                if (complaints.Where(x => user.Username.Contains(x.Username)) != null)
                {
                    complaints.RemoveAll(x => user.Username.Contains(x.Username));
                }
            }

            return complaints;

        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Korisnik")]
        public void SendComplaint(string message, byte[] sign)
        {
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            string[] name = clientName.Split(',');

            string[] username = name[0].Split('=');
            string clientNameSign = username[1] + "_sign";
            X509Certificate2 certificate = CertificateManager.GetCertificateFromStorage(StoreName.My,
                StoreLocation.LocalMachine, clientNameSign);

            if (!DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                throw new FaultException("Sign is invalid");
            }

            Complaint newComplaint = new Complaint() { Id = Guid.NewGuid().ToString(), Message = message, Username = clientName };

            try
            {
                XMLManager.WriteComplaint(newComplaint);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Message);
            }
        }


    }
}
