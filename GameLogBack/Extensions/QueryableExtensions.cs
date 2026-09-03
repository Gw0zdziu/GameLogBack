using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedResults<T>> GetPaginatedData<T>(this IQueryable<T> data, PaginatedQuery paginatedQuery)
    {
        var totalAmount = await data.CountAsync();
        if (totalAmount == paginatedQuery.PageSize)
        {
            paginatedQuery.PageNumber = 1;
        }

        var paginatedList = await data
            .Skip((paginatedQuery.PageNumber - 1) * paginatedQuery.PageSize)
            .Take(paginatedQuery.PageSize).ToListAsync();
        var firstItemIndexList = (paginatedQuery.PageNumber - 1) * paginatedList.Count() + 1;
        var lastItemIndexList = firstItemIndexList + paginatedList.Count() - 1;
        var amountPages = (int)Math.Ceiling((double)totalAmount / paginatedQuery.PageSize);
        var amountPagesList = Enumerable.Range(1, amountPages).ToList();
        var paginatedResult = new PaginatedResults<T>
        {
            Results = paginatedList,
            TotalAmount = totalAmount,
            PageNumber = amountPagesList.Count == 1 ? 1 : paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize,
            FirstItemIndexList = firstItemIndexList,
            LastItemIndexList = lastItemIndexList,
            AmountPagesList = amountPagesList
        };
        return paginatedResult;
    }
}