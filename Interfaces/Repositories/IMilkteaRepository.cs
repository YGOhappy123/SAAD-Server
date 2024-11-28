using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Repositories
{
    public interface IMilkteaRepository
    {
        Task<(List<Milktea>, int)> GetAllMilkteas(BaseQueryObject queryObject);
        Task<Milktea?> GetMilkteaById(int milkteaId);
        Task<Milktea?> GetMilkteaByName(string nameVi, string nameEn);
        Task<List<Milktea>> SearchMilkteasByName(string searchTerm);
        Task AddMilktea(Milktea milktea);
        Task UpdateMilktea(Milktea milktea);
        Task DisableMilkteasRelatedToCategory(int categoryId);
    }
}
