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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CategoryRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Category> ApplyFilters(IQueryable<Category> query, Dictionary<string, object> filters)
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
                        default:
                            query = query.Where(m => EF.Property<string>(m, filter.Key.CapitalizeEachWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Category> ApplySorting(IQueryable<Category> query, Dictionary<string, string> sort)
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

        public async Task<(List<Category>, int)> GetAllCategories(BaseQueryObject queryObject)
        {
            var query = _dbContext.Categories.AsQueryable();

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

            var categories = await query.ToListAsync();

            return (categories, total);
        }

        public async Task<Category?> GetCategoryById(int categoryId)
        {
            return await _dbContext.Categories.Where(c => c.Id == categoryId).SingleOrDefaultAsync();
        }

        public async Task<Category?> GetCategoryByName(string nameVi, string nameEn)
        {
            return await _dbContext
                .Categories.Where(c =>
                    c.NameVi.Equals(nameVi, StringComparison.OrdinalIgnoreCase)
                    || c.NameEn.Equals(nameEn, StringComparison.OrdinalIgnoreCase)
                )
                .SingleOrDefaultAsync();
        }

        public async Task AddCategory(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
        }
    }
}
