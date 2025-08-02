using Shared.Enums;

namespace Shared.Models
{
    public class CustomMedia
    {
        public FileType Type { get; set; } = FileType.Unknown;
        public string FileName { get; set; } = string.Empty;
    }
}
