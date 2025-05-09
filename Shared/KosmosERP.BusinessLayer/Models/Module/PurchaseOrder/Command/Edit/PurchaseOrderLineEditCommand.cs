﻿using Microsoft.EntityFrameworkCore;
using KosmosERP.Models;
using System.ComponentModel.DataAnnotations;

namespace KosmosERP.BusinessLayer.Models.Module.PurchaseOrder.Command.Edit;

public class PurchaseOrderLineEditCommand : DataCommand
{
    public int? id { get; set; }

    public int? product_id { get; set; }
    public int line_number { get; set; }
    public int? quantity { get; set; }
    [MaxLength(1000)]
    public string? description { get; set; }
    [Precision(14, 3)]
    public decimal? unit_price { get; set; }
    [Precision(14, 3)]
    public decimal? tax { get; set; } 
    public bool? is_taxable { get; set; }
}
