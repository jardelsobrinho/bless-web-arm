namespace BlessWebPedidoSidi.Application.ViaCEP;

public record RetornaDadosViaCEPModel
{
    public string Cep { get; init; } = "";
    public string Logradouro { get; init; } = "";
    public string Complemento { get; init; } = "";
    public string Bairro { get; init; } = "";
    public string Localidade { get; init; } = "";
    public string Uf { get; init; } = "";
    public int Ibge { get; init; } = 0;
    public int CidadeCodigo { get; init; } = 0;
    public bool Erro { get; init; } = false;
}