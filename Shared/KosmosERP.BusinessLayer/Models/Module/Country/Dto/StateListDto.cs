﻿using KosmosERP.Models;
using KosmosERP.BusinessLayer.Models.Module.State.Dto;

namespace KosmosERP.BusinessLayer.Models.Module.Country.Dto;

public class CountryListDto : BaseDto
{
    public required string country_name { get; set; }

    public required string iso3 { get; set; }

    public string? phonecode { get; set; }

    public string? currency { get; set; }
    public string? currency_symbol { get; set; }
    public string? region { get; set; }
}
