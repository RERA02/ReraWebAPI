using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReraModel.Common;
using ReraRepo.Services;

namespace ReraWebPortal.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuthController : ControllerBase
{
    private readonly UserStore  _users;
    private readonly AuthService _auth;

    public AuthController(UserStore users, AuthService auth)
    {
        _users = users;
        _auth  = auth;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        AppUser? user;

        if (!string.IsNullOrWhiteSpace(req.apiKey))
        {
            if (!_auth.ValidateApiKey(req.apiKey))
                return Unauthorized(new { message = "Invalid API key." });

            // API key grants a virtual admin session
            user = new AppUser { id = "api-key", username = "api", role = "admin" };
        }
        else if (!string.IsNullOrWhiteSpace(req.username) && !string.IsNullOrWhiteSpace(req.password))
        {
            user = _users.GetByUsername(req.username);
            if (user is null || !user.isActive || !PasswordHelper.Verify(req.password, user.passwordHash))
                return Unauthorized(new { message = "Invalid username or password." });
        }
        else
        {
            return BadRequest(new { message = "Provide username + password, or an API key." });
        }

        return Ok(new LoginResponse
        {
            token     = _auth.GenerateToken(user),
            username  = user.username,
            role      = user.role,
            expiresAt = _auth.TokenExpiry()
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var username = User.Identity?.Name ?? "unknown";
        var role     = User.FindFirstValue(ClaimTypes.Role) ?? "user";
        return Ok(new { username, role });
    }

    //[Authorize]
    [HttpPost("change-password")]
    public IActionResult ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || userId == "api-key")
            return BadRequest(new { message = "Cannot change password for API key sessions." });

        var user = _users.GetById(userId);
        if (user is null) return NotFound(new { message = "User not found." });

        if (!PasswordHelper.Verify(req.currentPassword, user.passwordHash))
            return Unauthorized(new { message = "Current password is incorrect." });

        if (req.newPassword.Length < 6)
            return BadRequest(new { message = "New password must be at least 6 characters." });

        _users.UpdatePassword(userId, PasswordHelper.Hash(req.newPassword));
        return Ok(new { message = "Password changed successfully." });
    }
}
