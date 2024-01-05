namespace TodoApp.Models
{
    public class User
    {
        public int UserID { get; set; } 
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool isAdmin { get; set; }


    }
}
