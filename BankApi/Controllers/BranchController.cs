using BankApi.Dto.Request;
using BankApi.Entities;
using BankApi.Enums;
using BankApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Route("api/admin/branches")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HasPermission(Permissions.ViewBranch)]
        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            return Ok(branches);
        }

        [HasPermission(Permissions.ViewBranch)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            return branch != null ? Ok(branch) : NotFound();
        }

        [HasPermission(Permissions.ViewBranch)]
        [HttpGet("by-bank/{bankId}")]
        public async Task<IActionResult> GetBranchesByBankId(int bankId)
        {
            var branches = await _branchService.GetBranchesByBankIdAsync(bankId);
            return Ok(branches);
        }

        [HasPermission(Permissions.CreateBranch)]
        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromBody] BranchRequestDto request)
        {
            var result = await _branchService.CreateBranchAsync(request);
            return CreatedAtAction(nameof(GetBranchById), new { id = result.BranchId }, result);
        }

        [HasPermission(Permissions.UpdateBranch)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] BranchRequestDto request)
        {
            var result = await _branchService.UpdateBranchAsync(id, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HasPermission(Permissions.DeleteBranch)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var success = await _branchService.DeleteBranchAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
