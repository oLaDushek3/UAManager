using System;
using System.Collections.Generic;

namespace ClientLauncher.Entities;

public partial class Vat
{
    public int Id { get; set; }

    public int Value { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
