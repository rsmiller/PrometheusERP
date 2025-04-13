﻿using Prometheus.Models;
using System.ComponentModel.DataAnnotations;

namespace Prometheus.BusinessLayer.Models.Module.ARInvoice.Command.Edit;

public class ARInvoiceHeaderEditCommand : DataCommand
{
    [Required]
    public int id { get; set; }
    public int? line_number { get; set; }
    public DateOnly? invoice_date { get; set; }
    public decimal? tax_percentage { get; set; }
    public int? payment_terms { get; set; }
    public DateOnly? invoice_due_date { get; set; }
    public bool? is_taxable { get; set; }
}
