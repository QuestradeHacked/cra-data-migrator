using CRA.DataMigration.DAL.Clients.Firestore;
using CRA.DataMigration.DAL.Entities.Firestore;
using CRA.DataMigration.DAL.Repositories.Firestore.Abstract;

namespace CRA.DataMigration.DAL.Repositories.Firestore
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(
            FirestoreClientFactory<Message> firestoreClientFactory)
            : base(firestoreClientFactory, "Messages")
        {
        }
    }
}