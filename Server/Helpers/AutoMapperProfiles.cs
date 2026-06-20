using AutoMapper;
using MabrukBlazor2026.Shared.Dtos;
using MabrukBlazor2026.Shared.Models;
using MabrukBlazor2026.Shared.ModelsInventario;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace MabrukBlazor2026.Server.Helpers
{
    public class AutoMapperProfiles : Profile
    {


        public AutoMapperProfiles()
        {

            CreateMap<Producto, ProductDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ProductoId))
                .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Codigo))
                .ForMember(d => d.BarcharCode, opt => opt.MapFrom(s => s.CodigoProducto))
                .ForMember(d => d.UPCCode, opt => opt.MapFrom(s => s.CodigoUpc))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.NombreProducto))
                .ForMember(d => d.MarkId, opt => opt.MapFrom(s => s.MarcaId))
                .ForMember(d => d.MarkName, opt => opt.MapFrom(s => s.Marca.Marca1))
                .ForMember(d => d.UnitsByPackage, opt => opt.MapFrom(s => s.UnidadesPorCaja));


            CreateMap<Detalletomafisica, ProductPhysicalDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.DetalleTomaFisicaId))
                .ForMember(d => d.InventoryTakingId, opt => opt.MapFrom(s => s.TomaFisicaId))
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductoId))
                .ForMember(d => d.BarcharCode, opt => opt.MapFrom(s => s.Producto.CodigoProducto))
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Producto.NombreProducto))
                .ForMember(d => d.SystemAvailable, opt => opt.MapFrom(s => s.DisponibleSistema))
                .ForMember(d => d.PhysicalQuantity, opt => opt.MapFrom(s => s.DisponibleFisico))
                .ForMember(d => d.Difference, opt => opt.MapFrom(s => s.Diferencia))
                .ForMember(d => d.Justified, opt => opt.MapFrom(s => s.Ajustado))
                .ForMember(d => d.Notes, opt => opt.MapFrom(s => s.Observaciones));


            CreateMap<Shared.Models.Tomafisica, InventoryTakingDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.TomaFisicaId))
                .ForMember(d => d.DateStart, opt => opt.MapFrom(s => s.Fecha))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Estado))
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UsuarioId))
                .ForMember(d => d.MarkId, opt => opt.MapFrom(s => s.MarcaId))
                .ForMember(d => d.MarkName, opt => opt.MapFrom(s => s.Marca.Marca1))
                .ForMember(d => d.Details, opt => opt.MapFrom(s => s.Detalletomafisica));


            CreateMap<Marca, MarkDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.MarcaId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Marca1));


            CreateMap<ProductPhysicalDto, Detalletomafisica>()
                .ForMember(d => d.TomaFisicaId, opt => opt.MapFrom(s => s.InventoryTakingId))
                .ForMember(d => d.ProductoId, opt => opt.MapFrom(s => s.ProductId))
                .ForMember(d => d.DisponibleSistema, opt => opt.MapFrom(s => s.SystemAvailable))
                .ForMember(d => d.DisponibleFisico, opt => opt.MapFrom(s => s.PhysicalQuantity))
                .ForMember(d => d.Diferencia, opt => opt.MapFrom(s => s.Difference))
                .ForMember(d => d.Observaciones, opt => opt.MapFrom(s => s.Notes));


            CreateMap<Detalledocumentocompra, ImportDetailDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.DetalleDocumentoCompraId))
                .ForMember(d => d.DocumentId, opt => opt.MapFrom(s => s.DocumentoCompraId))
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductoId))
                .ForMember(d => d.BarcharCode, opt => opt.MapFrom(s => s.Producto.CodigoProducto))
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Producto.NombreProducto))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Cantidad))
                .ForMember(d => d.VerifiedQuantity, opt => opt.MapFrom(s => s.CantidadVerificacion))
                .ForMember(d => d.TotalValue, opt => opt.MapFrom(s => s.ValorTotal));


            CreateMap<Documentocompra, ImportDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.DocumentoCompraId))
                .ForMember(d => d.DocumentTypeId, opt => opt.MapFrom(s => s.TipoDocumentoCompraId))
                .ForMember(d => d.DocumentTypeName, opt => opt.MapFrom(s => s.TipoDocumentoCompra.TipoDocumento1))
                .ForMember(d => d.DocumentNumber, opt => opt.MapFrom(s => s.NumeroDocumento))
                .ForMember(d => d.DocumentDate, opt => opt.MapFrom(s => s.Fecha))
                .ForMember(d => d.ProviderId, opt => opt.MapFrom(s => s.ProveedorId))
                .ForMember(d => d.ProviderName, opt => opt.MapFrom(s => s.Proveedor.NombreProveedor))
                .ForMember(d => d.TotalValue, opt => opt.MapFrom(s => s.ValorTotal))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.EstadoVerificacionId))
                .ForMember(d => d.Details, opt => opt.MapFrom(s => s.Detalledocumentocompra));


            CreateMap<sp_listado_pedidosResult, OrderRequestDto>()
                    .ForMember(d => d.Id, opt => opt.MapFrom(s => s.PedidoID))  //
                    .ForMember(d => d.OrderDate, opt => opt.MapFrom(s => s.Fecha))  //
                    .ForMember(d => d.OrderDateStr, opt => opt.MapFrom(s => s.Fecha_Str))        //
                    .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.Valor_Total))  //
                    .ForMember(d => d.IsQuoted, opt => opt.MapFrom(s => s.Es_Cotizacion))       //
                    .ForMember(d => d.Note, opt => opt.MapFrom(s => s.Observaciones))       //
                    .ForMember(d => d.AddressDelivery, opt => opt.MapFrom(s => s.DireccionEntrega))     //
                    .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.ClienteID)) //
                    .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Nombre_Cliente))      //
                    .ForMember(d => d.OrderStateId, opt => opt.MapFrom(s => s.Estado_PedidoID))  //
                    .ForMember(d => d.OrderStateName, opt => opt.MapFrom(s => s.Nombre_Estado_Pedido))      //
                    .ForMember(d => d.SellerId, opt => opt.MapFrom(s => s.VendedorID))      //
                    .ForMember(d => d.SellerName, opt => opt.MapFrom(s => s.NombreUsuario))     //
                    .ForMember(d => d.SellerEmail, opt => opt.MapFrom(s => s.Correo_Electronico_Vendedor));


            CreateMap<Pedido, OrderRequestDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.PedidoId))  //
                .ForMember(d => d.OrderDate, opt => opt.MapFrom(s => s.Fecha))  //
                                                                                //.ForMember(d => d.OrderDateStr, opt => opt.MapFrom(s=> s.Fecha_Str))        //
                .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.ValorTotal))  //
                .ForMember(d => d.IsQuoted, opt => opt.MapFrom(s => s.EsCotizacion))       //
                .ForMember(d => d.Note, opt => opt.MapFrom(s => s.Observaciones))       //
                .ForMember(d => d.AddressDelivery, opt => opt.MapFrom(s => s.DireccionEntrega))     //
                .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.ClienteId)) //
                                                                                    //.ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer))      //
                .ForMember(d => d.OrderStateId, opt => opt.MapFrom(s => s.EstadoPedidoId))  //
                                                                                            //.ForMember(d => d.OrderStateName, opt => opt.MapFrom(s => s.Nombre_Estado_Pedido))      //
                .ForMember(d => d.SellerId, opt => opt.MapFrom(s => s.VendedorId))      //
                                                                                        //.ForMember(d => d.SellerName, opt => opt.MapFrom(s => s.NombreUsuario))     //
                                                                                        //.ForMember(d => d.SellerEmail, opt => opt.MapFrom(s => s.Correo_Electronico_Vendedor))
                .ForMember(d => d.Details, opt => opt.MapFrom(s => s.DetallePedido));     //


            CreateMap<DetallePedido, OrderRequestDetailDto>()
                .ForMember(d => d.DetailId, opt => opt.MapFrom(s => s.DetallePedidoId))
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductoId))
                //.ForMember(d => d.ProductCode, opt => opt.MapFrom(s => s.Product))
                //.ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.PedidoID))
                //.ForMember(d => d.QuantityAvailable, opt => opt.MapFrom(s => s.PedidoID))
                //.ForMember(d => d.BrandName, opt => opt.MapFrom(s => s.PedidoID))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Cantidad))
                .ForMember(d => d.UnitPrize, opt => opt.MapFrom(s => s.ValorUnitario))
                .ForMember(d => d.Total, opt => opt.MapFrom(s => s.ValorTotal));


        }

    }
}
