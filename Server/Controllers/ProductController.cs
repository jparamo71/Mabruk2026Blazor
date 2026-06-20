using AutoMapper;
using MabrukBlazor2026.Server.Helpers;
using MabrukBlazor2026.Shared.Dtos;
using MabrukBlazor2026.Shared.Models;
using MabrukBlazor2026.Shared.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MabrukContext context;
        private readonly IMapper mapper;

        public ProductController(
            MabrukContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }



        /*
         * Function to use in UI for Update Product's Code
         * Search product using Code Field
         * If not found remove characters in filter to try to find
         * */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get(string filter)
        {
            var products = await GetProducts(filter);

            bool removeFirst = false;
            int intent = 1;
            while (!products.Any() && intent < 4)
            {
                if (filter.Length > 0)
                {
                    filter = filter.Remove((removeFirst ? filter.Length - 1 : 0), 1);
                    if (filter.Length > 0)
                    {
                        products = await GetProducts(filter);
                        if (products.Any())
                        {
                            return Ok(products);
                        }
                    }
                    removeFirst = !removeFirst;
                    intent++;
                }
            }

            return Ok(products);

        }


        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetPaged([FromQuery] PaginationDto page)
        {
            int endRow = page.Skip + page.Take;

            FormattableString query;

            if (string.IsNullOrEmpty(page.Filter))
            {
                query = $@"SELECT	t.*
                FROM	(
		                SELECT	[p].[ProductoId], 
				                [p].[Activo], 
				                [p].[Arancel], 
				                [p].[CantidadMinima], 
				                [p].[ClaseId], 
				                [p].[ClasificacionProducto], 
				                [p].[Codigo], 
				                [p].[CodigoProducto], 
				                [p].[CodigoUPC], 
				                [p].[Dimensiones], 
				                [p].[EmpresaId], 
				                [p].[FechaAlta], 
				                [p].[Imagen], 
				                [p].[LargoImpresion], 
				                [p].[Manufacturado], 
				                [p].[MarcaId], 
				                [p].[MaterialId], 
				                [p].[NombreProducto], 
				                [p].[Peso], 
				                [p].[PorcentajeMaximoRebaja], 
				                [p].[PresentacionId], 
				                [p].[Producido], 
				                [p].[RebajaDirecta], 
				                [p].[RebajaParcial], 
				                [p].[RutaImagen], 
				                [p].[Servicio], 
				                [p].[SubCategoriaId], 
				                [p].[TipoId], 
				                [p].[UnidadMedidaId], 
				                [p].[UnidadesPorCaja], 
				                [p].[UsuarioIngresoId], 
				                [p].[ValorCosto], 
				                [p].[ValorCostoSinIVA], 
				                [p].[ValorUnitario],
				                ROW_NUMBER() OVER (ORDER BY p.ProductoId) as RowNumber
		                FROM    Producto p
                        WHERE   p.Activo <> 0
                                AND
                                p.MarcaId = (
                                            CASE 
                                            WHEN {page.MarkId} > 0 THEN {page.MarkId}
                                            ELSE p.MarcaId
                                            END
                                )
                        ) t
                WHERE	t.RowNumber > {page.Skip} AND t.RowNumber <= {endRow}";
            }
            else
            {
                string filtered = $"%{page.Filter}%";
                query = $@"SELECT	t.*
                FROM	(
		                SELECT	[p].[ProductoId], 
				                [p].[Activo], 
				                [p].[Arancel], 
				                [p].[CantidadMinima], 
				                [p].[ClaseId], 
				                [p].[ClasificacionProducto], 
				                [p].[Codigo], 
				                [p].[CodigoProducto], 
				                [p].[CodigoUPC], 
				                [p].[Dimensiones], 
				                [p].[EmpresaId], 
				                [p].[FechaAlta], 
				                [p].[Imagen], 
				                [p].[LargoImpresion], 
				                [p].[Manufacturado], 
				                [p].[MarcaId], 
				                [p].[MaterialId], 
				                [p].[NombreProducto], 
				                [p].[Peso], 
				                [p].[PorcentajeMaximoRebaja], 
				                [p].[PresentacionId], 
				                [p].[Producido], 
				                [p].[RebajaDirecta], 
				                [p].[RebajaParcial], 
				                [p].[RutaImagen], 
				                [p].[Servicio], 
				                [p].[SubCategoriaId], 
				                [p].[TipoId], 
				                [p].[UnidadMedidaId], 
				                [p].[UnidadesPorCaja], 
				                [p].[UsuarioIngresoId], 
				                [p].[ValorCosto], 
				                [p].[ValorCostoSinIVA], 
				                [p].[ValorUnitario],
				                ROW_NUMBER() OVER (ORDER BY p.ProductoId) as RowNumber
		                FROM    Producto p
                        WHERE   p.Activo <> 0 
                                AND 
                                (ISNULL(p.CodigoProducto, '') + ' ' + p.NombreProducto) like {filtered}
                                AND
                                p.MarcaId = (
                                            CASE 
                                            WHEN {page.MarkId} > 0 THEN {page.MarkId}
                                            ELSE p.MarcaId
                                            END
                                )
                        ) t
                WHERE	t.RowNumber > {page.Skip} AND t.RowNumber <= {endRow}";
            }

            FormattableString fm = $"";

            var queryable = context.Producto
                .FromSql(query)
                .Include(x => x.Marca)
                .AsQueryable();


            var querycount = context.Producto.Where(x => x.Activo == true).AsQueryable();

            if (!string.IsNullOrEmpty(page.Filter))
            {
                querycount = querycount.Where(x => x.CodigoProducto.Contains(page.Filter!) || x.NombreProducto.Contains(page.Filter));
                /*var count = await context.Productos
                    .Where(x => x.Activo == true && x.CodigoProducto.Contains(page.Filter!))
                    .CountAsync();
                HttpContext.Response.Headers.Add("totalRows", count.ToString());*/
            }/*
            else
            {
                var count = await context.Productos
                    .Where(x => x.Activo == true)
                    .CountAsync();
                HttpContext.Response.Headers.Add("totalRows", count.ToString());
            }*/

            if (page.MarkId > 0)
            {
                querycount = querycount.Where(x => x.MarcaId == page.MarkId);
            }

            var count = await querycount.CountAsync();
            HttpContext.Response.Headers.Add("totalRows", count.ToString());

            var productsMapp = mapper.Map<IEnumerable<ProductDto>>(queryable.ToList());
            return Ok(productsMapp);

        }


        [HttpGet("tolist")]
        public async Task<ActionResult<IEnumerable<ProductItemList>>> GetToList([FromQuery] PaginationDto page)
        {
            int endRow = page.Skip + page.Take;

            FormattableString query;

            if (string.IsNullOrEmpty(page.Filter))
            {
                query = $@"SELECT	t.*
                FROM	(
		                SELECT	[p].[ProductoId] ProductId, 				                
				                [p].[CodigoProducto] BarcharCode, 
				                [p].[CodigoUPC] UPCCode, 				                
				                [p].[MarcaId] MarkId, 
				                [p].[NombreProducto] Name,
                                [m].Marca MarkName,
                                ISNULL(
                                        (SELECT SUM([i].InventarioDisponible) 
                                        FROM    Inventario [i] 
                                        WHERE   [i].ProductoId = [p].ProductoId
                                        ), 0
                                ) Available,
				                ROW_NUMBER() OVER (ORDER BY p.ProductoId) as RowNumber
		                FROM    Producto [p]
                                    LEFT JOIN 
                                Marca [m]
                                    ON [p].MarcaId = [m].MarcaId
                        WHERE   [p].Activo <> 0
                                AND
                                [p].MarcaId = (
                                            CASE 
                                            WHEN {page.MarkId} > 0 THEN {page.MarkId}
                                            ELSE [p].MarcaId
                                            END
                                )
                        ) t
                WHERE	t.RowNumber > {page.Skip} AND t.RowNumber <= {endRow}";
            }
            else
            {
                string filtered = $"%{page.Filter}%";
                query = $@"SELECT	t.*
                FROM	(
		                SELECT	[p].[ProductoId] ProductId, 				                
				                [p].[CodigoProducto] BarcharCode, 
				                [p].[CodigoUPC] UPCCode, 				                
				                [p].[MarcaId] MarkId, 
				                [p].[NombreProducto] Name,
                                [m].Marca MarkName,
                                ISNULL(
                                        (SELECT SUM([i].InventarioDisponible) 
                                        FROM    Inventario [i] 
                                        WHERE   [i].ProductoId = [p].ProductoId
                                        ), 0
                                ) Available,
				                ROW_NUMBER() OVER (ORDER BY p.ProductoId) as RowNumber
		                FROM    Producto [p]
                                    LEFT JOIN 
                                Marca [m]
                                    ON [p].MarcaId = [m].MarcaId
                        WHERE   [p].Activo <> 0
                                AND 
                                (ISNULL([p].CodigoProducto, '') + ' ' + [p].NombreProducto) like {filtered}
                                AND
                                [p].MarcaId = (
                                            CASE 
                                            WHEN {page.MarkId} > 0 THEN {page.MarkId}
                                            ELSE [p].MarcaId
                                            END
                                )
                        ) t
                WHERE	t.RowNumber > {page.Skip} AND t.RowNumber <= {endRow}";
            }

            FormattableString fm = $"";

            var queryable = context.ViewProducts
                .FromSql(query)
                .AsQueryable();


            var querycount = context.Producto.Where(x => x.Activo == true).AsQueryable();

            if (!string.IsNullOrEmpty(page.Filter))
            {
                querycount = querycount.Where(x => x.CodigoProducto.Contains(page.Filter!) || x.NombreProducto.Contains(page.Filter));
            }

            if (page.MarkId > 0)
            {
                querycount = querycount.Where(x => x.MarcaId == page.MarkId);
            }

            var count = await querycount.CountAsync();
            HttpContext.Response.Headers.Add("totalRows", count.ToString());

            return Ok(queryable.ToList());

        }


        [HttpGet("filters")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFiltered([FromQuery] ProductsFilterDto filter)
        {
            var query = context.Producto
                .Include(x => x.Marca)
                .Select(r => new ProductDto
                {
                    Id = r.ProductoId,
                    Code = r.Codigo,
                    BarcharCode = r.CodigoProducto,
                    UnitPrice = r.ValorUnitario??0,
                    UPCCode = r.CodigoUpc,
                    Name = r.NombreProducto,
                    IsActive = r.Activo,
                    MarkId = r.MarcaId ?? 0,
                    MarkName = r.Marca!.Marca1,
                    UnitsByPackage = r.UnidadesPorCaja ?? 0,
                    ImageUrl = r.RutaImagen,
                    Available = context.Inventario
                                            .Where(i => i.ProductoId == r.ProductoId)
                                            .Sum(i => i.InventarioDisponible) ?? 0

                })
                .AsQueryable();
            if (!string.IsNullOrEmpty(filter.TextToSearch))
            {
                query = query.Where(x => x.Code!.Contains(filter.TextToSearch) ||
                    x.Name.Contains(filter.TextToSearch) ||
                    x.MarkName!.Contains(filter.TextToSearch));
            }
            if (filter.SelectedMarkId > 0)
            {
                query = query.Where(x => x.MarkId == filter.SelectedMarkId);
            }
            if (filter.OnlyWithStock)
            {
                query = query.Where(x => context.Inventario
                    .Any(i => i.ProductoId == x.Id && i.InventarioDisponible > 0));
            }
            if (filter.IsActive)
            {
                query = query.Where(x => x.IsActive == true);
            }
            var products = await query.ToListAsync();
            return Ok(mapper.Map<IEnumerable<ProductDto>>(products));
        }


        [HttpGet("single/{code}")]
        public async Task<ActionResult<ProductDto>> GetSingle(string code)
        {
            var productDb = await context.Producto
                .Include(x => x.Marca)
                .FirstOrDefaultAsync(x => x.CodigoProducto == code);
            if (productDb == null)
            {
                return NotFound();
            }

            var product = mapper.Map<ProductDto>(productDb);
            return product;
        }


        [HttpGet("filterbymark/{code}/{markid}")]
        public async Task<ActionResult<ProductDto>> GetSingle(string code, int markid = 0)
        {

            var productDb = await context.Producto
                .Include(x => x.Marca)
                .FirstOrDefaultAsync(x => (x.CodigoProducto == code ||
                    x.CodigoUpc == code) && x.MarcaId == (markid > 0 ? markid : x.MarcaId));

            if (productDb == null)
            {
                return NotFound();
            }

            var product = mapper.Map<ProductDto>(productDb);

            var inventoryQuantity = await context.Inventario
                .Where(x => x.ProductoId == product.Id && x.InventarioDisponible > 0)
                .SumAsync(x => x.InventarioDisponible);

            product.Available = (decimal)(inventoryQuantity ?? 0);

            return product;
        }


        [HttpGet("filterbydocument/{code}/{docid}")]
        public async Task<ActionResult<ProductDto>> GetSingleByDoc(string code, int docid = 0)
        {
            try
            {
                var productsInDoc = await context.Detalledocumentocompra
                    .Where(x => x.DocumentoCompraId == docid)
                    .ToListAsync();

                if (!productsInDoc.Any())
                {
                    return BadRequest();
                }

                var listOfProductsId = productsInDoc.Select(x => x.ProductoId).ToArray();

                var productDb = await context.Producto
                    .Include(x => x.Marca)
                    .Where(x =>
                        listOfProductsId.Contains(x.ProductoId)
                        && (x.CodigoUpc == code) || x.CodigoProducto == code)
                    .FirstOrDefaultAsync();

                if (productDb == null)
                {
                    return NotFound();
                }

                var product = mapper.Map<ProductDto>(productDb);

                var quantityInDoc = productsInDoc!
                    .FirstOrDefault(x => x.ProductoId == product.Id)!
                    .Cantidad;

                product.Available = (decimal)quantityInDoc;

                return product;
            }
            catch (Exception ex)
            { throw; }
        }


        [HttpPost("updatebarcode")]
        public async Task<ActionResult> Post(ProductDto model)
        {
            var productDb = await context.Producto
                .FirstOrDefaultAsync(x => x.Codigo == model.Code);

            if (productDb == null)
            {
                return NotFound();
            }

            productDb.CodigoProducto = model.BarcharCode;
            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("updateupc")]
        public async Task<ActionResult> PostUPC(UpdateUPCCodeDto model)
        {
            var productDb = await context.Producto
                .FirstOrDefaultAsync(x => x.CodigoProducto == model.BarCharCode);

            if (productDb == null)
            {
                return NotFound();
            }

            productDb.UnidadesPorCaja = model.UnitsByPackage;
            productDb.CodigoUpc = model.UPCCode;
            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("get")]
        public async Task<ActionResult<ProductDto>> GetProduct(string code)
        {
            var productDb = await context.Producto.FirstOrDefaultAsync(x => x.CodigoProducto == code);
            if (productDb == null)
            {
                return NotFound();
            }

            var product = mapper.Map<ProductDto>(productDb);
            return Ok(product);
        }


        [HttpGet("image/{name}")]
        public async Task<ActionResult> GetImage(string name)
        {
            string drive = @"C:\";
            var imagePath = Path.Combine(drive, "inetpub", "wwwroot", "ws", "products_images", name);
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }
            var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            return File(imageBytes, "image/jpeg");
        }


        [HttpGet("products-by-brand")]
        public async Task<ActionResult<IEnumerable<sp_BuscarProductoResult>>> GetProductStock(
            int brandId,
            bool stockAvailable,
            string? searchText = "")
        {
            try
            {
                OutputParameter<int> returnValue = new OutputParameter<int>();

                var products = await context.Procedures.sp_buscar_productoAsync(
                    criterio: searchText,
                    marcaId: brandId > 0 ? brandId : null,
                    soloDisponible: stockAvailable,
                    returnValue);

                if (returnValue.Value != 0)
                {
                    return BadRequest("Error al buscar productos.");
                }
                return Ok(products);
            } catch(Exception e)
            {
                return BadRequest(e.Message);
            }

        }



        [HttpPut("upload-image")]
        public async Task<ActionResult> UploadImage([FromBody] UploadFileDto file)
        {
            try
            {
                var productDb = await context.Producto
                .FirstOrDefaultAsync(x => x.ProductoId == file.ProductId);
                if (productDb == null)
                {
                    return NotFound();
                }

                // Generating unique file name
                string drive = @"C:\";
                var imagesFolder = Path.Combine(drive, "inetpub", "wwwroot", "ws", "products_images");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }
                var byteArray = Convert.FromBase64String(file.FileContent);

                var fileExtension = Path.GetExtension(file.FileName);
                var newFileName = $"{productDb.Codigo}{fileExtension}";
                var filePath = Path.Combine(imagesFolder, newFileName);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    fs.Close();
                }

                productDb.RutaImagen = newFileName;
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al subir la imagen: {ex.Message}");
            }
        }


        #region Private



        private async Task<IEnumerable<ProductDto>> GetProducts(string filter)
        {
            var prds = await context.Producto
                .Include(x => x.Marca)
                .Where(x => x.Codigo.Contains(filter))
                .ToListAsync();
            return mapper.Map<IEnumerable<ProductDto>>(prds);
        }



        #endregion

    }
}
