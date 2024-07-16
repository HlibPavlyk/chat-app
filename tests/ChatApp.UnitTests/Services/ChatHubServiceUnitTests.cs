using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Services;
using ChatApp.Domain.Entities;
using Moq;

namespace ChatApp.UnitTests.Services;

public class ChatHubServiceUnitTests
{
    private readonly Mock<IChatHubRepository> _mockChatHubRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ChatHubService _chatHubService;

    public ChatHubServiceUnitTests()
    {
        _mockChatHubRepository = new Mock<IChatHubRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _chatHubService = new ChatHubService(_mockChatHubRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task SendMessageAsync_ValidSender_AddsMessageAndCompletesUnitOfWork()
    {
        var sender = new SenderDto { Username = "user1", Message = "Hello World" };
        _mockChatHubRepository.Setup(x => x.SendMessageAsync(sender.Username, sender.Message)).ReturnsAsync(1);
        _mockUnitOfWork.Setup(x => x.Messages.AddAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);

        await _chatHubService.SendMessageAsync(sender);

        _mockChatHubRepository.Verify(x => x.SendMessageAsync(sender.Username, sender.Message), Times.Once);
        _mockUnitOfWork.Verify(x => x.Messages.AddAsync(It.IsAny<Message>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task JoinChatAsync_ChatExists_UserJoinsChat()
    {
        var chatId = 1;
        var user = new UserDto { Username = "user1" };
        var chat = new Chat { Id = chatId };
        _mockUnitOfWork.Setup(x => x.Chats.GetByIdAsync(chatId)).ReturnsAsync(chat);
        _mockChatHubRepository.Setup(x => x.JoinChatAsync(It.IsAny<string>(), user.Username, chatId)).Returns(Task.CompletedTask);

        await _chatHubService.JoinChatAsync(chatId, user);

        _mockChatHubRepository.Verify(x => x.JoinChatAsync(It.IsAny<string>(), user.Username, chatId), Times.Once);
    }

    [Fact]
    public async Task JoinChatAsync_ChatDoesNotExist_ThrowsArgumentNullException()
    {
        var chatId = 999;
        var user = new UserDto { Username = "user1" };
        _mockUnitOfWork.Setup(x => x.Chats.GetByIdAsync(chatId)).ReturnsAsync(value: null);

        await Assert.ThrowsAsync<ArgumentNullException>(() => _chatHubService.JoinChatAsync(chatId, user));
    }

    [Fact]
    public async Task LeaveChatAsync_ValidUser_UserLeavesChat()
    {
        var user = new UserDto { Username = "user1" };
        _mockChatHubRepository.Setup(x => x.LeaveChatAsync(It.IsAny<string>(), user.Username)).Returns(Task.CompletedTask);

        await _chatHubService.LeaveChatAsync(user);

        _mockChatHubRepository.Verify(x => x.LeaveChatAsync(It.IsAny<string>(), user.Username), Times.Once);
    }
}