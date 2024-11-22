using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Product;
using milktea_server.Models;

namespace milktea_server.Mappers
{
    public static class MilkteaMapper
    {
        public static MilkteaDto ToMilkteaDto(this Milktea milktea)
        {
            return new MilkteaDto
            {
                Id = milktea.Id,
                NameVi = milktea.NameVi,
                NameEn = milktea.NameEn,
                DescriptionVi = milktea.DescriptionVi,
                DescriptionEn = milktea.DescriptionEn,
                Image = milktea.Image,
                IsAvailable = milktea.IsAvailable,
                IsActive = milktea.IsActive,
                CreatedAt = milktea.CreatedAt,
                Price = new MilkteaPrice
                {
                    S = milktea.PriceS,
                    M = milktea.PriceM,
                    L = milktea.PriceL,
                },
                Category = new CategoryDto
                {
                    Id = milktea.Category!.Id,
                    NameVi = milktea.Category!.NameVi,
                    NameEn = milktea.Category!.NameEn,
                    IsActive = milktea.Category!.IsActive,
                },
            };
        }
    }
}
