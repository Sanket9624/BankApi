using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/users")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    private int GetUserIdFromClaims()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    //Register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var result = await _authService.RegisterUserAsync(userDto);
            return Ok(new { message = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //Login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var message = await _authService.LoginAsync(loginDto);

            return Ok(message == "OTP Sent for Verification. Please verify OTP to proceed."
                ? new { message }
                : new { message = "Login Successfully", token = message });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    //Get Personal Details
    [HasPermission(Permissions.ViewPersonalDetails)]
    [HttpGet("me")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var user = await _authService.GetUserByIdAsync(userId);
            return user != null ? Ok(user) : NotFound(new { message = "User not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //Verify-otp
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationDto otpDto, [FromQuery] string flowType)
    {
        try
        {
            var result = await _authService.VerifyOtpAsync(otpDto.Email, otpDto.Otp, flowType);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //Forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var message = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
            return Ok(new { message }); 
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    //Reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var message = await _authService.ResetPasswordAsync(resetPasswordDto.Email, resetPasswordDto.NewPassword);
            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    //2fa enable
    [HasPermission(Permissions.TwoFactorStatus)]
    [HttpPost("toggle-2fa")]
    public async Task<IActionResult> ToggleTwoFactor([FromBody] TwoFactorToggleDto twoFactorDto)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var message = await _authService.ToggleTwoFactorAsync(userId, twoFactorDto.TwoFactorEnabled);
            return Ok(new { message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    //2fa detail
    [HasPermission(Permissions.TwoFactorStatus)]
    [HttpGet("two-factor-status")]
    public async Task<IActionResult> GetTwoFactorStatus()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            bool isEnabled = await _authService.GetTwoFactorStatusAsync(userId);
            return Ok(new { isEnabled });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

 
}
