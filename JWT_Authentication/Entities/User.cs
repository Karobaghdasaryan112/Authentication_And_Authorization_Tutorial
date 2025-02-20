namespace JWT_Authentication.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }

        public IEnumerable<Role> Roles { get; set; } = null!;
    }
}
