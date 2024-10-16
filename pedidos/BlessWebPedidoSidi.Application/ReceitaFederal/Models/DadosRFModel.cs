using System.Text.Json.Serialization;

namespace BlessWebPedidoSidi.Application.ReceitaFederal.Models;

public record DadosRFModel
{
    public string Abertura { get; init; } = "";
    public string Situacao { get; init; } = "";
    public string Tipo { get; init; } = "";
    public string Nome { get; init; } = "";
    public string Fantasia { get; init; } = "";
    public string Porte { get; init; } = "";

    [JsonPropertyName("natureza_juridica")]
    public string NaturezaJuridica { get; init; } = "";
    public string Logradouro { get; init; } = "";
    public string Numero { get; init; } = "";
    public string Complemento { get; init; } = "";
    public string Municipio { get; init; } = "";
    public string Bairro { get; init; } = "";
    public string Uf { get; init; } = "";
    public string Cep { get; init; } = "";
    public string Email { get; init; } = "";
    public string Telefone { get; init; } = "";

    [JsonPropertyName("municipio_codigo")]
    public int MunicipioCodigo { get; init; } = 0;

    [JsonPropertyName("data_situacao")]
    public string DataSituacao { get; init; } = "";
    public string Cnpj { get; init; } = "";

    [JsonPropertyName("ultima_atualizacao")]
    public string UltimaAtualizacao { get; init; } = "";
    public string Status { get; init; } = "";
    public string Efr { get; init; } = "";

    [JsonPropertyName("motivo_situacao")]
    public string MotivoSituacao { get; init; } = "";

    [JsonPropertyName("situacao_especial")]
    public string SituacaoEspecial { get; init; } = "";

    [JsonPropertyName("data_situacao_especial")]
    public string DataSituacaoEspecial { get; init; } = "";

    [JsonPropertyName("capital_social")]
    public string CapitalSocial { get; init; } = "";

    [JsonPropertyName("atividade_principal")]
    public IList<AtividadeRFModel> AtividadePrincipal { get; init; } = new List<AtividadeRFModel>();

    [JsonPropertyName("atividades_secundarias")]
    public IList<AtividadeRFModel> AtividadesSecundarias { get; init; } = new List<AtividadeRFModel>();
}

