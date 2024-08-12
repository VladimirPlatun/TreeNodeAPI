namespace TreeNodesApi.Data
{
    public class TreeNodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();
        public int TreeId { get; set; }
    }

}
