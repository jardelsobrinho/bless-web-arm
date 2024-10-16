namespace BlessWebPedidoSidi.Application.Shared;

public interface ICriptografia
{
    string Encrypt(string valor);
    string Decrypt(string valor);
}
