﻿using Prometheus.Models;

namespace Prometheus.BusinessLayer.Models.Module.Order.Dto;

public class OrderLineDto : BaseDto
{
    public int order_id { get; set; }
    public int product_id { get; set; }
    public int line_number { get; set; }
    public int? opportunity_line_id { get; set; }
    public int quantity { get; set; }
    public decimal unit_price { get; set; }
    public string guid { get; set; }

    public List<OrderLineAttributeDto> attributes { get; set; } = new List<OrderLineAttributeDto>();
}
