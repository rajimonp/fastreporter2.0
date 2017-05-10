﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.LinkedIn")]
    public class LinkedInSettingsPartRecord : ContentPartRecord
    {
        public virtual string ClientId { get; set; }
        public virtual string EncryptedClientSecret { get; set; }
    }
}
