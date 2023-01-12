using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SecurityServiceManager
{
    public class XMLManager
    {
        private static string bannedWordsPath = "../../../Server/banned_words.xml";
        private static string bannedCertsPath = "../../../Server/banned_certs.xml";
        private static string complaintsPath = "../../../Server/complaints.xml";

        public static void WriteBannedUsers(string message)
        {

            BannedUser bannedUser = new BannedUser() { Username = message };

            XmlSerializer serializer = new XmlSerializer(typeof(List<BannedUser>));

            var bannedUsers = ReadBannedUsers();

            if (bannedUsers.SingleOrDefault(x => x.Username == message) != null)
            {
                throw new FaultException("User already banned");
            }

            bannedUsers.Add(bannedUser);

            using (var writer = new StreamWriter(bannedCertsPath))
            {
                serializer.Serialize(writer, bannedUsers);
            }
        }

        public static List<BannedUser> ReadBannedUsers()
        {
            //AddDummyData();
            List<BannedUser> bannedUsers = new List<BannedUser>(1);
            XmlSerializer serializer = new XmlSerializer(typeof(List<BannedUser>));
            using (var reader = new StreamReader(bannedCertsPath))
            {
                bannedUsers = (List<BannedUser>)serializer.Deserialize(reader);
            }

            return bannedUsers;
        }

        public static List<BannedWord> ReadBannedWords()
        {

            List<BannedWord> bannedWords = new List<BannedWord>(1);

            XmlSerializer serializer = new XmlSerializer(typeof(List<BannedWord>));

            using (var reader = new StreamReader(bannedWordsPath))
            {
                bannedWords = (List<BannedWord>)serializer.Deserialize(reader);
            }

            return bannedWords;

        }


        public static List<Complaint> ReadComplaints()
        {

            List<Complaint> complaints = new List<Complaint>(1);

            XmlSerializer serializer = new XmlSerializer(typeof(List<Complaint>));

            using (var reader = new StreamReader(complaintsPath))
            {
                complaints = (List<Complaint>)serializer.Deserialize(reader);
            }

            return complaints;

        }

        public static void WriteComplaint(Complaint newComplaint)
        {

            List<Complaint> complaints = ReadComplaints();
            complaints.Add(newComplaint);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Complaint>));
            using (var writer = new StreamWriter(complaintsPath))
            {
                serializer.Serialize(writer, complaints);
            }
        }

        private static void AddDummyData()
        {
            List<BannedWord> bannedWords = new List<BannedWord>() { new BannedWord { Word = "asd" }, new BannedWord { Word = "lol" }, new BannedWord { Word = "haha" } };
            XmlSerializer serializer = new XmlSerializer(typeof(List<BannedWord>));
            using (var writer = new StreamWriter(bannedWordsPath))
            {
                serializer.Serialize(writer, bannedWords);
            }


            List<BannedUser> users = new List<BannedUser>() { new BannedUser { Username = "blabla" } };
            serializer = new XmlSerializer(typeof(List<BannedUser>));

            using (var writer = new StreamWriter(bannedCertsPath))
            {
                serializer.Serialize(writer, users);
            }


        }





    }
}
