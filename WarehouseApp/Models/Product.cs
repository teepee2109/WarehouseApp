using System;
using System.Collections.Generic;

namespace WarehouseApp.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public virtual Category? Category { get; set; }

    public int Quantity { get; set; }


    public virtual ICollection<ExportOrderDetail> ExportOrderDetails { get; set; } = new List<ExportOrderDetail>();

    public virtual ICollection<ImportOrderDetail> ImportOrderDetails { get; set; } = new List<ImportOrderDetail>();
}
