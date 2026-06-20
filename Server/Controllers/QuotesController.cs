using AutoMapper;
using MabrukBlazor2026.Shared.Dtos;
using MabrukBlazor2026.Shared.Models;
using MabrukBlazor2026.Shared.ModelsInventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly MabrukInventarioContext context;
        private readonly MabrukContext contextMabruk;
        private readonly IMapper mapper;
        private readonly IConverter converter;
        private readonly IWebHostEnvironment env;

        public QuotesController(
            MabrukInventarioContext context,
            MabrukContext contextMabruk,
            IMapper mapper,
            IConverter converter, 
            IWebHostEnvironment env)
        {
            this.context = context;
            this.contextMabruk = contextMabruk;
            this.mapper = mapper;
            this.converter = converter;
            this.env = env;
        }



        [HttpGet("documents")]
        public async Task<ActionResult<IEnumerable<OrderRequestDto>>> GetQuotes(
            string email,
            int type)
        {
            try
            {
                Shared.ModelsInventario.OutputParameter<int> returnValue = new Shared.ModelsInventario.OutputParameter<int>();
                var orders = await context.Procedures.sp_listado_pedidosAsync(email, type, returnValue);

                var results = mapper.Map<IEnumerable<OrderRequestDto>>(orders);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("document")]
        public async Task<ActionResult<ResponseDto>> GetDocument(int id)
        {
            try
            {
                var quoteExists = await context.Pedido.AnyAsync(x => x.PedidoId == id); //&& x.EsCotizacion);
                if (quoteExists)
                {
                    return Ok(await QuoteDataService.GetQuote(context, contextMabruk, id));
                }

                return NotFound("No se encontró la cotización solicitada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("create")]
        public async Task<ActionResult> CreateQuote([FromQuery] StartQuoteDto model)
        {
            try
            {
                Shared.ModelsInventario.OutputParameter<int?> quoteId = new Shared.ModelsInventario.OutputParameter<int?>();
                Shared.ModelsInventario.OutputParameter<int?> error = new Shared.ModelsInventario.OutputParameter<int?>();
                Shared.ModelsInventario.OutputParameter<string> errMessage = new Shared.ModelsInventario.OutputParameter<string>();
                Shared.ModelsInventario.OutputParameter<int> returnValue = new Shared.ModelsInventario.OutputParameter<int>();
                var result = await context.Procedures.sp_iniciar_pedidoAsync(
                    model.CustomerId,
                    model.SellerEmail,
                    model.IsQuote,
                    quoteId,
                    errMessage,
                    error,
                    returnValue);

                if (error._value == 0)
                {
                    return Ok(await QuoteDataService.GetQuote(context, contextMabruk, quoteId.Value ?? 0));
                    
                }

                return BadRequest($"No se pudo crear la cotización. {error._value} este fue el error {error.Value} ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("confirm")]
        public async Task<ActionResult> ConfirmQuote(int id, string email)
        {
            try
            {
                Shared.ModelsInventario.OutputParameter<string> errMessage = new Shared.ModelsInventario.OutputParameter<string>();
                Shared.ModelsInventario.OutputParameter<int?> error = new Shared.ModelsInventario.OutputParameter<int?>();
                Shared.ModelsInventario.OutputParameter<int> returnValue = new Shared.ModelsInventario.OutputParameter<int>();
                var result = await context.Procedures.sp_confirmar_pedidoAsync(id, email, errMessage, error, returnValue);

                if (error.Value == 0)
                {
                    return Ok(error.Value);
                }

                return BadRequest(errMessage.Value.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("add-product")]
        public async Task<ActionResult> AddProductToQuote([FromBody] DocumentDetailDto model)
        {
            try
            {
                Shared.ModelsInventario.OutputParameter<int?> documentDetailId = new Shared.ModelsInventario.OutputParameter<int?>();
                Shared.ModelsInventario.OutputParameter<string> errMessage = new Shared.ModelsInventario.OutputParameter<string>();
                Shared.ModelsInventario.OutputParameter<int?> error = new Shared.ModelsInventario.OutputParameter<int?>();
                Shared.ModelsInventario.OutputParameter<int> returnValue = new Shared.ModelsInventario.OutputParameter<int>();

                documentDetailId._value = model.DetailId;
                var result = await context.Procedures.sp_actualizar_detalle_pedidoAsync(
                    model.DocumentId,
                    model.ProductId,
                    model.Quantity,
                    model.UnitPrice,
                    model.TotalPrice,
                    documentDetailId, errMessage, error, returnValue);
                if (error.Value == 0)
                {
                    return Ok(error.Value);
                }

                return BadRequest(errMessage.Value.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("delete-product/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                Shared.ModelsInventario.OutputParameter<string> errMessage = new Shared.ModelsInventario.OutputParameter<string>();
                Shared.ModelsInventario.OutputParameter<int?> error = new Shared.ModelsInventario.OutputParameter<int?>();
                Shared.ModelsInventario.OutputParameter<int> returnValue = new Shared.ModelsInventario.OutputParameter<int>();
                var result = await context.Procedures.sp_eliminar_detalle_pedidoAsync(id, errMessage, error, returnValue);
                if (error.Value == 0)
                {
                    return Ok(error.Value);
                }
                return BadRequest(errMessage.Value.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("update-note")]
        public async Task<ActionResult> UpdateNote([FromBody]UpdateNoteDto model)
        {
            try
            {
                var pedidoDB = await context.Pedido
                    .FirstOrDefaultAsync(x => x.PedidoId == model.Id);

                if (pedidoDB == null)
                {
                    return NotFound("No se encontró la cotización solicitada.");
                }

                pedidoDB.Observaciones = model.Note;
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("generate-pdf/{id}")]
        public async Task<ActionResult<string>> GeneratePDF(int id)
        {

            // Getting Quote information from database
            Pedido? pedido = await context.Pedido.FirstOrDefaultAsync(x =>x.PedidoId == id);
            if (pedido == null)
            {
                return NotFound();
            }

            Cliente? cliente = await contextMabruk.Cliente.FirstOrDefaultAsync(y => y.ClienteId == pedido.ClienteId);
            if (cliente == null)
            {
                return NotFound();
            }


            string htmlContenido = $@"
    <html>
    <head>
        <style>
            body {{ font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; color: #333; margin: 30px; }}
            .header-table {{ width: 100%; border-collapse: collapse; margin-bottom: 30px; }}
            .logo {{ width: 120px; }}
            .empresa-info {{ text-align: right; font-size: 12px; color: #666; }}
            .titulo {{ font-size: 24px; color: #2C3E50; font-weight: bold; }}
            .detalles-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
            .detalles-table th {{ background-color: #2C3E50; color: white; padding: 10px; text-align: left; }}
            .detalles-table td {{ padding: 10px; border-bottom: 1px solid #ddd; font-size: 14px; }}
            .total-row {{ font-weight: bold; font-size: 16px; background-color: #f9f9f9; }}
        </style>
    </head>
    <body>
        <!-- Encabezado con Logo y Datos -->
        <table class='header-table'>
            <tr>
                <td>
                    <img src='logo.png' class='logo' />
                    <div class='titulo'>COTIZACIÓN</div>
                </td>
                <td class='empresa-info'>
                    <strong>Mabruk S.A.</strong><br/>
                    Nit: 123456-7<br/>
                    info@mabrukgt.com
                </td>
            </tr>
        </table>

        <!-- Datos del Cliente -->
        <div style='margin-bottom: 20px;'>
            <strong>Atención a:</strong><br/>
            NIT: {cliente.Nit}<br />
            Nombre: {cliente.NombreComercial}<br/>
            Fecha: {DateTime.Now:dd/MM/yyyy}
        </div>

        <!-- Tabla de Productos -->
        <table class='detalles-table'>
            <thead>
                <tr>
                    <th>Descripción</th>
                    <th style='text-align: right;'>Cantidad</th>
                    <th style='text-align: right;'>Precio Unitario</th>
                    <th style='text-align: right;'>Total</th>
                </tr>
            </thead>
            <tbody>";

            string body = "";
            foreach (var item in pedido.DetallePedido)
            {
                Producto? producto = await contextMabruk.Producto.FirstOrDefaultAsync(x => x.ProductoId == item.ProductoId);
                if (producto == null)
                {
                    return NotFound("producto");
                }
                body += $@"
                <tr>
                    <td>{producto.NombreProducto}</td>
                    <td style='text-align: right;'>{item.Cantidad?.ToString("N2")}</td>
                    <td style='text-align: right;'>{item.ValorUnitario?.ToString("N2")}</td>
                    <td style='text-align: right;'>{item.ValorTotal?.ToString("N2")}</td>
                </tr>";
            }


            string footer = $@"                
                <tr class='total-row'>
                    <td colspan='3' style='text-align: right;'>Total:</td>
                    <td style='text-align: right;'>{pedido.ValorTotal.ToString("N2")}</td>
                </tr>
            </tbody>
        </table>
    </body>
    </html>";

            htmlContenido += body + footer;

            string nombreArchivo = "CT_Mabruk2026.pdf";
            string carpetaDestino = Path.Combine(env.WebRootPath, "pdf");

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Out = rutaCompleta // Guarda el archivo directamente en el disco del servidor
            },
                Objects = {
                new ObjectSettings() {
                    PagesCount = true,
                    HtmlContent = htmlContenido,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            // Ejecuta la conversión física
            converter.Convert(doc);

            return rutaCompleta;
        }
    }


    /// Servicio de datos
    public static class QuoteDataService {


        // Retorna la data completa de una cotización,
        // incluyendo información del cliente, vendedor y detalles de productos.
        // Cuando se solicita esta función ya debe garantizarse
        // que el id corresponde a una cotización válida.
        public static async Task<OrderRequestDto> GetQuote(
            MabrukInventarioContext context,
            MabrukContext contextMabruk, 
            int id)
        {
            var pedidoDB = await context.Pedido
                .Include(x => x.EstadoPedido)
                .Where(x => x.PedidoId == id)
                .Select(x => new OrderRequestDto
                {
                    Id = x.PedidoId,
                    OrderDate = x.Fecha,
                    OrderDateStr = x.Fecha.ToString("dd/MM/yyyy"),
                    TotalAmount = x.ValorTotal,
                    IsQuoted = x.EsCotizacion,
                    Note = x.Observaciones,
                    AddressDelivery = x.DireccionEntrega,
                    CustomerId = x.ClienteId,
                    CustomerName = "",
                    CustomerNit = "",
                    OrderStateId = x.EstadoPedidoId,
                    OrderStateName = x.EstadoPedido.NombreEstadoPedido,
                    SellerId = x.VendedorId,
                    SellerName = "",
                    SellerEmail = ""
                })
                .FirstOrDefaultAsync();

            var clienteDB = await contextMabruk.Cliente
                .FirstOrDefaultAsync(x => x.ClienteId == pedidoDB.CustomerId);

            if (clienteDB != null)
            {
                pedidoDB.CustomerName = clienteDB.NombreComercial;
                pedidoDB.CustomerNit = clienteDB.Nit;
            }

            var vendedorDB = await contextMabruk.Usuario
                .FirstOrDefaultAsync(x => x.CorreoElectronico == pedidoDB.SellerId);

            if (vendedorDB != null)
            {
                pedidoDB.SellerName = vendedorDB.NombreUsuario;
                pedidoDB.SellerEmail = vendedorDB.CorreoElectronico;
            }


            var detalleDB = await context.DetallePedido
                .Where(x => x.PedidoId == id)
                .ToListAsync();

            detalleDB.ForEach(p =>
            {
                var productoDB = contextMabruk.Producto
                        .Include(x => x.Marca)
                        .FirstOrDefault(y => y.ProductoId == p.ProductoId);

                var avialableStock = contextMabruk.Inventario
                    .Where(x => x.ProductoId == p.ProductoId)
                    .Sum(x => x.InventarioDisponible);

                var detalle = new OrderRequestDetailDto
                {
                    DetailId = p.DetallePedidoId,
                    ProductId = p.ProductoId ?? 0,
                    ProductCode = productoDB!.Codigo,
                    ProductName = productoDB.NombreProducto,
                    QuantityAvailable = avialableStock ?? 0,
                    BrandName = productoDB.Marca.Marca1,
                    Quantity = p.Cantidad ?? 0,
                    UnitPrize = p.ValorUnitario ?? 0,
                    Total = p.ValorTotal ?? 0
                };

                pedidoDB.Details.Add(detalle);

            });

            return pedidoDB;
        }

    }
}
