using Chat.Application.Commands;
using Chat.Domain.Aggregates.MessageAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class AddMessageCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly AddMessageCommandHandler _addMessageCommandHandler;

    public AddMessageCommandHandlerTests()
    {
        Mock<IMessageRepository> mockMessageRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.MessageRepository).Returns(mockMessageRepository.Object);

        _addMessageCommandHandler = new AddMessageCommandHandler(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_MessageCreated_ReturnsMessageId()
    {
        // Arrange
        AddMessageCommand command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "user", "message");
        _mockRepositoryManager.Setup(x => x.MessageRepository.CreateAsync(It.IsAny<Message>())).ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _addMessageCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.MessageRepository.CreateAsync(It.IsAny<Message>()), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }
}
