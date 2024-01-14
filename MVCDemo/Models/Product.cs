namespace MVCDemo.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public string? Picture { get; set; }
        public List<OrderDetail> Details { get; set; }

    }
}
