namespace SimpleVersion
{
    public class Semver2Result : IVersionResult
    {
        public Semver2Result(VersionInfo info, int height)
        {
            Height = height;
            Version = info.Version;
            Label = string.Join(".", info.Label);

            if (!string.IsNullOrWhiteSpace(Label))
                Label += $".{Height}";
            else if (!string.IsNullOrWhiteSpace(MetaData))
                MetaData += Height;

            if (info.MetaData.Count > 0)
                MetaData += $".{string.Join(".", info.MetaData)}";
        }

        public int Height { get; }

        public string Version { get; }

        public string Label { get; }

        public string MetaData { get; }

        public string FullVersion => ToString();

        public override string ToString()
        {
            var result = Version;
            if (!string.IsNullOrWhiteSpace(Label))
                result += $"-{Label}";

            if (!string.IsNullOrWhiteSpace(MetaData))
                result += $"+{MetaData}";

            return result;
        }
    }
}
