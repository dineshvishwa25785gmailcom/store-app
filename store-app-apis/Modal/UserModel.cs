using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace store_app_apis.Modal
{
    public class UserModel
    {

        public string Username { get; set; } = null!;


        public string Name { get; set; } = null!;


        public string? Email { get; set; }


        public string? Phone { get; set; }


        public string? Password { get; set; }


        public bool? Isactive { get; set; }


        public string? Role { get; set; }


        public bool? Islocked { get; set; }


        public int? Failattempt { get; set; }

        public string? Statusname { get; set; }
    }
}
