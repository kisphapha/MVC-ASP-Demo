using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDemo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        [Column(TypeName = "varchar(12)")]
        public string phoneNumber { get; set; }
        public List<Order> orders {get ; set;}
        public override string ToString()
        {
            return Id + "," + Name + "," + Email + "," + phoneNumber + "," + Role;
        }
        public static User? parseUser(string user)
        {
            try
            {
                string[] props = user.Split(',');
                User userObj = new User()
                {
                    Id = int.Parse(props[0]),
                    Name = props[1],
                    Email = props[2],
                    phoneNumber = props[3],
                    Role = int.Parse(props[4]),
                };
                return userObj;
            } catch (Exception)
            {
                return null;
            }
        }
    }
}
