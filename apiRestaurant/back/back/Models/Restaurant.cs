using System;
using System.Collections.Generic;

namespace back.Models;

public partial class Restaurant
{
    public int Id { get; set; }

    public int IdTypeRestaurant { get; set; }

    public string Nom { get; set; } = null!;

    public string Adresse { get; set; } = null!;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string? Telephone { get; set; }

    public virtual Typerestaurant IdTypeRestaurantNavigation { get; set; } = null!;

    public virtual ICollection<Personnerestauranthistorique> Personnerestauranthistoriques { get; set; } = new List<Personnerestauranthistorique>();

    public virtual ICollection<Personne> IdPersonnes { get; set; } = new List<Personne>();
}
