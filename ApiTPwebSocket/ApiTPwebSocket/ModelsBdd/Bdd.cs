namespace ApiTPwebSocket.ModelsBdd;

public sealed class Bdd
{
    private static object threadSafe = new object();
    private static Bdd? instance = null;

    public static Bdd Instance 
    {
        get
        {
            if (instance is null)
            {
                lock (threadSafe)
                {
                    if(instance is null)
                        instance = new();
                }
            }
                
            return instance;
        }
    }

    public List<Joueur> ListeJoueur { get; set; } = new();
    public List<Pixel> ListePixel { get; set; } = new();
    public List<string> ListeMessage { get; set; } = new();

    private Bdd()
    {
        
    }

}

public sealed record Joueur
{
    public string Pseudo { get; init; } = null!;
}

public sealed class Pixel
{
    public string Id { get; init; }

    public int PosX { get; init; }
    public int PosY { get; init; }

    public string Couleur { get; set; }
}
