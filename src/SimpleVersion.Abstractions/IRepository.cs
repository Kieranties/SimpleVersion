namespace SimpleVersion
{
    public interface IRepository
    {
        (int height, VersionModel model) GetResult();
    }
}
