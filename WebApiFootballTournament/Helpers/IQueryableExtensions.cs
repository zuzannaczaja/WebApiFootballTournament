using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace WebApiFootballTournament.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByString = string.Empty;

            var orderByAfterSplit = orderBy.Split(',');

            foreach (var orderByClause in orderByAfterSplit)
            {
                var trimmedOrderByClause = orderByClause.Trim();

                bool revert = false;

                if (trimmedOrderByClause.EndsWith(" desc"))
                {
                    revert = true;
                }

                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var destinationProperties = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                var destinationPropertiesSplit = destinationProperties.Trim().Split(',');

                foreach (var destinationProperty in destinationPropertiesSplit)
                {

                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + destinationProperty
                        + (revert ? " desc" : "");
                }
            }

            return source.OrderBy(orderByString);
        }
    }
}
