using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Complaint
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Message { get; set; }

        public Complaint()
        {

        }

    }
}
