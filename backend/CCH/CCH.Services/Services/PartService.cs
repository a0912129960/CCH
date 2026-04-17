using CCH.Core.DTOs;
using CCH.Core.Interfaces;
using System.Text;

namespace CCH.Services.Services;

/// <summary>
/// Mock part service.
/// (繁體中文) 模擬零件服務。
/// </summary>
public class PartService : IPartQueryService, IPartLifecycleService, IPartExcelService
{
    // INTERNAL-AI-20260416: Expanded mock to 6 distinct items for end-to-end list → detail flow.
    // (INTERNAL-AI-20260416: 擴充為 6 筆各自獨立的 mock 資料，支援清單→詳情完整流程。)
    /* SearchParts returned only 2 hardcoded rows — replaced below */
    // INTERNAL-AI-20260416: Expanded to 18 mock items with varied status/supplier/customer.
    // (INTERNAL-AI-20260416: 擴充為 18 筆各式 mock 資料，涵蓋各種狀態、供應商與客戶。)
    public PartListResponseDto SearchParts(string? customerId, string? status, string? partNo, string? supplier, int page, int pageSize) => new()
    {
        Total = 18,
        Page = page,
        Data = new[]
        {
            new PartListItemDto { Id =  1, Customer = "Customer A", PartNo = "PART-001", PartDesc = "5G Comm Module",          Country = "TW", HtsCode = "8471.30.0000", Rate = 0,   Supplier = "Supplier Alpha", Status = "S04", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now,               SlaStatus = "green"  },
            new PartListItemDto { Id =  2, Customer = "Customer A", PartNo = "PART-002", PartDesc = "Portable Laptop Unit",    Country = "CN", HtsCode = "8517.12.0000", Rate = 5,   Supplier = "Supplier Beta",  Status = "S02", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-1),   SlaStatus = "yellow" },
            new PartListItemDto { Id =  3, Customer = "Customer A", PartNo = "PART-003", PartDesc = "Router Switch",           Country = "JP", HtsCode = "8517.62.0000", Rate = 0,   Supplier = "Supplier Gamma", Status = "S03", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-2),   SlaStatus = "red"    },
            new PartListItemDto { Id =  4, Customer = "Customer A", PartNo = "PART-004", PartDesc = "Server Blade",            Country = "US", HtsCode = "8471.50.0000", Rate = 10,  Supplier = "Supplier Alpha", Status = "S01", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-3),   SlaStatus = "green"  },
            new PartListItemDto { Id =  5, Customer = "Customer A", PartNo = "PART-005", PartDesc = "LCD Monitor",             Country = "TW", HtsCode = "8528.52.0000", Rate = 3,   Supplier = "Supplier Delta", Status = "S02", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-4),   SlaStatus = "yellow" },
            new PartListItemDto { Id =  6, Customer = "Customer A", PartNo = "PART-006", PartDesc = "Wireless Receiver",       Country = "CN", HtsCode = "8517.62.0000", Rate = 7,   Supplier = "Supplier Beta",  Status = "S04", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-5),   SlaStatus = "green"  },
            new PartListItemDto { Id =  7, Customer = "Customer B", PartNo = "PART-007", PartDesc = "Network Adapter",         Country = "TW", HtsCode = "8517.62.0000", Rate = 0,   Supplier = "Supplier Alpha", Status = "S03", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-6),   SlaStatus = "red"    },
            new PartListItemDto { Id =  8, Customer = "Customer B", PartNo = "PART-008", PartDesc = "Base Station Component",  Country = "CN", HtsCode = "8517.12.0000", Rate = 8,   Supplier = "Supplier Gamma", Status = "S04", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-7),   SlaStatus = "green"  },
            new PartListItemDto { Id =  9, Customer = "Customer B", PartNo = "PART-009", PartDesc = "Power Supply Unit",       Country = "US", HtsCode = "8504.40.0000", Rate = 4,   Supplier = "Supplier Delta", Status = "S02", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-8),   SlaStatus = "yellow" },
            new PartListItemDto { Id = 10, Customer = "Customer B", PartNo = "PART-010", PartDesc = "Cooling Fan Assembly",    Country = "JP", HtsCode = "8414.59.0000", Rate = 0,   Supplier = "Supplier Beta",  Status = "S01", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-9),   SlaStatus = "green"  },
            new PartListItemDto { Id = 11, Customer = "Customer B", PartNo = "PART-011", PartDesc = "SSD Storage Module",      Country = "KR", HtsCode = "8471.70.0000", Rate = 2,   Supplier = "Supplier Alpha", Status = "S04", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-10),  SlaStatus = "green"  },
            new PartListItemDto { Id = 12, Customer = "Customer C", PartNo = "PART-012", PartDesc = "GPU Accelerator Card",    Country = "TW", HtsCode = "8473.30.0000", Rate = 12,  Supplier = "Supplier Gamma", Status = "S02", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-11),  SlaStatus = "yellow" },
            new PartListItemDto { Id = 13, Customer = "Customer C", PartNo = "PART-013", PartDesc = "Memory DIMM 32GB",        Country = "CN", HtsCode = "8473.30.0000", Rate = 0,   Supplier = "Supplier Delta", Status = "S04", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-12),  SlaStatus = "green"  },
            new PartListItemDto { Id = 14, Customer = "Customer C", PartNo = "PART-014", PartDesc = "PCIe Expansion Card",     Country = "US", HtsCode = "8473.30.0000", Rate = 6,   Supplier = "Supplier Beta",  Status = "S03", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-13),  SlaStatus = "red"    },
            new PartListItemDto { Id = 15, Customer = "Customer C", PartNo = "PART-015", PartDesc = "USB Hub Controller",      Country = "JP", HtsCode = "8473.30.0000", Rate = 0,   Supplier = "Supplier Alpha", Status = "S01", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-14),  SlaStatus = "green"  },
            new PartListItemDto { Id = 16, Customer = "Customer C", PartNo = "PART-016", PartDesc = "Optical Drive Unit",      Country = "TW", HtsCode = "8471.70.0000", Rate = 1,   Supplier = "Supplier Gamma", Status = "S02", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-15),  SlaStatus = "yellow" },
            new PartListItemDto { Id = 17, Customer = "Customer C", PartNo = "PART-017", PartDesc = "Touchpad Module",         Country = "CN", HtsCode = "8471.60.0000", Rate = 0,   Supplier = "Supplier Delta", Status = "S04", UpdatedBy = "dcb001",      UpdatedDate = DateTime.Now.AddDays(-16),  SlaStatus = "green"  },
            new PartListItemDto { Id = 18, Customer = "Customer C", PartNo = "PART-018", PartDesc = "Display Panel 15.6 inch", Country = "KR", HtsCode = "8524.11.0000", Rate = 9,   Supplier = "Supplier Beta",  Status = "S02", UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-17),  SlaStatus = "yellow" },
        }
    };

    public byte[] ExportParts(string? customerId, string? status, string? partNo, string? supplier) => 
        Encoding.UTF8.GetBytes("Mock Excel Content");

    public object BatchAccept(IEnumerable<int> partIds) => new { failed = new List<object>() };

    public List<BulkUploadResponseDto> BulkUpload(Stream fileStream) => new List<BulkUploadResponseDto>()
    {
        new() { PartId = 1, Error = "error" },
        new() { PartId = 2, Error = "error" }
    };

    public byte[] GetUploadTemplate() => Encoding.UTF8.GetBytes("Mock Template Content");

    // INTERNAL-AI-20260416: Returns null when partId <= 0 to simulate 404 not found scenario.
    // (INTERNAL-AI-20260416: 當 partId <= 0 時回傳 null，模擬 404 找不到的情境。)
    /* public PartDetailResponseDto GetPartDetail(int partId) => new()
    {
        Before = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "Old Desc", HtsCode = "8471.30", Rate = 0 },
        Modified = new PartDetailDto { PartNo = "PART-001", Country = "TW", Division = "DIV1", Supplier = "SUP1", PartDesc = "New Desc", HtsCode = "8471.30", Rate = 0 }
    }; */
    // INTERNAL-AI-20260416: Returns different mock data per partId matching SearchParts items.
    // (INTERNAL-AI-20260416: 依 partId 回傳對應的 mock 資料，與 SearchParts 清單一致。)
    public PartDetailResponseDto? GetPartDetail(int partId)
    {
        // Simulate 404: return null when partId is not in mock set (模擬 404：partId 不在 mock 集合時回傳 null)
        // INTERNAL-AI-20260416: Added Status to mock tuple so frontend can display status badge.
        // (INTERNAL-AI-20260416: 在 mock tuple 中加入 Status，供前端顯示狀態標籤。)
        var mock = partId switch
        {
            1  => (PartNo: "PART-001", Country: "TW", Division: "DIV-A", Supplier: "Supplier Alpha", PartDesc: "5G Comm Module",          HtsCode: "8471.30.0000", Rate: 0m,   HtsCode1: "8517.12.0000", Rate1: (decimal?)5.5m,  Remark: "Updated HTS code",         Status: "S04"),
            2  => (PartNo: "PART-002", Country: "CN", Division: "DIV-B", Supplier: "Supplier Beta",  PartDesc: "Portable Laptop Unit",    HtsCode: "8517.12.0000", Rate: 5m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "Initial entry",             Status: "S02"),
            3  => (PartNo: "PART-003", Country: "JP", Division: "DIV-A", Supplier: "Supplier Gamma", PartDesc: "Router Switch",           HtsCode: "8517.62.0000", Rate: 0m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "Returned for correction",   Status: "S03"),
            4  => (PartNo: "PART-004", Country: "US", Division: "DIV-C", Supplier: "Supplier Alpha", PartDesc: "Server Blade",            HtsCode: "8471.50.0000", Rate: 10m,  HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S01"),
            5  => (PartNo: "PART-005", Country: "TW", Division: "DIV-B", Supplier: "Supplier Delta", PartDesc: "LCD Monitor",             HtsCode: "8528.52.0000", Rate: 3m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "Pending review",            Status: "S02"),
            6  => (PartNo: "PART-006", Country: "CN", Division: "DIV-C", Supplier: "Supplier Beta",  PartDesc: "Wireless Receiver",       HtsCode: "8517.62.0000", Rate: 7m,   HtsCode1: "8528.52.0000", Rate1: (decimal?)2.0m,  Remark: "Dual HTS classification",   Status: "S04"),
            7  => (PartNo: "PART-007", Country: "TW", Division: "DIV-A", Supplier: "Supplier Alpha", PartDesc: "Network Adapter",         HtsCode: "8517.62.0000", Rate: 0m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S03"),
            8  => (PartNo: "PART-008", Country: "CN", Division: "DIV-B", Supplier: "Supplier Gamma", PartDesc: "Base Station Component",  HtsCode: "8517.12.0000", Rate: 8m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S04"),
            9  => (PartNo: "PART-009", Country: "US", Division: "DIV-C", Supplier: "Supplier Delta", PartDesc: "Power Supply Unit",       HtsCode: "8504.40.0000", Rate: 4m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "Pending review",            Status: "S02"),
            10 => (PartNo: "PART-010", Country: "JP", Division: "DIV-A", Supplier: "Supplier Beta",  PartDesc: "Cooling Fan Assembly",    HtsCode: "8414.59.0000", Rate: 0m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S01"),
            11 => (PartNo: "PART-011", Country: "KR", Division: "DIV-B", Supplier: "Supplier Alpha", PartDesc: "SSD Storage Module",      HtsCode: "8471.70.0000", Rate: 2m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S04"),
            12 => (PartNo: "PART-012", Country: "TW", Division: "DIV-C", Supplier: "Supplier Gamma", PartDesc: "GPU Accelerator Card",    HtsCode: "8473.30.0000", Rate: 12m,  HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "Pending review",            Status: "S02"),
            13 => (PartNo: "PART-013", Country: "CN", Division: "DIV-A", Supplier: "Supplier Delta", PartDesc: "Memory DIMM 32GB",        HtsCode: "8473.30.0000", Rate: 0m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S04"),
            14 => (PartNo: "PART-014", Country: "US", Division: "DIV-B", Supplier: "Supplier Beta",  PartDesc: "PCIe Expansion Card",     HtsCode: "8473.30.0000", Rate: 6m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "Returned for correction",   Status: "S03"),
            15 => (PartNo: "PART-015", Country: "JP", Division: "DIV-C", Supplier: "Supplier Alpha", PartDesc: "USB Hub Controller",      HtsCode: "8473.30.0000", Rate: 0m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S01"),
            16 => (PartNo: "PART-016", Country: "TW", Division: "DIV-A", Supplier: "Supplier Gamma", PartDesc: "Optical Drive Unit",      HtsCode: "8471.70.0000", Rate: 1m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "Pending review",            Status: "S02"),
            17 => (PartNo: "PART-017", Country: "CN", Division: "DIV-B", Supplier: "Supplier Delta", PartDesc: "Touchpad Module",         HtsCode: "8471.60.0000", Rate: 0m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S04"),
            18 => (PartNo: "PART-018", Country: "KR", Division: "DIV-C", Supplier: "Supplier Beta",  PartDesc: "Display Panel 15.6 inch", HtsCode: "8524.11.0000", Rate: 9m,   HtsCode1: (string?)null,  Rate1: (decimal?)null,  Remark: "",                          Status: "S02"),
            _  => default
        };

        if (mock == default) return null;

        return new PartDetailResponseDto
        {
            Status   = mock.Status,
            Before   = new PartDetailDto { PartNo = mock.PartNo, Country = mock.Country, Division = mock.Division, Supplier = mock.Supplier, PartDesc = mock.PartDesc + " (prev)", HtsCode = mock.HtsCode, Rate = mock.Rate,   Remark = "",           UpdatedBy = "customer001", UpdatedDate = DateTime.Now.AddDays(-5) },
            Modified = new PartDetailDto { PartNo = mock.PartNo, Country = mock.Country, Division = mock.Division, Supplier = mock.Supplier, PartDesc = mock.PartDesc,            HtsCode = mock.HtsCode, Rate = mock.Rate, HtsCode1 = mock.HtsCode1, Rate1 = mock.Rate1, Remark = mock.Remark, UpdatedBy = "customer001", UpdatedDate = DateTime.Now }
        };
    }

    public object CreatePart(PartSaveRequest request, string status) => new { partId = 3, partNo = request.PartNo, status };

    public object UpdatePart(int partId, PartSaveRequest request) => new { };

    public object SubmitPart(int partId, PartSaveRequest request) => new { partId, status = "S02" };

    public object AcceptPart(int partId) => new { partId, status = "S04" };

    public object ReturnPart(int partId, string returnReason) => new { partId, status = "S03" };

    public object InactivatePart(int partId) => new { partId, status = "Inactive" };

    public IEnumerable<MilestoneDto> GetMilestones(int partId) => new[]
    {
        new MilestoneDto { Action = "Unknown", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now.AddDays(-5), Remark = "" },
        new MilestoneDto { Action = "Pending Dimerco Review", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now.AddDays(-4), Remark = "" },
        new MilestoneDto { Action = "Pending Customer Review", UpdatedBy = "DCB001", UpdatedDate = DateTime.Now.AddDays(-3), Remark = "test remark" },
        new MilestoneDto { Action = "Pending Dimerco Review", UpdatedBy = "Customer001", UpdatedDate = DateTime.Now.AddDays(-2), Remark = "" },
        new MilestoneDto { Action = "Reviewed", UpdatedBy = "DCB001", UpdatedDate = DateTime.Now.AddDays(-1), Remark = "" }
    };

    public IEnumerable<PartDetailDto> GetHistory(int partId) => new[]
    {
        new PartDetailDto 
        { 
            PartNo = "PART-001", 
            Country = "TW",
            Division = "DIV1",
            Supplier = "SUP1",
            PartDesc = "Version 2 (Current)", 
            HtsCode = "8471.30",
            Rate = 0,
            Remark = "Updated HTS description",
            UpdatedBy = "Customer001",
            UpdatedDate = DateTime.Now.AddDays(-1)
        },
        new PartDetailDto 
        { 
            PartNo = "PART-001", 
            Country = "TW",
            Division = "DIV1",
            Supplier = "SUP1",
            PartDesc = "Version 1 (Initial)", 
            HtsCode = "8471.30",
            Rate = 0,
            Remark = "Initial entry",
            UpdatedBy = "Customer001",
            UpdatedDate = DateTime.Now.AddDays(-5)
        }
    };
}
