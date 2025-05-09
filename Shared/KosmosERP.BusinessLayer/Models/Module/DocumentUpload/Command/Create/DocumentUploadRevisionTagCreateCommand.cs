﻿using KosmosERP.Models;
using System.ComponentModel.DataAnnotations;

namespace KosmosERP.BusinessLayer.Models.Module.DocumentUpload.Command.Create;

public class DocumentUploadRevisionTagCreateCommand : DataCommand
{
    public int? document_upload_revision_id { get; set; }

    [Required]
    public int document_upload_object_tag_id { get; set; }

    [Required]
    public string tag_name { get; set; }

    [Required]
    public string tag_value { get; set; }

    [Required]
    public bool is_required { get; set; } = false;
}
