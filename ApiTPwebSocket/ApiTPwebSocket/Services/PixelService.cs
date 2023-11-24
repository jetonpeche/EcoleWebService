using ApiTPwebSocket.ModelsBdd;

namespace ApiTPwebSocket.Services;

public sealed class PixelService
{
    public bool Existe(string _id) => Bdd.Instance.ListePixel.Any(x => x.Id == _id);
    public List<Pixel> Lister() => Bdd.Instance.ListePixel;
    public void Ajouter(Pixel _pixel) => Bdd.Instance.ListePixel.Add(_pixel);

    public void Modifier(Pixel _pixel)
    {
        if(Existe(_pixel.Id))
            Bdd.Instance.ListePixel.First(x => x.Id == _pixel.Id).Couleur = _pixel.Couleur;
    }

    public void Supprimer(Pixel _pixel) => Bdd.Instance.ListePixel.Remove(_pixel);
}
