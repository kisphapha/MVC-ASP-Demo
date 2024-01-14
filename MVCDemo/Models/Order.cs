namespace MVCDemo.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Total {  get; set; }

        public DateTime CreatedDate { get; set; }

        public string Status { get; set; }

        public User User { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}
