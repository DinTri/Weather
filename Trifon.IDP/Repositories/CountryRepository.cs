using Microsoft.EntityFrameworkCore;
using Trifon.IDP.Data;
using Trifon.IDP.Entities;

namespace Trifon.IDP.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;

        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.ToListAsync();
        }

        public async Task<Country> GetCountryByIsoAsync(string iso)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Iso == iso);
        }
    }
}
