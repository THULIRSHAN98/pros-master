using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using pro.Models; // Add this using directive to reference the classes

namespace pro.Models
{
    public class UserDetails
    {
    public string UserId { get; set; }
    public string UserName { get; set; }
    public Education Education { get; set; }
    public List<Skill> Skills { get; set; }
    public Department Department { get; set; }
    }
   
}
