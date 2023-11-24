using ApiTPwebSocket.ModelsBdd;

namespace ApiTPwebSocket.Services;

public sealed class MessageService
{
    public List<string> Lister() => Bdd.Instance.ListeMessage;
    public void Ajouter(string _msg) => Bdd.Instance.ListeMessage.Add(_msg);
}
