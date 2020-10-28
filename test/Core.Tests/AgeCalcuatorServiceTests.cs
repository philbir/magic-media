using System;
using FluentAssertions;
using Xunit;

namespace MagicMedia.Tests.Core
{
    public class AgeCalculatorServiceTests
    {
        [Fact]
        public void CalculateAge_ReturnsExpectedAge()
        {
            // Arrange
            AgeCalculatorService ageService = new AgeCalculatorService();
            DateTime dateOfBirth = new DateTime(1976, 4, 2);
            DateTimeOffset dateTaken = new DateTimeOffset(new DateTime(2020, 10, 29));

            // Act
            int? ageInMonths = ageService.CalculateAge(dateTaken, dateOfBirth);

            // Assert
            ageInMonths.Should().Be(534);
        }

        [Fact]
        public void CalculateAge_WithNullDateTaken_ReturnNull()
        {
            // Arrange
            AgeCalculatorService ageService = new AgeCalculatorService();
            DateTime dateOfBirth = new DateTime(1976, 4, 2);

            // Act
            int? ageInMonths = ageService.CalculateAge(null, dateOfBirth);

            // Assert
            ageInMonths.Should().BeNull();
        }
    }
}
