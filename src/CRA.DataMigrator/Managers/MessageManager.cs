using System.Threading;
using System.Threading.Tasks;
using CRA.DataMigration.DAL.Entities.Firestore;
using CRA.DataMigration.DAL.Repositories.Firestore.Abstract;
using CRA.DataMigrator.Managers.Abstract;

namespace CRA.DataMigrator.Managers
{
    public class MessageManager : IMessageManager
    {
        private readonly IMessageRepository _messageRepository;

        public MessageManager(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<bool> IsProcessedAsync(string messageId, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.GetByIdAsync(messageId, cancellationToken);

            return message != null;
        }

        public async Task AddMessageAsync(string messageId, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                Id = messageId
            };

            await _messageRepository.UpsertAsync(message, cancellationToken);
        }
    }
}