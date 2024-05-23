using System;
using Google.Cloud.Firestore;

namespace CRA.DataMigration.DAL.Entities.Firestore
{
    public class FirestoreBaseEntity
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreDocumentCreateTimestamp]
        public DateTime Created { get; set; }
    }
}