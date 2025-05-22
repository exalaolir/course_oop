using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using course_oop.Shared.Const;

namespace course_oop.Core.Entities
{
    public class Courier : User
    {
        public Consts.Transport? Transport { get; set; }

        public double? Veight { get; set; }

        public double? CurrentWeight { get; set; }

        public string? StatusMessage { get; set; }

        public double? X { get; set; } = 3067936.6575493305;
        public double? Y { get; set; } = 7149649.28063618;

        public bool? IsWork { get; set; } = false;

        public int? SallerMinutes { get; set; }

        public int? UserMinutes { get; set; }

        public string? Adress {  get; set; }
    }
}