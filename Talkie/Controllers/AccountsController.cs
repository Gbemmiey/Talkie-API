using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talkie.DTOs.Account;

namespace Talkie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("All")]
        public async Task<ActionResult<ServiceResponse<List<GetAccountDto>>>> GetAllAccounts()
        {
            return Ok(await _accountService.GetAllAccounts());
        }

        [Authorize]
        [HttpGet("{AccountNumber}")]
        public async Task<ActionResult<ServiceResponse<List<GetAccountDto>>>> GetAnAccount(string AccountNumber)
        {
            return Ok(await _accountService.GetAccount(AccountNumber));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetAccountDto>>> AddAccount(AddAccountDto newAccount)
        {
            return Ok(await _accountService.CreateAccount(newAccount));
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetAccountDto>>> ModifyAccount(UpdateAccountDto updateAccount)
        {
            return Ok(await _accountService.ModifyAccount(updateAccount));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<GetProfileDto>>> GetProfile()
        {
            return Ok(await _accountService.GetProfile());
        }
    }
}