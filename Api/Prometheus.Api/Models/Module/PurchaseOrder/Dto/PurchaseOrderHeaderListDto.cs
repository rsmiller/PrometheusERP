﻿using Prometheus.Models;

namespace Prometheus.Api.Models.Module.PurchaseOrder.Dto;

public class PurchaseOrderHeaderListDto : BaseDto
{
    public int vendor_id { get; set; }
    public string po_type { get; set; }
    public int revision_number { get; set; }
    public int po_number { get; set; }
    public string? po_quote_number { get; set; }
    public string? deleted_reason { get; set; }
    public string? canceled_reason { get; set; }
    public bool is_complete { get; set; }
    public bool is_canceled { get; set; }
    public DateTime? completed_on { get; set; }
    public int? completed_by { get; set; }
    public DateTime? canceled_on { get; set; }
    public int? canceled_by { get; set; }
    public string guid { get; set; }
}
