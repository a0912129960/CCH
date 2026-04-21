using CCH.Core.Entities;
using System.Collections.Generic;

namespace CCH.Core.Constants;

/// <summary>
/// Centralized constants for Part Lifecycle Statuses. (SSoT)
/// (繁體中文) 零件生命週期狀態的集中常數 (單一事實來源)。
/// </summary>
public static class PartStatusConstants
{
    public const string S01 = "S01";
    public const string S02 = "S02";
    public const string S03 = "S03";
    public const string S04 = "S04";
    public const string S05 = "S05";

    /// <summary>
    /// Gets all predefined status entities.
    /// (繁體中文) 取得所有預定義的狀態實體。
    /// </summary>
    public static readonly List<StatusEntity> AllStatuses = new()
    {
        new() { Code = S01, Description = "Unknown" },
        new() { Code = S02, Description = "Pending Dimerco Review" },
        new() { Code = S03, Description = "Pending Customer Review" },
        new() { Code = S04, Description = "Reviewed" },
        new() { Code = S05, Description = "Flagged" }
    };
}
