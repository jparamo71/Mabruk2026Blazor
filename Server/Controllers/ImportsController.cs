using AutoMapper;
using MabrukBlazor2026.Shared.Dtos;
using MabrukBlazor2026.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly MabrukContext context;
        private readonly IMapper mapper;

        public ImportsController(
            MabrukContext context,
            IMapper mapper
            )
        {
            this.context = context;
            this.mapper = mapper;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportDto>>> Get()
        {
            var queryable = context.Documentocompra
                .Include(x => x.TipoDocumentoCompra)
                .Include(x => x.Proveedor)
                .Where(x => x.TipoDocumentoCompraId == "IM")
                .OrderByDescending(x => x.Fecha)
                .Take(30)
                .AsQueryable();
                        
            var imports = mapper.Map<List<ImportDto>>(await queryable.ToListAsync());
            return Ok(imports);            
        }



        /// <summary>
        /// 
        /// Only active Taking Inventory
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<ImportDto>>> Get([FromQuery] PaginationDto page)
        {
            int endRow = page.Skip + page.Take;

            FormattableString query = $@"SELECT tf.*
                FROM    (
                        SELECT	[dc].[DocumentoCompraId] [Id], 
		                        [dc].[TipoDocumentoCompraId] [DocumentTypeId], 
			                    [td].[TipoDocumento] [DocumentTypeName],
		                        [dc].[NumeroDocumento] [DocumentNumber], 
		                        [dc].[ProveedorId] [ProviderId], 
			                    ISNULL([p].[NombreProveedor], '') [ProviderName],
		                        [dc].[Fecha] [DocumentDate], 
		                        [dc].[ValorTotal] [TotalValue], 
		                        [dc].[EstadoDocumentoId] [Status], 
                                ROW_NUMBER() OVER (ORDER BY [dc].[Fecha] DESC) as RowNumber
                        FROM	[documentocompra] AS [dc]
				                    INNER JOIN 
			                    [tipodocumento] [td]
				                    ON [dc].[TipoDocumentoCompraId] = [td].[TipoDocumentoId]
				                    LEFT JOIN 
			                    [proveedor] [p]
				                    ON [dc].[ProveedorId] = [p].[ProveedorId]
	                    WHERE	[dc].[TipoDocumentoCompraId] = 'IM'
                        ) tf
                WHERE	tf.RowNumber > {page.Skip} AND tf.RowNumber <= {endRow}";

            var queryable = context.ViewImportDocs
                .FromSql(query)
                .AsQueryable();

            var querycount = context.Documentocompra
                .Where(x => x.TipoDocumentoCompraId == "IM")
                .AsQueryable();

            var count = await querycount.CountAsync();
            HttpContext.Response.Headers.Add("totalRows", count.ToString());

            return Ok(queryable.ToList());
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ImportDto>> GetSingle(int id)
        {
            var importDB = await context.Documentocompra
                .Include(x => x.TipoDocumentoCompra)
                .Include(x => x.Proveedor)
                .Include(x => x.Detalledocumentocompra)
                    .ThenInclude(x => x.Producto)
                .FirstOrDefaultAsync(x => x.DocumentoCompraId == id);

            if (importDB == null)
            {
                return NotFound();
            }

            var import = mapper.Map<ImportDto>(importDB);
            return Ok(import);
        }



        [HttpPut]
        public async Task<ActionResult> Put(ProductPhysicalDto model)
        {
            var importDetailDB = await context.Detalledocumentocompra
                .FirstOrDefaultAsync(x => x.DocumentoCompraId == model.Id
                    && x.ProductoId == model.ProductId);

            if (importDetailDB == null) 
            {
                return NotFound();
            }

            importDetailDB.CantidadVerificacion = (decimal)model.PhysicalQuantity;
            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("startverification/{id}")]
        public async Task<ActionResult> PutStart(int id)
        {
            var documentDb = await context.Documentocompra
                .FirstOrDefaultAsync(x => x.DocumentoCompraId == id);

            if (documentDb == null)
            {
                return NotFound();
            }

            documentDb.EstadoVerificacionId = "EVR";
            await context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPut("finishverification/{id}")]
        public async Task<ActionResult> PutFinish(int id)
        {
            var documentDb = await context.Documentocompra
                .FirstOrDefaultAsync(x => x.DocumentoCompraId == id);

            if (documentDb == null)
            {
                return NotFound();
            }

            documentDb.EstadoVerificacionId = "VRF";
            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
