using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeNodesApi.Models
{
    public class TreeNode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [JsonIgnore]  
        [ForeignKey("ParentId")]
        public TreeNode? Parent { get; set; }

        [JsonProperty("children")]
        public ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();

        [Required]
        public int TreeId { get; set; }
    }
}
