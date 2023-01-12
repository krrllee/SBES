using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecurityServiceManager
{
    public class CertificateManager
    {
        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates;


            foreach (X509Certificate2 c in certCollection)
            {
                string[] name = c.SubjectName.Name.Split(',');
                if (name[0].Equals(string.Format("CN={0}", subjectName)))
                {
                    return c;
                }
            }

            return null;
        }


    }
}
