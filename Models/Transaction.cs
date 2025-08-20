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
        public string? Description { get; set; } // Nova propriedade para descrição
        public string? Location { get; set; } // Nova propriedade para local/estabelecimento
        public required DateTimeOffset Date { get; set; }
        public required decimal Value { get; set; }
        public bool IsRecurring { get; set; } = false; // Nova propriedade para recorrência
        public RecurrenceType? RecurrenceType { get; set; } // Tipo de recorrência
        public string? AttachmentPath { get; set; } // Caminho para anexos/recibos
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now; // Data de criação
        public DateTimeOffset? UpdatedAt { get; set; } // Data de atualização

        // Relacionamento com o usuário
        public Guid UserId { get; set; }
    }
}
