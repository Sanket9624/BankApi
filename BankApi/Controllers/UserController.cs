using BankApi.Dto;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    //[Authorize(Policy = "CustomerOnly")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromQuery] UserRequestDto userRequestDto)
    {
        var result = await _userService.RegisterUserAsync(userRequestDto);
        return Ok(new { message = result });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)

    {
        var token = await _userService.LoginAsync(loginDto);
        return Ok(new { token });
    }

    //[Authorize(Policy = "CustomerOnly")]
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
