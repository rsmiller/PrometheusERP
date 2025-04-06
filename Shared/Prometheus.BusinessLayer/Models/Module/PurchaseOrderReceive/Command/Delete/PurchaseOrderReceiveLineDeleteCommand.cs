﻿using Prometheus.Models;
using System.ComponentModel.DataAnnotations;

namespace Prometheus.BusinessLayer.Models.Module.PurchaseOrderReceive.Command.Delete;

public class PurchaseOrderReceiveLineDeleteCommand : DataCommand
{
    [Required]
    public int id { get; set; }
}
