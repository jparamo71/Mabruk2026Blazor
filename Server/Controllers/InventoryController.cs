using AutoMapper;
using MabrukBlazor2026.Shared.Dtos;
using MabrukBlazor2026.Shared.Models;
using MabrukBlazor2026.Shared.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly MabrukContext context;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccesor;

        public InventoryController(
            MabrukContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccesor)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContextAccesor = httpContextAccesor;
        }



        /// <summary>
        /// 
        /// Only active Taking Inventory
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<ImportDocument>>> Get([FromQuery]PaginationDto page)
        {
            int endRow = page.Skip + page.Take;

            FormattableString query = $@"SELECT tf.*
            FROM    (
                    SELECT	[t].[TomaFisicaId], 
		                    [t].[DocumentoCompraId], 
		                    [t].[DocumentoEnvioId], 
		                    [t].[EmpresaId], 
		                    [t].[Estado], 
		                    [t].[Fecha], 
		                    [t].[MarcaId], 
		                    [t].[UsuarioId],
                            ROW_NUMBER() OVER (ORDER BY [t].[Fecha] DESC) as RowNumber
                    FROM	[tomafisica] AS [t]
                    ) tf
            WHERE	tf.RowNumber > {page.Skip} AND tf.RowNumber <= {endRow}";

            var queryable = context.Tomafisica
                .FromSql(query)
                .Include(x => x.Marca)
                .AsQueryable();

            var querycount = context.Tomafisica.AsQueryable();

            var count = await querycount.CountAsync();
            HttpContext.Response.Headers.Append("totalRows", count.ToString());

            var inventories = mapper.Map<IEnumerable<InventoryTakingDto>>(queryable.ToList());
            return Ok(inventories);

        }



        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryTakingDto>> GetSingle(int id)
        {
            var inventoryDB = await context.Tomafisica
                .Include(x => x.Marca)
                .Include(x => x.Detalletomafisica)
                    .ThenInclude(x => x.Producto)
                .FirstOrDefaultAsync(x => x.TomaFisicaId == id);

            if (inventoryDB == null)
            {
                return NotFound();
            }

            var tomaFisica = mapper.Map<InventoryTakingDto>(inventoryDB);
            return Ok(tomaFisica);
        }



        /// <summary>
        /// Start new inventory taking register
        /// </summary>
        /// <returns></returns>
        [HttpPut("startaking")]
        public async Task<ActionResult> PostTaking(InventoryTakingDto model)
        {
            var tomaFisica = new OutputParameter<int?>();
            var errorNumber = new OutputParameter<int?>();
            var errorMessage = new OutputParameter<string>();

            var result = await context.Procedures.sp_update_tomafisicaAsync(DateTime.Now,
                1, model.MarkId, 1, tomaFisica, errorMessage, errorNumber);

            var valueError = errorNumber?._value ?? 0;
            int tomaFisicaId = tomaFisica._value ?? 0;
            if (valueError == 0)
            {
                var context = httpContextAccesor?.HttpContext;
                if (context != null)  
                {                    
                    context.Session.SetString("TomaFisicaId", tomaFisicaId.ToString());
                    return NoContent();
                }
            }
            return BadRequest();            
        }



        [HttpPost("updateproduct")]
        public async Task<ActionResult> Post(ProductPhysicalDto model)
        {
            try
            {
                var ti = await context.Tomafisica
                .FirstOrDefaultAsync(x => x.TomaFisicaId == model.InventoryTakingId);

                if (ti == null)
                {
                    return BadRequest($"La toma física no ha podido ser identificada {model.InventoryTakingId}");
                }

                if (ti.Estado == "AJU")
                {
                    return BadRequest("La toma física ya fue concluida y no puede agregarse un nuevo producto");
                }

                var detailDB = await context.Detalletomafisica
                    .FirstOrDefaultAsync(x => x.TomaFisicaId == model.InventoryTakingId
                        && x.ProductoId == model.ProductId);

                if (detailDB == null)
                {
                    return BadRequest("El producto no corresponde con la toma física");
                }
                else
                {
                    detailDB.DisponibleFisico = (decimal)model.PhysicalQuantity;
                    detailDB.Diferencia = (decimal)model.Difference;
                    detailDB.Observaciones = model.Notes;
                }

                await context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTakingInventory(int id)
        {
            var tfDb = await context.Tomafisica
                .FirstOrDefaultAsync(x => x.TomaFisicaId == id);

            if (tfDb == null)
            {
                return NotFound();
            }

            if (tfDb.Estado.Equals("REG"))
            {
                context.Detalletomafisica
                    .Where(x => x.TomaFisicaId == id)
                    .ExecuteDelete();

                context.Tomafisica.Remove(tfDb);
                await context.SaveChangesAsync();
                return NoContent(); 
            }

            return BadRequest("La toma física no puede eliminarse");
        }
    }
}
