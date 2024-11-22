using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<(List<Category>, int)> GetAllCategories(BaseQueryObject queryObject);
    }
}
