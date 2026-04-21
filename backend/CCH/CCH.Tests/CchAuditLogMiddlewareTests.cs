using CCH.API.Middlewares;
using CCH.Core.Entities;
using CCH.Core.Interfaces;
using CCH.Services.Repositories.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;
using Xunit;

namespace CCH.Tests;

public class CchAuditLogMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ControllerRequest_ShouldLogToDatabase()
    {
        // Arrange
        var mockLogs = new Mock<DbSet<CchLog>>();
        var mockDbContext = new Mock<CspDbContext>(new DbContextOptions<CspDbContext>());
        mockDbContext.Setup(m => m.CchLog).Returns(mockLogs.Object);

        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(u => u.UserId).Returns("test-user");

        var nextInvoked = false;
        RequestDelegate next = (innerContext) =>
        {
            innerContext.Response.StatusCode = 200;
            byte[] data = Encoding.UTF8.GetBytes("{\"result\":\"success\"}");
            innerContext.Response.Body.Write(data, 0, data.Length);
            nextInvoked = true;
            return Task.CompletedTask;
        };

        var middleware = new CchAuditLogMiddleware(next);
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/test";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{\"input\":\"data\"}"));

        // Mock Endpoint with ControllerActionDescriptor
        var endpoint = new Endpoint(
            context => Task.CompletedTask,
            new EndpointMetadataCollection(new ControllerActionDescriptor
            {
                ControllerName = "TestController",
                ActionName = "TestAction"
            }),
            "Test"
        );
        context.SetEndpoint(endpoint);

        // Act
        await middleware.InvokeAsync(context, mockDbContext.Object, mockUserContext.Object);

        // Assert
        Assert.True(nextInvoked);
        mockLogs.Verify(m => m.Add(It.IsAny<CchLog>()), Times.Once());
        mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_NonControllerRequest_ShouldNotLog()
    {
        // Arrange
        var mockLogs = new Mock<DbSet<CchLog>>();
        var mockDbContext = new Mock<CspDbContext>(new DbContextOptions<CspDbContext>());
        mockDbContext.Setup(m => m.CchLog).Returns(mockLogs.Object);

        var mockUserContext = new Mock<IUserContext>();

        RequestDelegate next = (innerContext) => Task.CompletedTask;
        var middleware = new CchAuditLogMiddleware(next);
        var context = new DefaultHttpContext();
        
        // No Endpoint set or Non-Controller endpoint
        context.SetEndpoint(null);

        // Act
        await middleware.InvokeAsync(context, mockDbContext.Object, mockUserContext.Object);

        // Assert
        mockLogs.Verify(m => m.Add(It.IsAny<CchLog>()), Times.Never());
    }
}
