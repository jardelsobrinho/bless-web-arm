namespace BlessWebPedidoSidi.Application.Usuario;

public record UsuarioModel
{
    public required string ExibirTodosClientesPedidoSidi { get; init; } = "";
    public required string ExibirTodasCondPagtos { get; init; } = "";
    public required string ExibirMarcaVendasWeb { get; init; } = "N";
    public required string ExibirTodosPedidosSidiWeb { get; init; } = "N";
    public required string ExibirSoladoPalmilhaSidiWeb { get; init; } = "N";
    public required string ExibirObsItemSidiWeb { get; init; } = "N";

    public int Fabrica;
}