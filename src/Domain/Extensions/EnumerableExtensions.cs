using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IsotopesStats.Domain.Extensions;

public static class EnumerableExtensions
{
    private static readonly Dictionary<string, string> ColumnToPropertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Player", "PlayerName" },
        { "GP", "GamesPlayed" },
        { "H", "H" },
        { "AB", "AB" },
        { "PA", "PA" },
        { "2B", "H2B" },
        { "3B", "H3B" },
        { "IPHR", "IPHR" },
        { "HR", "HR" },
        { "FC", "FC" },
        { "BB", "BB" },
        { "SF", "SF" },
        { "K", "K" },
        { "KF", "KF" },
        { "GO", "GO" },
        { "FO", "FO" },
        { "R", "R" },
        { "RBI", "RBI" },
        { "AVG", "AVG" },
        { "OBP", "OBP" },
        { "SLG", "SLG" },
        { "OPS", "OPS" },
        { "BO", "BO" }
    };

    /// <summary>
    /// Sorts an enumerable of statistics based on a column name and direction.
    /// </summary>
    public static IEnumerable<T> SortByColumn<T>(this IEnumerable<T> source, string column, bool isAscending)
    {
        if (string.IsNullOrEmpty(column)) return source;

        // Map UI column names to actual property names
        string propertyName = ColumnToPropertyMap.TryGetValue(column, out string? mappedName) 
            ? mappedName 
            : column;

        PropertyInfo? propertyInfo = typeof(T).GetProperty(propertyName);
        if (propertyInfo == null) return source;

        return isAscending 
            ? source.OrderBy(x => propertyInfo.GetValue(x, null)) 
            : source.OrderByDescending(x => propertyInfo.GetValue(x, null));
    }
}
