namespace Application.IRepository
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync();
    }
}
