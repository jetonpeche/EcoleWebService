using System;
using System.Collections.Generic;

namespace back.Models;

public partial class Personnerestauranthistorique
{
    public int Id { get; set; }

    public int IdPersonne { get; set; }

    public int IdRestaurant { get; set; }

    public DateOnly Date { get; set; }

    public decimal Prix { get; set; }

    public virtual Personne IdPersonneNavigation { get; set; } = null!;

    public virtual Restaurant IdRestaurantNavigation { get; set; } = null!;
}
