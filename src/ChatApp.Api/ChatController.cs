using ChatApp.Application.Dto;
using ChatApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api;

[ApiController]
[Route("api/chats")]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChatAsync([FromBody] ChatPostDto chat)
    {
        try
        {
            var createdChat = await _chatService.CreateChatAsync(chat);
            return Created(createdChat.Id.ToString(), createdChat);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChatByIdAsync(int id)
    {
        try
        {
            var chat = await _chatService.GetChatByIdAsync(id);
            return Ok(chat);
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
    
   
    [HttpGet("search")]
    public async Task<IActionResult> GetChatsAsync([FromQuery] string? title = null, [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        try
        {
            var chats = await _chatService.GetChatsWithSearchAsync(title, page, size);
            return Ok(chats);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChatAsync(int id, [FromBody] UserDto user)
    {
        try
        {
            await _chatService.DeleteChatAsync(id, user);
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
}