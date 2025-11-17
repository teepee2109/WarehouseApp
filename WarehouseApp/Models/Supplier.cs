using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();
}
