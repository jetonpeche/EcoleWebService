using System;
using System.Collections.Generic;

namespace back.Models;

public partial class Typerestaurant
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
