using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Entities;
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
        var message = await _userService.LoginAsync(loginDto);
        if (message == "OTP Sent for Verification. Please verify OTP to proceed.")
        {
            return Ok(new { message });
        }

        return Ok(new { message = "Login Successfully", token = message });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationDto dto, [FromQuery] string flowType , AccountType accountType)
    {
        var result = await _userService.VerifyOtpAsync(dto.Email, dto.Otp, flowType,accountType);
        return Ok(result);
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

    [Authorize(Policy = "AllUsers")]
    [HttpPut("toggle-2fa")]
    public async Task<IActionResult> ToggleTwoFactor([FromBody] TwoFactorToggleDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var message = await _userService.ToggleTwoFactorAsync(userId, dto.TwoFactorEnabled);
        return Ok(new { message });
    }
    [Authorize(Policy = "AllUsers")]
    [HttpGet("two-factor-status")]
    public async Task<IActionResult> GetTwoFactorStatus()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isEnabled = await _userService.GetTwoFactorStatusAsync(userId);
            return Ok(new { TwoFactorEnabled = isEnabled });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var message = await _userService.ForgotPasswordAsync(dto.Email);
        return Ok(new { message });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var message = await _userService.ResetPasswordAsync(dto.Email, dto.Otp, dto.NewPassword);
        return Ok(new { message });
    }

}
