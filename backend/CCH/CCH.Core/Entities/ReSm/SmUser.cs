using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCH.Core.Entities.ReSm;

/// <summary>
/// Ported from MyDimerco. Represents a user in the ReSM database.
/// (繁體中文) 從 MyDimerco 移植。代表 ReSM 資料庫中的使用者。
/// </summary>
[Table("SMUser")]
public partial class SmUser
{
    [Key]
    public int HQID { get; set; }
    public string UserID { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }

    public string StationID { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? Birthday { get; set; }

    public string Password { get; set; }
    public string Email { get; set; }

    public string Title { get; set; }
    public string Status { get; set; }

    public string Admin { get; set; }
}
