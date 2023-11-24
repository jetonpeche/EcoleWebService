namespace back.ModelsImport.Restaurants;

public sealed class RestaurantImport
{
    public required string Nom { get; set; }
    public required string Adresse { get; set; }
    public required string? Description { get; set; }
    public required string? Url { get; set; }
    public required string? Telephone { get; set; }
    public required int IdTypeRestaurant { get; set; }
}
