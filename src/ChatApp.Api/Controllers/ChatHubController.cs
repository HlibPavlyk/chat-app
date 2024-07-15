using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/chat-hub")]
public class ChatHubController : Controller
{
    private readonly IChatHubService _chatHubService;
    
    public ChatHubController(IChatHubService chatHubService)
    {
        _chatHubService = chatHubService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessageToChatAsync([FromBody] SenderDto sender)
    {
        try
        {
            await _chatHubService.SendMessageAsync(sender);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("join/{id}")]
    public async Task<IActionResult> JoinToChatAsync(int id, [FromBody] UserDto user)
    {
        try
        {
            await _chatHubService.JoinChatAsync(id, user);
            return NoContent();
        }
        catch (ArgumentNullException e)
        {
            return NotFound(e.Message);
        }    
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("leave/{id}")]
    public async Task<IActionResult> LeaveToChatAsync([FromBody] UserDto user)
    {
        try
        {
            await _chatHubService.LeaveChatAsync(user);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}