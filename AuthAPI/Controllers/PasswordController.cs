using AuthAPI.Dto.Users;
using AuthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers;

[Route("api/v1/password")]
[ApiController]
public class PasswordController : ControllerBase
{
    private readonly IAccountService _accountService;

    public PasswordController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPut("change")]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] UserChangePasswordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var changed = await _accountService.ChangePassword(userId, dto);

        if (changed is null) return Conflict(new {error_message = "Cannot change password"});

        return Ok(changed);
    }

    [HttpPost("recover")]
    public Task<IActionResult> Recover(Guid userId)
    {
        // Requires email service
        return Task.FromResult<IActionResult>(Ok());
    }
}