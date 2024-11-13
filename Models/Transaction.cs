using static APPFinanca.Models.Enumation;

namespace APPFinanca.Models
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required TransactionType TransactionType { get; set; }
        public required PaymentType PaymentType { get; set; }
        public required GroupType Group { get; set; }
        public required string Name { get; set; }
        public required DateTimeOffset Date { get; set; }
        public required decimal Value { get; set; }

        // Relacionamento com o usuário
        public Guid UserId { get; set; }
    }
}
