using BlessWebPedidoSidi.Application.ControleSistemaPedido;
using MediatR;

namespace BlessWebPedidoSidi.Application.Shared.GeraCaminhoImagem;

public class GeraCaminhoImagemHandler(IMediator mediator) : IRequestHandler<GeraCaminhoImagemCommand, string>
{
    public async Task<string> Handle(GeraCaminhoImagemCommand command, CancellationToken cancellationToken)
    {
        var pesquisaControleSistemaPedidoQuery = new ControleSistemaPedidoQuery();
        var controleSistemaPedido = await mediator.Send(pesquisaControleSistemaPedidoQuery, cancellationToken);

        return $"https://app-sidimobile.s3.sa-east-1.amazonaws.com/img-colecoes/{controleSistemaPedido.PastaAwsS3}/{command.ModeloCodigo}-{command.CorCodigo}/1.jpg";
    }
}

public record GeraCaminhoImagemCommand : IRequest<string>
{
    public required int ModeloCodigo { get; set; }
    public required int CorCodigo { get; set; }
}
