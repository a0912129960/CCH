namespace CCH.Core.Entities;

/// <summary>
/// Supplier entity associated with a customer.
/// (繁體中文) 與客戶關聯的供應商實體。
/// </summary>
public class SupplierEntity
{
    public int ID { get; set; }
    public int CustomerID { get; set; }
    public string Name { get; set; } = string.Empty;
}
