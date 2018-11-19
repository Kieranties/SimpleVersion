namespace SimpleVersion.Formatters
{
    public class Semver2Format : IVersionFormat
    {
        public void Apply(VersionInfo info, VersionResult result)
        {
            var format = info.Version;

            var label = string.Join(".", info.Label);
            var meta = string.Empty;

            if (!string.IsNullOrWhiteSpace(label))
                label += $".{result.Height}";
            else
                meta += result.Height;

            if (info.MetaData.Count > 0)
                meta += $".{string.Join(".", info.MetaData)}";

            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";

            if (!string.IsNullOrWhiteSpace(meta))
                format += $"+{meta}";

            result.Formats["Semver2"] = format;
        }
    }
}
