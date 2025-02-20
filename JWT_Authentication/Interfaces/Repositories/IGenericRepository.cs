namespace JWT_Authentication.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetEntityById(int entityId);
        Task<bool> DeleteEntityById(int EntityId);
        Task UpdateEntity(TEntity entity, int Id);
        Task<ICollection<TEntity>> GetAll();
        Task<bool> CreateEntity(TEntity entity);
    }
}
