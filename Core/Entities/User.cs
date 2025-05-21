using System.ComponentModel.DataAnnotations;
using course_oop.Shared.Const;

namespace course_oop.Core.Entities
{
    public class User
    {
        [Key] public int Id { get; set; }

        [Required] public string? Email { get; set; }

        [Required] public string? Password { get; set; }

        [Required] public string? Phone { get; set; }

        [Required] public string? Name { get; set; }

        [Required] public string? FirstName { get; set; }

        public bool Banned { get; set; } = false;
    }
}