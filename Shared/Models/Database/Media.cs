using Shared.Enums;

namespace Shared.Models.Database
{
    public class Media
    {
        public int MediaId { get; set; }
        public FileType Type { get; set; }
        public string FileName { get; set; }
    }
}
