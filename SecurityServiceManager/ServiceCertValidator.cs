using Common;
using SecurityServiceManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SecurityServiceManager
{
    public class ServiceCertValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            var bannedUsers = XMLManager.ReadBannedUsers();
            string[] name = certificate.Subject.Split(',');

            foreach (var user in bannedUsers)
            {
                string[] certName = user.Username.Split(',');
                if (name[0].Equals(certName[0]) || name[0].Equals(certificate.Issuer))
                {
                    throw new Exception("User is banned");
                }
            }

        }
    }
}
