using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityServiceManager
{
    public class CustomAuthorizationPolicy : IAuthorizationPolicy
    {
        public CustomAuthorizationPolicy()
        {
            Id = Guid.NewGuid().ToString();
        }
        public ClaimSet Issuer { get { return ClaimSet.System; } }
        public string Id { get; }
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            if (!evaluationContext.Properties.TryGetValue("Identities", out object list))
            {
                return false;
            }

            IList<IIdentity> identities = list as IList<IIdentity>;
            if (list == null || identities.Count <= 0)
            {
                return false;
            }

            // X509Identity is an internal class, so we cannot directly access it
            Type x509IdentityType = identities[0].GetType();

            // The certificate is stored inside a private field of this class
            FieldInfo certificateField = x509IdentityType.GetField("certificate", BindingFlags.Instance | BindingFlags.NonPublic);

            X509Certificate2 certificate = (X509Certificate2)certificateField.GetValue(identities[0]);

            string name = (certificate.SubjectName.Name).Split(';')[0];

            evaluationContext.Properties["Principal"] =
              new CustomPrincipal(identities[0]);
            return true;
        }
    }
}
