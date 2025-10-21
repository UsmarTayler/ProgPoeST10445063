using CMCS.Mvc.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace CMCS.Tests
{
    public class ClaimTests
    {
        [Fact]
        public void TotalAmount_Computes_Hours_Times_Rate()
        {
            var c = new Claim { HoursWorked = 12.5, HourlyRate = 200m };
            Assert.Equal(2500m, c.TotalAmount);
        }

        [Fact]
        public void DefaultStatus_IsPending()
        {
            var c = new Claim();
            Assert.Equal(ClaimStatus.Pending, c.Status);
        }

        [Fact]
        public void Validation_Fails_When_HoursWorked_Is_Zero()
        {
            var c = new Claim
            {
                LecturerId = 1,
                Month = "April",
                HoursWorked = 0,         // invalid per [Range]
                HourlyRate = 150m
            };

            var ctx = new ValidationContext(c);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(c, ctx, results, validateAllProperties: true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Claim.HoursWorked)));
        }
    }
}
