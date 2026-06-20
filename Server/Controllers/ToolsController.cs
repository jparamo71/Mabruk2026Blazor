using MabrukBlazor2026.Shared.Dtos;
using MabrukBlazor2026.Shared.ModelsInventario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolsController : ControllerBase
    {
        private readonly MabrukInventarioContext context;

        public ToolsController(MabrukInventarioContext context)
        {
            this.context = context;
        }


        [HttpPost("send-email")]
        public async Task<ActionResult> PostSendEmail([FromQuery] SendEmailDto model)
        {
            try
            {
                OutputParameter<string> errMessage = new OutputParameter<string>();
                OutputParameter<int?> error = new OutputParameter<int?>();
                OutputParameter<int> returnValue = new OutputParameter<int>();

                var result = await context.Procedures.sp_enviar_correoAsync(model.PedidoId, model.Email, errMessage, error, returnValue);
                if (returnValue.Value != 0)
                {
                    return NoContent();
                }
                return BadRequest($"Error: {error.Value}: {errMessage.Value}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        [HttpDelete("delete-document")]
        public async Task<ActionResult> DeleteDocument([FromQuery] SendEmailDto model)
        {
            try
            {
                OutputParameter<string> errMessage = new OutputParameter<string>();
                OutputParameter<int?> error = new OutputParameter<int?>();
                OutputParameter<int> returnValue = new OutputParameter<int>();

                var result = await context.Procedures.sp_eliminar_pedidoAsync(model.PedidoId, model.Email, errMessage, error, returnValue);
                if (error.Value == 0)
                {
                    return NoContent();
                }
                return BadRequest($"Error: {error.Value}: {errMessage.Value}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }



}