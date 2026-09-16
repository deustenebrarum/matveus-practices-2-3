using System.Linq.Expressions;
using Marten.Linq;
using VaultCore.Modules.Catalog.Models;

namespace VaultCore.Modules.Catalog.Queries;

public class MiniaturesByFactionQuery : ICompiledListQuery<Miniature>
{
    public string Faction { get; set; } = string.Empty;

    public Expression<Func<IMartenQueryable<Miniature>, IEnumerable<Miniature>>> QueryIs()
    {
        return query => query.Where(x => x.Faction == Faction).OrderBy(x => x.Price);
    }
}
