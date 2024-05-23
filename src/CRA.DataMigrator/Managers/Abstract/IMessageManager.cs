using System.Threading;
using System.Threading.Tasks;

namespace CRA.DataMigrator.Managers.Abstract
{
    public interface IMessageManager
    {
        Task<bool> IsProcessedAsync(string messageId, CancellationToken cancellationToken);
        Task AddMessageAsync(string messageId, CancellationToken cancellationToken);
    }
}