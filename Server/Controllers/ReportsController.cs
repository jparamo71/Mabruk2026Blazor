using MabrukBlazor2026.Server.Helpers;
using MabrukBlazor2026.Shared.Models;
using MabrukBlazor2026.Shared.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using System;
using System.Data;
using System.Reflection.Metadata;
using System.Text;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly MabrukContext context;
        private readonly IWebHostEnvironment environment;

        public ReportsController(
            MabrukContext context,
            IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }



        [HttpGet("products")]
        public async Task<ActionResult> GetProducts()
        {

            FormattableString query = $@"SELECT p.ProductoId as ProductId, 
                        p.CodigoProducto BarcharCode,  
                        p.CodigoUPC UPCCode,
                        p.NombreProducto Name,
                        ISNULL(p.MarcaId, 0) MarkId,
                        m.Marca MarkName,
                        ISNULL(
                                (
                                    SELECT  SUM(InventarioDisponible)
                                    FROM    Inventario
                                    WHERE   ProductoId = p.ProductoId
                                ), 0
                        ) as Available
                FROM    Producto p
                            LEFT JOIN
                        Marca m
                            ON p.MarcaId = m.MarcaId
                WHERE   ISNULL(p.Activo, 0) <> 0";

            var products = await context.ViewProducts.FromSql(query).ToListAsync();
            var dt = EntityToDatatable.CreateDataTable<ProductItemList>(products);


            //string mimetype = "";
            //int extension = 1;
            //string encoding = "";

            //ReportViewer rptViewer1 = new ReportViewer();
            //rptViewer1.ProcessingMode = ProcessingMode.Local;

            var path = $"{environment.WebRootPath}\\Reports\\ReportProductList.rdlc";

            ////Dictionary<string, string> parameters = new Dictionary<string, string>();
            ////parameters.Add("ReportParameter1", "RDLC in Blazor Web Application.");

            //rptViewer1.LocalReport.ReportPath = path;
            //ReportDataSource dataSource = new ReportDataSource() { Name = "dsProducts", Value = dt };
            //rptViewer1.LocalReport.DataSources.Add(dataSource);

            //byte[] buffer = rptViewer1.LocalReport.Render(format: "PDF", deviceInfo: ""); // out mimetype, out encoding, out extension);
            //return File(buffer, "application/pdf");

            //string mimetype = "";
            //int extension = 1;
            //var path = $"{environment.WebRootPath}\\Reports\\Report1.rdlc";
            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("ReportParameter1", "RDLC in Blazor Web Application.");
            //LocalReport localReport = new LocalReport(path);
            //var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
            //return File(result.MainStream, "application/pdf");


            LocalReport report = new LocalReport();
            report.ReportPath = path;
            //report.LoadReportDefinition(path);
            ReportDataSource dataSource = new ReportDataSource() { Name = "dsProducts", Value = dt };
            report.DataSources.Add(dataSource); // new ReportDataSource("source", dt));
            //report.SetParameters(new[] { new ReportParameter("Parameter1", "Parameter value") });
            byte[] pdf = report.Render("PDF");
            return File(pdf, "application/pdf");

        }




        [HttpGet("takeinventory/{id}")]
        public async Task<ActionResult> GetTakingInventory(int id)
        {

            FormattableString query = $@"SELECT	tf.Fecha DateStart,
		            tf.Estado Status,
		            m.Marca MarkName,
		            ISNULL(p.CodigoProducto, '') BarcharCode,
		            p.NombreProducto ProductName,
		            ISNULL(dtf.DisponibleSistema, 0) SystemAvailable,
		            ISNULL(dtf.DisponibleFisico, 0) PhysicalQuantity,
		            ISNULL(dtf.Diferencia, 0) [Difference],
		            ISNULL(dtf.Observaciones, '') Notes
            FROM	tomafisica tf
			            INNER JOIN
		            marca m
			            ON tf.MarcaId = m.MarcaId
			            LEFT JOIN 
		            (
			            detalletomafisica dtf
				            INNER JOIN 
			            producto p
				            ON dtf.ProductoId = p.ProductoId 
		            )
			            ON tf.TomaFisicaId = dtf.TomaFisicaId
            WHERE	tf.TomaFisicaId = {id}";

            var takingInventory = await context.ViewTakeInventory.FromSql(query).ToListAsync();
            var dt = EntityToDatatable.CreateDataTable<TakingInventoryItemList>(takingInventory);

            var path = $"{environment.WebRootPath}\\Reports\\ReportTakeInventory.rdlc";

            LocalReport report = new LocalReport();
            report.ReportPath = path;
            ReportDataSource dataSource = new ReportDataSource() { Name = "dsTakingInventory", Value = dt };
            report.DataSources.Add(dataSource);
            byte[] pdf = report.Render("PDF");
            return File(pdf, "application/pdf");

        }




        [HttpGet("verificationimport/{id}")]
        public async Task<ActionResult> GetVerificationImport(int id)
        {

            FormattableString query = $@"SELECT	[dc].[TipoDocumentoCompraId] [DocumentTypeId],
		            [dc].[NumeroDocumento] [DocumentNumber],
		            [dc].[Fecha] [DateStart],
		            [dc].[EstadoDocumentoId] [Status],
		            [pr].[NombreProveedor] [ProviderName],
		            ISNULL([p].[CodigoProducto], '') [BarcharCode],
		            [p].[NombreProducto] [ProductName],
		            [ddc].[Cantidad] [SystemAvailable],
		            [ddc].[CantidadVerificacion] [PhysicalQuantity],
		            ([ddc].[Cantidad] - [ddc].[CantidadVerificacion]) [Difference]
            FROM	documentocompra dc
			            INNER JOIN
		            proveedor pr
			            ON dc.ProveedorId = pr.ProveedorId
			            LEFT JOIN 
		            (
			            detalledocumentocompra ddc
				            INNER JOIN 
			            producto p
				            ON ddc.ProductoId = p.ProductoId 
		            )
			            ON dc.DocumentoCompraId = ddc.DocumentoCompraId
            WHERE	dc.DocumentoCompraId = {id}";

            var verificationImport = await context.ViewVerificationImport.FromSql(query).ToListAsync();
            var dt = EntityToDatatable.CreateDataTable<VerificationImport>(verificationImport);

            var path = $"{environment.WebRootPath}\\Reports\\ReportVerificationImport.rdlc";

            LocalReport report = new LocalReport();
            report.ReportPath = path;
            ReportDataSource dataSource = new ReportDataSource() { Name = "dsVerificationImport", Value = dt };
            report.DataSources.Add(dataSource);
            byte[] pdf = report.Render("PDF");
            return File(pdf, "application/pdf");

        }








    }
}
