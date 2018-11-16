namespace SimpleVersion
{
    public interface IRepositoryResolver
    {
        IRepository Resolve(string path);
    }
}
