using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public VoucherType DiscountType { get; set; } = VoucherType.FixedAmount;

        [Column(TypeName = "decimal(8,2)")]
        public decimal DiscountAmount { get; set; }
        public int? UsageLimit { get; set; } = null;
        public DateTime? Expiration { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = false;
    }
}
