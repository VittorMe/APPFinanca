namespace APPFinanca.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Password { get; set; }

        // Lista de transações
        public List<Guid> Transactions { get; set; } = new List<Guid>();
    }
}
