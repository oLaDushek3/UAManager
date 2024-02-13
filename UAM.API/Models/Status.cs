using System.Text.Json.Serialization;

namespace UAM.API.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Problem> Problems { get; set; } = new List<Problem>();
}
