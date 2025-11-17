using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class ImportOrderDetail
{
    public int ImportDetailId { get; set; }

    public int ImportId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual ImportOrder? Import { get; set; }

    public virtual Product? Product { get; set; }
}
