using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Response;

namespace milktea_server.Interfaces.Services
{
    public interface IStatisticService
    {
        Task<ServiceResponse<object>> GetSummaryStatistic(string type);
        Task<ServiceResponse<object>> GetPopularStatistic(string type);
        Task<ServiceResponse<object>> GetRevenuesChart(string type, string locale);
        Task<ServiceResponse<object>> GetProductStatistic(int milkteaId);
    }
}
