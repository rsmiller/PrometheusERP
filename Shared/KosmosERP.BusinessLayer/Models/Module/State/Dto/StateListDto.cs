﻿using KosmosERP.Models;

namespace KosmosERP.BusinessLayer.Models.Module.State.Dto;

public class StateListDto : BaseDto
{
    public int country_id { get; set; }
    public string state_name { get; set; }
    public string iso2 { get; set; }
}
