using Banking_System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Banking.Controllers.Installments
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallmentsController : ControllerBase
    {

        private readonly ContextApi _contextApi;

        public InstallmentsController(ContextApi context)
        {
            _contextApi = context;
        }



        [HttpGet]
        public async Task<IActionResult> GetAllInstallments()
        {
            var installments = await _contextApi.Installments.ToListAsync();
            return Ok(installments);
        }



        //[HttpPost]


    }
}
