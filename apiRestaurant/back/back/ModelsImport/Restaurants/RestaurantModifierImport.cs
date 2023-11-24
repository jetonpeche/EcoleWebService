namespace back.ModelsImport.Restaurants;

public sealed class RestaurantModifierImport
{
    public required int Id { get; init; }
    public required string Nom { get; set; } = null!;
    public required string Adresse { get; set; } = null!;
    public required string? Description { get; set; }
    public required string? Telephone { get; set; }
    public required string? Url { get; set; }
    public required int IdTypeRestaurant { get; init; }
}
