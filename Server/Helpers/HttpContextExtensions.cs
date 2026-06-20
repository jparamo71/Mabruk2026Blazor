using Microsoft.EntityFrameworkCore;

namespace MabrukBlazor2026.Server.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task HeaderResponseParameter<T>(
            this HttpContext context, IQueryable<T> queryable, int countRowsToShow)
        {
            if (context is null) { throw new ArgumentNullException(nameof(context)); }

            double totalRows = await queryable.CountAsync();
            double totalPages = Math.Ceiling(totalRows / countRowsToShow);
            context.Response.Headers.Add("totalRows", totalRows.ToString());
            context.Response.Headers.Add("totalPages", totalPages.ToString());
        }
    }
}
