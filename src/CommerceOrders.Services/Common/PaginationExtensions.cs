using System.Linq.Expressions;
using CommerceOrders.Contracts.UI;

namespace CommerceOrders.Services.Common;

public static class PaginationExtensions
{
    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> resultSet, int page)
    {
        return resultSet
            .Skip((page - 1) * AppSettings.ResponsePageLimit)
            .Take(AppSettings.ResponsePageLimit);
    }

    public static PaginationResultQueryResponse<TSource> ToPaginationResult<TSource>(
        this IEnumerable<TSource> items, int totalItems,
        int page)
    {
        return new PaginationResultQueryResponse<TSource>()
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            TotalPages = totalItems % AppSettings.ResponsePageLimit == 0
                ? totalItems / AppSettings.ResponsePageLimit
                : totalItems / AppSettings.ResponsePageLimit + 1
        };
    }
}