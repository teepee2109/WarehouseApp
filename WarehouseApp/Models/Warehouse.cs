using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<ExportOrder> ExportOrders { get; set; } = new List<ExportOrder>();

    public virtual ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();
}
