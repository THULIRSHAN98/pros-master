using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pro.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        public string PositionName { get; set; }
        [JsonIgnore]
        public User User { get; set; }

    }
}
