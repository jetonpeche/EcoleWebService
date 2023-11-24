using ApiTPwebSocket.ModelsBdd;

namespace ApiTPwebSocket.Services;

public sealed class JoueurService
{
    public void Ajouter(string _pseudo) => Bdd.Instance.ListeJoueur.Add(new Joueur { Pseudo = _pseudo });
    public bool Existe(string _pseudo) => Bdd.Instance.ListeJoueur.Any(x => x.Pseudo == _pseudo);
}
