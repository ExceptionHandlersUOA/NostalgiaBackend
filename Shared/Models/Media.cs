using Shared.Enums;

namespace Shared.Models
{
    public class Media
    {
        public int MediaId { get; set; }
        public FileType Type { get; set; } = FileType.Unknown;
        public string FileName { get; set; } = string.Empty;
    }
}
