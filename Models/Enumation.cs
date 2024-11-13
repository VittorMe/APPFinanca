namespace APPFinanca.Models
{
    public class Enumation
    {
        public enum TransactionType
        {
            Income,
            Expenses
        }
        public enum PaymentType
        {
            Selecione = 0,           // Valor de placeholder
            Dinheiro = 1,
            CartaoCredito = 2,
            CartaoDebito = 3,
            Pix = 4,
            TransferenciaBancaria = 5,
            Boleto = 6,
            Cheque = 7,
            ValeRefeicao = 8,
            ValeAlimentacao = 9,
            CreditoLoja = 10
        }


        public enum GroupType
        {
            Selecione = 0,           // Valor de placeholder
            Lazer = 1,
            Alimentacao = 2,
            Transporte = 3,
            Moradia = 4,
            Educacao = 5,
            Saude = 6,
            Compras = 7,
            Investimentos = 8,
            Poupanca = 9,
            Dividas = 10,
            Viagens = 11,
            PresentesDoacoes = 12,
            Servicos = 13,
            ImpostosTaxas = 14,
            CasaDecoracao = 15,
            AnimaisEstimacao = 16,
            Tecnologia = 17,
            Outros = 18
        }
        public enum Month
        {
            Janeiro = 1,
            Fevereiro = 2,
            Março = 3,
            Abril = 4,
            Maio = 5,
            Junho = 6,
            Julho = 7,
            Agosto = 8,
            Setembro = 9,
            Outubro = 10,
            Novembro = 11,
            Dezembro = 12
        }

    }
}
