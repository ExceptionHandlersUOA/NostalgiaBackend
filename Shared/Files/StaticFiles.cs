namespace Shared.Files
{
    public static class StaticFiles
    {
        private const string FileHostDirectory = "StaticServedFiles";

        private static string GetFileHostDirectory()
        {
            var directory = Path.Combine(Constants.BaseDirectory, FileHostDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        private static string GetRandomFileName(string extension)
        {
            var fileName = $"{Path.GetRandomFileName()}.{extension}";

            if (File.Exists(Path.Combine(GetFileHostDirectory(), fileName)))
                return GetRandomFileName(extension);

            return fileName;
        }

        public static async Task<string> AddFileToSystem(Stream stream, string extension)
        {
            var directory = Path.Combine(GetFileHostDirectory());
            var fileName = GetRandomFileName(extension);

            var path = Path.Combine(directory, fileName);

            var fileStream = new FileStream(path, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);

            return fileName;
        }

        public static async Task<byte[]> GetFileOnSystem(string fileName)
        {
            var filePath = Path.Combine(GetFileHostDirectory(), fileName);

            return await File.ReadAllBytesAsync(filePath);
        }

        public static void DeleteFileOnSystem(string fileName)
        {
            var filePath = Path.Combine(GetFileHostDirectory(), fileName);

            File.Delete(filePath);
        }
    }
}
