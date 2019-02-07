namespace SimpleVersion
{
    public class VersioningEnvironment : IEnvironment
    {
        public string GetVariable(string name) => System.Environment.GetEnvironmentVariable(name);
    }
}
