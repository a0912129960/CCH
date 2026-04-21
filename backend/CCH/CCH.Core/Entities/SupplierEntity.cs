namespace CCH.Core.Entities;

/// <summary>
/// Supplier entity associated with a customer.
/// (繁體中文) 與客戶關聯的供應商實體。
/// </summary>
public class SupplierEntity
{
    public int ID { get; set; }
    public int? CustomerID { get; set; }
    
    // For compatibility with existing code using .Name (為了與現有程式碼的 .Name 相容)
    public string Name { get; set; } = string.Empty;
    
    // For consistency with CCHSuppliers table (為了與 CCHSuppliers 資料表一致)
    public string? SupplierName { get; set; }
    
    public string? Status { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}
