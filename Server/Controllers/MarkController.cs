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
    public class MarkController : ControllerBase
    {
        private readonly MabrukContext context;
        private readonly IMapper mapper;

        public MarkController(
            MabrukContext context,
            IMapper mapper
            )
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarkDto>>> Get()
        {
            var marksDb = await context.Marca
                .OrderBy(x => x.Marca1)
                .ToListAsync();

            if (marksDb.Any())
            {
                var marks = mapper.Map<IEnumerable<MarkDto>>(marksDb);
                return Ok(marks);
            }

            return NotFound();
        }


    }
}
