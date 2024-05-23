using System;
using Google.Cloud.Firestore;

namespace CRA.DataMigration.DAL.Entities.Firestore
{
    [FirestoreData]
    public class Customer : FirestoreBaseEntity
    {
        [FirestoreProperty]
        public string FirstName { get; set; }

        [FirestoreProperty]
        public string LastName { get; set; }

        [FirestoreDocumentUpdateTimestamp]
        public DateTime Modified { get; set; }
    }
}