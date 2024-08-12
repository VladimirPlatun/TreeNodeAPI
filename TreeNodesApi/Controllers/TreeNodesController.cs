using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeNodesApi.Data;
using TreeNodesApi.Models;

namespace TreeNodesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreeNodesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreeNodesController(AppDbContext context)
        {
            _context = context;
        }

         
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreeNode>>> GetAllNodes()
        {
            return await _context.TreeNodes
                .Include(t => t.Children)  
                .ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TreeNodeDto>> GetNodeById(int id)
        {
            var treeNode = await _context.TreeNodes
                .Include(t => t.Children)  
                .FirstOrDefaultAsync(t => t.Id == id);

            if (treeNode == null)
            {
                return NotFound();
            }

             
            var treeNodeDto = new TreeNodeDto
            {
                Id = treeNode.Id,
                Name = treeNode.Name,
                ParentId = treeNode.ParentId,
                TreeId = treeNode.TreeId,
                Children = treeNode.Children.Select(child => new TreeNodeDto
                {
                    Id = child.Id,
                    Name = child.Name,
                    ParentId = child.ParentId,
                    TreeId = child.TreeId,
                    Children = new List<TreeNodeDto>()  
                }).ToList()
            };

            return treeNodeDto;
        }

        [HttpPost]
        public async Task<ActionResult<TreeNode>> CreateNode(TreeNode treeNode)
        {
            _context.TreeNodes.Add(treeNode);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNodeById), new { id = treeNode.Id }, treeNode);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(int id)
        {
            var node = await _context.TreeNodes
                .Include(n => n.Children)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (node == null)
            {
                return NotFound();
            }

            if (node.Children.Any())
            {
                throw new SecureException("You have to delete all children nodes first");
            }

            _context.TreeNodes.Remove(node);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class SecureException : Exception
    {
        public SecureException(string message) : base(message) { }
    }
}
