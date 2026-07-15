using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIMBrain.Queries
{
    public class QueryRouter
    {
        private readonly IEnumerable<IQueryHandler> _handlers;

        public QueryRouter(IEnumerable<IQueryHandler> handlers)
        {
            _handlers = handlers ?? Enumerable.Empty<IQueryHandler>();
        }

        public async Task<QueryResult> RouteAsync(QueryContext context)
        {
            foreach (var handler in _handlers)
            {
                if (handler.CanHandle(context))
                {
                    return await handler.HandleAsync(context);
                }
            }

            return new QueryResult { Handled = false };
        }
    }
}
