using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pro.Models
{
    public class JobApplication
    {
        [Key]
        public int AppId { get; set; }

        public string UserId { get; set; }

        public string DesiredLocation { get; set; }

        public bool IsFullTimePosition { get; set; }

        public string StartDate { get; set; }

        public string Source { get; set; }

        public string PreferredContactMethod { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        internal JobApplication FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public static implicit operator JobApplication(List<JobApplication> v)
        {
            throw new NotImplementedException();
        }
    }

   
}
