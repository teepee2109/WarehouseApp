using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class ExportOrderDetail
{
    public int ExportDetailId { get; set; }

    public int ExportId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual ExportOrder? Export { get; set; }

    public virtual Product? Product { get; set; }
}
