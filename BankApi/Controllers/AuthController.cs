using BankApi.Dto;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/users")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _userService;

    public AuthController(IAuthService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register( UserRequestDto userRequestDto)
    {
        var result = await _userService.RegisterUserAsync(userRequestDto);
        return Ok(new { message = result });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)

    {
        var token = await _userService.LoginAsync(loginDto);
        return Ok(new { message = "Login Succesfully.",token });
    }

    [Authorize(Policy = "AllUsers")]
    [HttpGet("me")]
    public async Task<IActionResult> GetUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }
        return Ok(user);
    }

}
