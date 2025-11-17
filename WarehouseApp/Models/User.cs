using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<ExportOrder> ExportOrders { get; set; } = new List<ExportOrder>();

    public virtual ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();

    public virtual Role? Role { get; set; }
}
