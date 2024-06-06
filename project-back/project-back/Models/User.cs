namespace project_back.Models
{
    public class User
    {
        public User(Guid id, string name, string password, string role)
        {
            this.Id = id;
            this.Name = name;
            this.Password = password;
            Role = role;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }    
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
