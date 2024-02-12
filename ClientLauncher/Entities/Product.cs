using System;
using System.Collections.Generic;

namespace ClientLauncher.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int UnitOfMeasurementId { get; set; }

    public decimal Price { get; set; }

    public int VatId { get; set; }

    public virtual ICollection<RefundProduct> RefundProducts { get; set; } = new List<RefundProduct>();

    public virtual UnitOfMeasurement UnitOfMeasurement { get; set; } = null!;

    public virtual Vat Vat { get; set; } = null!;
}
