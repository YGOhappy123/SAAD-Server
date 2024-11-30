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
        Task<List<Milktea>> GetMilkteasByName(string nameVi, string nameEn);
        Task<List<Milktea>> SearchMilkteasByName(string searchTerm);
        Task AddMilktea(Milktea milktea);
        Task UpdateMilktea(Milktea milktea);
        Task DisableMilkteasRelatedToCategory(int categoryId);
        Task<List<Milktea>> GetBestSellers(DateTime startTime, DateTime endTime, int limit);
        Task<(int, decimal)> GetMilkteaStatisticInTimeRange(DateTime startTime, DateTime endTime, int milkteaId);
    }
}
