namespace Shared.Files
{
    public static class StaticFiles
    {
        private const string FileHostDirectory = "StaticServedFiles";

        private static string GetRandomFileName(string extension)
        {
            var fileName = $"{Path.GetRandomFileName()}.{extension}";

            if (File.Exists(Path.Combine(FileHostDirectory, fileName)))
                return GetRandomFileName(extension);

            return fileName;
        }

        public static async Task<string> AddFileToSystem(Stream stream, string extension)
        {
            var directory = Path.Combine(FileHostDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fileName = GetRandomFileName(extension);

            var path = Path.Combine(directory, fileName);

            var fileStream = new FileStream(path, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);

            return fileName;
        }

        public static async Task<byte[]> GetFileOnSystem(string fileName)
        {
            var filePath = Path.Combine(FileHostDirectory, fileName);

            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
