using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicMedia.Metadata;
using Xunit;

namespace MagicMedia.Tests.Core
{
    public class DateTakenParserTests
    {
        [Theory]
        [InlineData("19760402_123015")]
        public async Task Parse_Match(string dateString)
        {
            // Arrange
            var parser = new DateTakenParser();

            // Act
            DateTime? date = parser.Parse(dateString);

            // Assert
            date.Should().Be(new DateTime(1976, 4, 2, 12, 30, 15));
        }
    }
}
