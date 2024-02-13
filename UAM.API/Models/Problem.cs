using System;
using System.Collections.Generic;

namespace UAM.API.Models;

public partial class Problem
{
    public int Id { get; set; }

    public string ProblemText { get; set; } = null!;

    public int? PriorityId { get; set; }

    public int StatusId { get; set; }

    public int? WorkerId { get; set; }

    public DateTime CreatedTime { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string Email { get; set; } = null!;

    public string? Solution { get; set; }

    public string? Version { get; set; }

    public virtual Priority? Priority { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Worker? Worker { get; set; }
}
