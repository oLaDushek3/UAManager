using System;
using System.Collections.Generic;

namespace ClientLauncher.Entities;

public partial class RefundProduct
{
    public int Id { get; set; }

    public int RefundId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal Amount { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Refund Refund { get; set; } = null!;
}
