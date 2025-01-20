﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Prometheus.Database.Models;

namespace Prometheus.Database.Configurations;

public class UserRoleConfiguration : BaseConfiguration<UserRole>
{
    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_roles");

        builder.HasOne<Role>(x => x.role).WithMany().HasForeignKey(x => x.role_id).HasPrincipalKey(c => c.id);
    }
}