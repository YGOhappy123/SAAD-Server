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
    public class ToppingRepository : IToppingRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public ToppingRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Topping> ApplyFilters(IQueryable<Topping> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "startTime":
                            query = query.Where(tp => tp.CreatedAt >= DateTime.Parse(value));
                            break;
                        case "endTime":
                            query = query.Where(tp => tp.CreatedAt <= DateTime.Parse(value));
                            break;
                        case "nameVi":
                            query = query.Where(tp => tp.NameVi.Contains(value));
                            break;
                        case "nameEn":
                            query = query.Where(tp => tp.NameEn.Contains(value));
                            break;
                        case "minPrice":
                            query = query.Where(tp => tp.Price >= Convert.ToDecimal(value));
                            break;
                        case "maxPrice":
                            query = query.Where(tp => tp.Price <= Convert.ToDecimal(value));
                            break;
                        default:
                            query = query.Where(tp => EF.Property<string>(tp, filter.Key.CapitalizeEachWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Topping> ApplySorting(IQueryable<Topping> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(tp => EF.Property<object>(tp, order.Key.CapitalizeEachWords()))
                        : query.OrderByDescending(tp => EF.Property<object>(tp, order.Key.CapitalizeEachWords()));
            }

            return query;
        }

        public async Task<(List<Topping>, int)> GetAllToppings(BaseQueryObject queryObject)
        {
            var query = _dbContext.Toppings.AsQueryable();

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

            var toppings = await query.ToListAsync();

            return (toppings, total);
        }

        public async Task<Topping?> GetToppingById(int toppingId)
        {
            return await _dbContext.Toppings.Where(tp => tp.Id == toppingId).FirstOrDefaultAsync();
        }

        public async Task<Topping?> GetToppingByName(string nameVi, string nameEn)
        {
            return await _dbContext
                .Toppings.Where(tp =>
                    tp.NameVi.Equals(nameVi, StringComparison.OrdinalIgnoreCase)
                    || tp.NameEn.Equals(nameEn, StringComparison.OrdinalIgnoreCase)
                )
                .SingleOrDefaultAsync();
        }

        public async Task AddTopping(Topping topping)
        {
            _dbContext.Toppings.Add(topping);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTopping(Topping topping)
        {
            _dbContext.Toppings.Update(topping);
            await _dbContext.SaveChangesAsync();
        }
    }
}
