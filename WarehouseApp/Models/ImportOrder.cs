using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class ImportOrder
{
    public int ImportId { get; set; }

    public DateTime ImportDate { get; set; }

    public int? UserId { get; set; }

    public int? SupplierId { get; set; }

    public int? WarehouseId { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<ImportOrderDetail> ImportOrderDetails { get; set; } = new List<ImportOrderDetail>();

    public virtual Supplier? Supplier { get; set; }

    public virtual User? User { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
