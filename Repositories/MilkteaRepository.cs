using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using milktea_server.Data;
using milktea_server.Enums;
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
                            query = query.Where(mt => mt.CreatedAt >= DateTime.Parse(value));
                            break;
                        case "endTime":
                            query = query.Where(mt => mt.CreatedAt <= DateTime.Parse(value));
                            break;
                        case "nameVi":
                            query = query.Where(mt => mt.NameVi.Contains(value));
                            break;
                        case "nameEn":
                            query = query.Where(mt => mt.NameEn.Contains(value));
                            break;
                        case "minPrice":
                            query = query.Where(mt => mt.PriceL >= Convert.ToDecimal(value));
                            break;
                        case "maxPrice":
                            query = query.Where(mt => mt.PriceS <= Convert.ToDecimal(value));
                            break;
                        default:
                            query = query.Where(mt => EF.Property<string>(mt, filter.Key.CapitalizeWord()) == value);
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
                        ? query.OrderBy(mt => EF.Property<object>(mt, order.Key.CapitalizeWord()))
                        : query.OrderByDescending(mt => EF.Property<object>(mt, order.Key.CapitalizeWord()));
            }

            return query;
        }

        public async Task<(List<Milktea>, int)> GetAllMilkteas(BaseQueryObject queryObject)
        {
            var query = _dbContext.Milkteas.Include(mt => mt.Category).AsQueryable();

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

        public async Task<Milktea?> GetMilkteaById(int milkteaId)
        {
            return await _dbContext.Milkteas.Include(mt => mt.Category).Where(mt => mt.Id == milkteaId).FirstOrDefaultAsync();
        }

        public async Task<List<Milktea>> SearchMilkteasByName(string searchTerm)
        {
            return await _dbContext
                .Milkteas.Where(mt => mt.IsActive && mt.IsAvailable && (mt.NameVi.Contains(searchTerm) || mt.NameEn.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<List<Milktea>> GetMilkteasByName(string nameVi, string nameEn)
        {
            return await _dbContext.Milkteas.Where(mt => mt.NameVi == nameVi || mt.NameEn == nameEn).ToListAsync();
        }

        public async Task AddMilktea(Milktea milktea)
        {
            _dbContext.Milkteas.Add(milktea);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateMilktea(Milktea milktea)
        {
            _dbContext.Milkteas.Update(milktea);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DisableMilkteasRelatedToCategory(int categoryId)
        {
            var milkteas = await _dbContext.Milkteas.Where(mt => mt.CategoryId == categoryId).ToListAsync();

            foreach (var milktea in milkteas)
            {
                milktea.IsActive = false;
                milktea.IsAvailable = false;

                _dbContext.Milkteas.Update(milktea);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Milktea>> GetBestSellers(DateTime startTime, DateTime endTime, int limit)
        {
            var orderItemIds = await _dbContext
                .OrderItems.Where(oi =>
                    oi.Order!.CreatedAt >= startTime && oi.Order!.CreatedAt < endTime && oi.Order!.Status == OrderStatus.Done
                )
                .Select(oi => oi.Id)
                .ToListAsync();

            return await _dbContext
                .Milkteas.Include(mt => mt.OrderItems.Where(oi => orderItemIds.Contains(oi.Id)))
                .Where(mt => mt.OrderItems.Any(oi => orderItemIds.Contains(oi.Id)))
                .OrderByDescending(mt => mt.OrderItems.Sum(oi => oi.Quantity * oi.Price))
                .Take(limit)
                .ToListAsync();
        }

        public async Task<(int, decimal)> GetMilkteaStatisticInTimeRange(DateTime startTime, DateTime endTime, int milkteaId)
        {
            var orderItemIds = await _dbContext
                .OrderItems.Where(oi =>
                    oi.Order!.CreatedAt >= startTime && oi.Order!.CreatedAt < endTime && oi.Order!.Status == OrderStatus.Done
                )
                .Select(oi => oi.Id)
                .ToListAsync();

            var milktea = await _dbContext
                .Milkteas.Include(mt => mt.OrderItems.Where(oi => orderItemIds.Contains(oi.Id)))
                .Where(mt => mt.OrderItems.Any(oi => orderItemIds.Contains(oi.Id)) && mt.Id == milkteaId)
                .SingleOrDefaultAsync();

            return (milktea?.OrderItems.Sum(oi => oi.Quantity) ?? 0, milktea?.OrderItems.Sum(oi => oi.Quantity * oi.Price) ?? 0);
        }
    }
}
