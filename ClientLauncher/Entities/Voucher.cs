namespace ClientLauncher.Entities;

public partial class Voucher
{
    public int Id { get; set; }

    public int Number { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
