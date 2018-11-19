namespace SimpleVersion
{
    public class VersionResult : IVersionResult
    {
        private readonly int _height;
        private readonly VersionInfo _info;

        public VersionResult(VersionInfo info, int height)
        {
            _height = height;
            _info = info;

            Semver1 = GetSemver1();
            Semver2 = GetSemver2();
        }

        public string Semver1 { get; }

        public string Semver2 { get; }

        private string GetSemver1()
        {
            var result = _info.Version;

            var label = string.Join("-", _info.Label);

            if (!string.IsNullOrWhiteSpace(label))
            {
                label += $"-{_height.ToString("D4")}";
                result += $"-{label}";
            }

            return result;

        }

        private string GetSemver2()
        {
            var result = _info.Version;

            var label = string.Join(".", _info.Label);
            var meta = string.Empty;

            if (!string.IsNullOrWhiteSpace(label))
                label += $".{_height}";
            else
                meta += _height;

            if (_info.MetaData.Count > 0)
                meta += $".{string.Join(".", _info.MetaData)}";

            if (!string.IsNullOrWhiteSpace(label))
                result += $"-{label}";

            if (!string.IsNullOrWhiteSpace(meta))
                result += $"+{meta}";

            return result;
        }
    }
}
