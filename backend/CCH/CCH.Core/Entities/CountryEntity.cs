namespace CCH.Core.Entities;

/// <summary>
/// Country entity for internal storage.
/// (繁體中文) 內部存儲用的國家實體。
/// </summary>
public class CountryEntity
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
