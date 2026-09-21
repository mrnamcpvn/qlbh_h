using API._Repositories;
using API.Data;
using API.DTOs;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

#nullable enable
namespace API._Services.Services
{
    public class BaseServices
    {
        protected readonly IRepositoryAccessor _repoAccessor;
        public BaseServices(DBContext dbContext)
        {
            _repoAccessor = new RepositoryAccessor<DBContext>(dbContext);
        }
    }
}