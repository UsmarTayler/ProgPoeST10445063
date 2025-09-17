using CMCS.Mvc.Models;
namespace CMCS.Mvc.Services
{
    public interface IDummyDataProvider
    {
        List<Claim> GetAll();
        List<Claim> GetPending();
    }
}