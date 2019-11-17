using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Exate.Rules.WebApi.Models
{
    public class ManifestTreeRequestModel
    {
        [Required]
        public SamplePayloadTypeEnum PayloadType { get; set; }

        [Required]
        public string SamplePayload { get; set; }
    }
}
