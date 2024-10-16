using BlessWebPedidoSidi.Domain.OrcamentoWeb.ValueObjects;
using BlessWebPedidoSidi.Domain.Shared;

namespace BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities
{
    public class OrcamentoWebEntity : IEntity
    {
        public required int Id { get; set; }
        public required DateTime DataCriacao { get; set; }
        public required double ValorTotal { get; set; }
        public required int UsuarioCodigo { get; set; }
        public required string RepresentanteCnpj { get; set; }
        public required string EmpresaCnpj { get; set; }
        public required string Uuid { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataEntrega { get; set; }
        public string? ClienteCnpjCpf { get; set; }
        public int? TabelaPrecoCodigo { get; set; }
        public int? CondicaoPagamentoCodigo { get; set; }
        public string? TipoEstoqueCodigo { get; set; }
        public string? Frete { get; set; }
        public string? ClienteNome { get; set; }
        public string? Obs { get; set; }
        public required EOrcamentoStatus Status { get; set; }
        public IList<OrcamentoWebItemEntity> Itens { get; set; } = new List<OrcamentoWebItemEntity>();

        public void ValidaDados()
        {
            if (ClienteCnpjCpf == null || ClienteCnpjCpf == "")
                throw new EntidadeInvalidaException("OWE01 - Selecione um cliente antes de finalizar o orçamento");

            if (CondicaoPagamentoCodigo == null || CondicaoPagamentoCodigo == 0)
                throw new EntidadeInvalidaException("OWE02 - Selecione uma condição de pagamento antes de finalizar o orçamento");

            if (TabelaPrecoCodigo == null || TabelaPrecoCodigo == 0)
                throw new EntidadeInvalidaException("OWE03 - Selecione uma tabela de preço antes de finalizar o orçamento");

            if (Frete == null || Frete == "")
                throw new EntidadeInvalidaException("OWE04 - Selecione um frete antes de finalizar o orçamento");

            if (TipoEstoqueCodigo == null || TipoEstoqueCodigo == "")
                throw new EntidadeInvalidaException("OWE05 - Selecione um tipo de estoque antes de finalizar o orçamento");

            if (DataEntrega == null)
                throw new EntidadeInvalidaException("OWE09 - O campo Data de Entrega deve ser preenchido");

            var DataBase = DataEmissao ?? throw new EntidadeInvalidaException("OWE11 - Conversão inválida para Data de Emissão");
            if (DataEntrega < DataBase.Date)
                throw new EntidadeInvalidaException("OWE10 - O campo Data de Entrega deve ser maior que a Data de Emissão");

            if (Itens.Count == 0)
                throw new EntidadeInvalidaException("OWE06 - Adicione um modelo e cor antes de finalizar o orçamento");

            var itensSemPreco = Itens.Count(x => x.PrecoUnitario <= 0);
            if (itensSemPreco > 0)
                throw new EntidadeInvalidaException("OWE07 - Não é possível continuar porque existem itens sem preço");

            if (ValorTotal == 0)
                throw new EntidadeInvalidaException("OWE08 - O campo valor total deve ser maior que zero");
        }
    }
}