using MabrukBlazor2026.Shared.ModelsInventario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly MabrukInventarioContext context;

        public CustomerController(MabrukInventarioContext context)
        {
            this.context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<sp_buscar_clienteResult>>> GetCustomers(string filter = "")
        {
            OutputParameter<int> returnValue = new OutputParameter<int>();
            var customers = await context.Procedures.sp_buscar_clienteAsync(filter, returnValue);
            return Ok(customers);
        }


        [HttpGet("customer")]
        public async Task<ActionResult<sp_obtener_clienteResult>> GetCustomer(int id)
        {
            OutputParameter<int> returnValue = null;
            var customers = await context.Procedures.sp_obtener_clienteAsync(id, returnValue);
            return Ok(customers.FirstOrDefault());
        }


    }
}
