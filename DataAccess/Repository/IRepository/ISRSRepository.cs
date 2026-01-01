using Models;

namespace DataAccess.Repository.IRepository
{
    public interface ISRSRepository : IRepository<SRS>
    {
        Task UpdateAsync(SRS objFromDb);
        void UpdateRange(IEnumerable<SRS> entities);
    }
}
