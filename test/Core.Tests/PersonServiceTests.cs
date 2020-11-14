using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagicMedia.Messaging;
using MagicMedia.Store;
using MassTransit;
using Moq;
using Xunit;

namespace MagicMedia.Tests.Core
{
    public class PersonServiceTests
    {
        [Fact]
        public async Task GetOrCreatePerson_VerifiedAddCalled()
        {
            // Arrange
            Mock<IPersonStore> storeMock = CreatePersonStoreMock();
            Mock<IBus> busMock = CreateBusMock();

            var service = new PersonService(storeMock.Object, null, null, null, busMock.Object);

            string name = "Bart";

            // Act
            Person person = await service.GetOrCreatePersonAsync(name, default);

            // Assert
            person.Name.Should().Be(name);

            storeMock
                .Verify(x => x.AddAsync(
                   It.Is<Person>(p => p.Name == name),
                   It.IsAny<CancellationToken>()),
                 Times.Once);

            busMock
              .Verify(m => m.Publish(
                    It.Is<PersonUpdatedMessage>(m => m.Id == person.Id),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private Mock<IPersonStore> CreatePersonStoreMock()
        {
            var mock = new Mock<IPersonStore>();
            mock
                .Setup(m => m.TryGetByNameAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            mock
                .Setup(m => m.AddAsync(
                    It.IsAny<Person>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person p, CancellationToken c) =>
                {
                    return p;
                });

            return mock;
        }

        public Mock<IBus> CreateBusMock()
        {
            var mock = new Mock<IBus>();
            mock
                .Setup(m => m.Publish(
                    It.IsAny<PersonUpdatedMessage>(),
                    It.IsAny<CancellationToken>()));

            return mock;
        }
    }
}
