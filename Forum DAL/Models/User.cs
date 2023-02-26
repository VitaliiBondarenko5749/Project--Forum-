namespace Authorization_DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? LoginU { get; set; }
        public string? PasswordU { get; set; }
        public string? StatusU { get; set; }
        public string? Email { get; set; }
        public string? NameU { get; set; }
        public string? SurnameU { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
    }
}
