namespace back.ModelsImport.Personnes
{
    public sealed class PersonneImport
    {
        public required string Nom { get; set; }
        public required string Login { get; set; }
        public required string Mdp { get; set; }
        public required List<int> ListeIdRestaurant { get; set; }
    }
}
