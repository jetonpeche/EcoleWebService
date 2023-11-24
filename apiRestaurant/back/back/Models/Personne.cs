using System;
using System.Collections.Generic;

namespace back.Models;

public partial class Personne
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Mdp { get; set; } = null!;

    public bool EstAmin { get; set; }

    public virtual ICollection<Personnerestauranthistorique> Personnerestauranthistoriques { get; set; } = new List<Personnerestauranthistorique>();

    public virtual ICollection<Restaurant> IdRestaurants { get; set; } = new List<Restaurant>();
}
