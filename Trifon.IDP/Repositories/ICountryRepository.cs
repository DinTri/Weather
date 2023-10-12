using Trifon.IDP.Entities;

namespace Trifon.IDP.Repositories
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetAllCountriesAsync();
        Task<Country> GetCountryByIsoAsync(string iso);
    }
}
