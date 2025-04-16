using BankApi.Dto.Request;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [Route("api/admin/banks")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HasPermission(Permissions.ViewBank)]
        [HttpGet]
        public async Task<IActionResult> GetAllBanks()
        {
            var banks = await _bankService.GetAllBanksAsync();
            return Ok(banks);
        }

        [HasPermission(Permissions.ViewBank)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankById(int id)
        {
            var bank = await _bankService.GetBankByIdAsync(id);
            return bank != null ? Ok(bank) : NotFound();
        }

        [HasPermission(Permissions.CreateBank)]
        [HttpPost]
        public async Task<IActionResult> CreateBank([FromBody] BankRequestDto request)
        {
            var result = await _bankService.CreateBankAsync(request);
            return CreatedAtAction(nameof(GetBankById), new { id = result.BankId }, result);
        }

        [HasPermission(Permissions.UpdateBank)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] BankRequestDto request)
        {
            var result = await _bankService.UpdateBankAsync(id, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HasPermission(Permissions.DeleteBank)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            var success = await _bankService.DeleteBankAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}