using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Product;

namespace milktea_server.Dtos.Statistic
{
    public class MilkteaWithSales
    {
        public int Id { get; set; }
        public string NameVi { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? Image { get; set; }
        public MilkteaPrice? Price { get; set; }
        public int? SoldUnitsThisMonth { get; set; } = 0;
    }
}
