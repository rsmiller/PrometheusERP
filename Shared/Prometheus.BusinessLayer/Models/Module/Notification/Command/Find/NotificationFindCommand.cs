﻿using Prometheus.Models;
using System.ComponentModel.DataAnnotations;


namespace Prometheus.BusinessLayer.Models.Module.Notification.Command.Find;

public class NotificationFindCommand : DataCommand
{
    [Required]
    public int user_id { get; set; }
    public bool show_read { get; set; } = false;
}
