﻿using Prometheus.Database.Models;

namespace Prometheus.Api.Models.Module.User.Dto;

public class RoleDto
{
    public required int role_id { get; set; }
    public required string name { get; set; }

    public List<ModuleRolePermissionDto> module_permission { get; set; } = new List<ModuleRolePermissionDto>();
}
