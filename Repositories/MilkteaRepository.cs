using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using milktea_server.Data;
using milktea_server.Interfaces.Repositories;
using milktea_server.Models;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Repositories
{
    public class MilkteaRepository : IMilkteaRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public MilkteaRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Milktea> ApplyFilters(IQueryable<Milktea> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "startTime":
                            var startTime = DateTime.Parse(value);
                            query = query.Where(m => m.CreatedAt >= startTime);
                            break;
                        case "endTime":
                            var endTime = DateTime.Parse(value);
                            query = query.Where(m => m.CreatedAt <= endTime);
                            break;
                        case "nameVi":
                            query = query.Where(m => m.NameVi.Contains(value));
                            break;
                        case "nameEn":
                            query = query.Where(m => m.NameEn.Contains(value));
                            break;
                        case "minPrice":
                            query = query.Where(m => m.PriceL >= Convert.ToDecimal(value));
                            break;
                        case "maxPrice":
                            query = query.Where(m => m.PriceS <= Convert.ToDecimal(value));
                            break;
                        default:
                            query = query.Where(m => EF.Property<string>(m, filter.Key.CapitalizeEachWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Milktea> ApplySorting(IQueryable<Milktea> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(m => EF.Property<object>(m, order.Key.CapitalizeEachWords()))
                        : query.OrderByDescending(m => EF.Property<object>(m, order.Key.CapitalizeEachWords()));
            }

            return query;
        }

        public async Task<(List<Milktea>, int)> GetAllMilkteas(BaseQueryObject queryObject)
        {
            var query = _dbContext.Milkteas.Include(m => m.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Filter))
            {
                var parsedFilter = JsonSerializer.Deserialize<Dictionary<string, object>>(queryObject.Filter);
                query = ApplyFilters(query, parsedFilter!);
            }

            if (!string.IsNullOrWhiteSpace(queryObject.Sort))
            {
                var parsedSort = JsonSerializer.Deserialize<Dictionary<string, string>>(queryObject.Sort);
                query = ApplySorting(query, parsedSort!);
            }

            var total = await query.CountAsync();

            if (queryObject.Skip.HasValue)
                query = query.Skip(queryObject.Skip.Value);

            if (queryObject.Limit.HasValue)
                query = query.Take(queryObject.Limit.Value);

            var milkteas = await query.ToListAsync();

            return (milkteas, total);
        }
    }
}
