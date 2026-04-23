using CCH.Core.Entities;
using CCH.Core.Entities.CSP;
using CCH.Core.Entities.ReSm;
using CCH.Core.Interfaces.Repositories;
using CCH.Services.Features.Common;
using Moq;
using Xunit;

namespace CCH.Tests;

public class CommonServiceTests
{
    [Fact]
    public void GetCustomers_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ICommonRepository>();
        mockRepo.Setup(r => r.GetCustomers()).Returns(new List<SmCustomer>
        {
            new() { HQID = 1, CustomerName = "Customer A" },
            new() { HQID = 2, CustomerName = "Customer B" }
        });
        var service = new CommonService(mockRepo.Object);

        var result = service.GetCustomers().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].Key);
        Assert.Equal("Customer A", result[0].Value);
    }

    [Fact]
    public void GetCountries_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ICommonRepository>();
        mockRepo.Setup(r => r.GetCountries()).Returns(new List<CountryEntity>
        {
            new() { ID = 1, Code = "US", Name = "United States" },
            new() { ID = 2, Code = "TW", Name = "Taiwan" }
        });
        var service = new CommonService(mockRepo.Object);

        var result = service.GetCountries().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].Key);
        Assert.Equal("United States", result[0].Value);
    }

    [Fact]
    public void GetSuppliers_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ICommonRepository>();
        var mockSuppliers = new List<CchSuppliers>
        {
            new() { ID = 1, SupplierName = "Supplier A", CustomerID = 101 },
            new() { ID = 2, SupplierName = "Supplier B", CustomerID = 101 }
        };
        mockRepo.Setup(r => r.GetSuppliers(101)).Returns(mockSuppliers);
        var service = new CommonService(mockRepo.Object);

        var result = service.GetSuppliers("101").ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].Key);
        Assert.Equal("Supplier A", result[0].Value);
    }

    [Fact]
    public void GetStatus_ReturnsMappedDtos()
    {
        var mockRepo = new Mock<ICommonRepository>();
        mockRepo.Setup(r => r.GetStatuses()).Returns(new List<StatusEntity>
        {
            new() { Code = "S01", Description = "Draft" }
        });
        var service = new CommonService(mockRepo.Object);

        var result = service.GetStatus().ToList();

        Assert.Single(result);
        Assert.Equal("S01", result[0].Key);
        Assert.Equal("Draft", result[0].Value);
    }
}
