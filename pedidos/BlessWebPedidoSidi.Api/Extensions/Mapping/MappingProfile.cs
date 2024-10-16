using AutoMapper;
using BlessWebPedidoSidi.Api.Models.OrcamentoWeb.AdicionaModelo;
using BlessWebPedidoSidi.Application.OrcamentosWeb.AdicionaItens.Models;
using BlessWebPedidoSidi.Application.OrcamentosWeb.RetornaOrcamento.Models;
using BlessWebPedidoSidi.Domain.OrcamentoWeb.Entities;

namespace BlessWebPedidoSidi.Api.Extensions.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<OrcamentoModel, OrcamentoWebEntity>().ReverseMap();
        CreateMap<OrcamentoItemModel, OrcamentoWebItemEntity>().ReverseMap();
        CreateMap<OrcamentoItemGradeModel, OrcamentoWebItemGradeEntity>().ReverseMap();

        CreateMap<CorModel, CorRequest>().ReverseMap();
        CreateMap<TamanhoModel, TamanhoRequest>().ReverseMap();
    }
}
