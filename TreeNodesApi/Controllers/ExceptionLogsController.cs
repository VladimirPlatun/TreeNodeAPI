using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeNodesApi.Data;
using TreeNodesApi.Models;

namespace TreeNodesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExceptionLogsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExceptionLog>>> GetExceptionLogs()
        {
            return await _context.ExceptionLogs.ToListAsync();
        }

 
        [HttpGet("{id}")]
        public async Task<ActionResult<ExceptionLog>> GetExceptionLog(int id)
        {
            var exceptionLog = await _context.ExceptionLogs.FindAsync(id);

            if (exceptionLog == null)
            {
                return NotFound();
            }

            return Ok(exceptionLog);
        }
    }
}
