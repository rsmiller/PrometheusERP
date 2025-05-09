﻿using KosmosERP.Models;
using System.ComponentModel.DataAnnotations;

namespace KosmosERP.BusinessLayer.Models.Module.Customer.Command.Create;

public class CustomerCreateCommand : DataCommand
{
    [Required]
    public string customer_name { get; set; }
    public string? customer_description { get; set; }

    [Required]
    public string phone { get; set; }
    public string? fax { get; set; }
    public string? general_email { get; set; }
    public string? website { get; set; }
    public string category { get; set; }
    [Required]
    public bool is_taxable { get; set; } = true;
    [Required]
    public decimal tax_rate { get; set; } = 0;
}
