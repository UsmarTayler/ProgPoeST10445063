using CMCS.Mvc.Data;
using CMCS.Mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;
using System.Threading;
using Xunit;

public class ClaimsControllerTests
{
    private CmcsContext MakeContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<CmcsContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var ctx = new CmcsContext(options);
        // seed a lecturer so LecturerId is valid
        if (!ctx.Lecturers.Any())
        {
            ctx.Lecturers.Add(new Lecturer { LecturerId = 1, FullName = "Test Lecturer" });
            ctx.SaveChanges();
        }
        return ctx;
    }

    private static IFormFile MakeFile(byte[] bytes, string fileName, string contentType = "application/octet-stream")
    {
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, stream.Length, "upload", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static ControllerContext WithTempData(Controller controller)
    {
        var http = new DefaultHttpContext();
        controller.TempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>());
        return new ControllerContext { HttpContext = http };
    }

    private static IWebHostEnvironment TempEnv(string webRoot)
    {
        var env = new Mock<IWebHostEnvironment>();
        env.SetupGet(e => e.WebRootPath).Returns(webRoot);
        return env.Object;
    }

    [Fact]
    public async Task Create_SavesClaim_WhenValid_NoFiles()
    {
        var db = MakeContext(nameof(Create_SavesClaim_WhenValid_NoFiles));
        var webroot = Path.Combine(Path.GetTempPath(), "cmcs-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(webroot);

        var controller = new ClaimsController(db, TempEnv(webroot));
        controller.ControllerContext = WithTempData(controller);

        var claim = new Claim
        {
            LecturerId = 1,
            Month = "June",
            HoursWorked = 10,
            HourlyRate = 200m,
            Description = "Test"
        };

        var result = await controller.Create(claim, null, null);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(1, db.Claims.Count());
        Assert.Empty(db.SupportingDocuments); // no docs uploaded
    }

    [Fact]
    public async Task Create_InvalidExtension_SetsError_And_DoesNotSaveDocument()
    {
        var db = MakeContext(nameof(Create_InvalidExtension_SetsError_And_DoesNotSaveDocument));
        var webroot = Path.Combine(Path.GetTempPath(), "cmcs-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(webroot);

        var controller = new ClaimsController(db, TempEnv(webroot));
        controller.ControllerContext = WithTempData(controller);

        var claim = new Claim
        {
            LecturerId = 1,
            Month = "July",
            HoursWorked = 5,
            HourlyRate = 100m
        };

        // invalid extension: .exe
        var badFile = MakeFile(Encoding.UTF8.GetBytes("hi"), "virus.exe");

        var result = await controller.Create(claim, badFile, null);

        // redirects to Index even if file invalid (your logic keeps the claim, skips file)
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        Assert.Equal(1, db.Claims.Count());
        Assert.Empty(db.SupportingDocuments); // document not stored

        // TempData should contain the friendly error message
        Assert.True(controller.TempData.ContainsKey("error"));
    }
}
