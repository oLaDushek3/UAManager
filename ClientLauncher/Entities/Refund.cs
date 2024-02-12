using System;
using System.Collections.Generic;

namespace ClientLauncher.Entities;

public partial class Refund
{
    public int Id { get; set; }

    public string Customer { get; set; } = null!;

    public int EmployeeId { get; set; }

    public DateTime Date { get; set; }

    public int VoucherId { get; set; }

    public DateTime AnnexDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<RefundProduct> RefundProducts { get; set; } = new List<RefundProduct>();

    public virtual Voucher Voucher { get; set; } = null!;
}
