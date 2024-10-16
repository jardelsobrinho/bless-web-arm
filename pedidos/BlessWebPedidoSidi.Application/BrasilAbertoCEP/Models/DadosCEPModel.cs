namespace BlessWebPedidoSidi.Application.BrasilAbertoCEP.Models;

public record DadosCEPModel
{
    public string Street { get; init; } = "";
    public string Complement { get; init; } = "";
    public string District { get; init; } = "";
    public int DistrictId { get; init; } = 0;
    public string City { get; init; } = "";
    public int CityId { get; init; } = 0;
    public int IbgeId { get; init; } = 0;
    public string State { get; init; } = "";
    public string StateShortname { get; init; } = "";
    public string Zipcode { get; init; } = "";
}
