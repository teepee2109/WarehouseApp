using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class ExportOrder
{
    public int ExportId { get; set; }

    public DateTime ExportDate { get; set; }

    public int? UserId { get; set; }

    public int? WarehouseId { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<ExportOrderDetail> ExportOrderDetails { get; set; } = new List<ExportOrderDetail>();

    public virtual User? User { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
