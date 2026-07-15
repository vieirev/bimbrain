using System.Threading.Tasks;

namespace BIMBrain.Queries
{
    public interface IQueryHandler
    {
        bool CanHandle(QueryContext context);

        Task<QueryResult> HandleAsync(QueryContext context);
    }
}
