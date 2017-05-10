﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Facebook")]
    public class FacebookSettingsPart : ContentPart<FacebookSettingsPartRecord>
    {
        [Required(ErrorMessage = "Facebook ClientId is required")]
        public string ClientId { get { return Record.ClientId; } set { Record.ClientId = value; } }

        public string ClientSecret { get; set; }
    }
}
