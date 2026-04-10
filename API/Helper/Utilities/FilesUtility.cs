using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace API.Helper.Utilities
{
    public static class FilesUtility
    {
        private static readonly string _webRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        #region UploadAsync
        /// <summary>
        /// Upload a file to server folder.
        /// </summary>
        /// <param name="file">Uploaded file.</param>
        /// <param name="subfolder">Subfolder. Default: "upload"</param>
        /// <param name="rawFileName">Raw file name. Default: uploaded file name.</param>
        /// <returns>File name.</returns>
        public static async Task<string> UploadAsync(IFormFile file, string subfolder = "upload", string rawFileName = null)
        {
            if (file == null)
                return null;

            var folderPath = Path.Combine(_webRootPath, subfolder);
            var fileName = file.FileName;
            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrEmpty(extension))
                return null;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (!string.IsNullOrEmpty(rawFileName))
                fileName = $"{rawFileName}{extension}";

            var filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            try
            {
                using (FileStream fs = File.Create(filePath))
                {
                    await file.CopyToAsync(fs);
                    await fs.FlushAsync();
                }

                return fileName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Upload a base64 string file to server folder.
        /// </summary>
        /// <param name="file">Uploaded file.</param>
        /// <param name="subfolder">Subfolder. Default: "upload"</param>
        /// <param name="rawFileName">Raw file name. Default: uploaded file name.</param>
        /// <param name="rawExtension">Raw file extension. Default: uploaded file extension.</param>
        /// <returns>File name.</returns>
        public static async Task<string> UploadAsync(string file, string subfolder = "upload", string rawFileName = null, string rawExtension = null)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                    return null;

                var folderPath = Path.Combine(_webRootPath, subfolder);
                var extension = $".{file.Split(';')[0].Split('/')[1]}";

                if (!string.IsNullOrWhiteSpace(rawExtension))
                    extension = $".{rawExtension}";
                if (string.IsNullOrEmpty(extension))
                    return null;

                var fileName = $"{Guid.NewGuid()}{extension}";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (!string.IsNullOrEmpty(rawFileName))
                    fileName = $"{rawFileName}{extension}";

                var filePath = Path.Combine(folderPath, fileName);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                var base64String = file[(file.IndexOf(',') + 1)..];
                var fileData = Convert.FromBase64String(base64String);

                await File.WriteAllBytesAsync(filePath, fileData);
                return fileName;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region DeleteFile
        /// <summary>
        /// Delete file.
        /// </summary>
        /// <param name="filePath">FilePath. Default: "upload"</param>
        public static void DeleteFile(string filePath = "upload")
        {
            if (File.Exists(Path.Combine(_webRootPath, filePath)))
                File.Delete(Path.Combine(_webRootPath, filePath));
        }
        #endregion

        #region SaveFile
        public static async Task SaveFile(IFormFile file, string subfolder = "upload", string rawFileName = null)
        {
            string folderPath = Path.Combine(_webRootPath, subfolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = file.FileName;
            string extension = Path.GetExtension(file.FileName);
            if (!string.IsNullOrEmpty(rawFileName))
                fileName = $"{rawFileName}{extension}";

            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using FileStream fs = File.Create(filePath);
            await file.CopyToAsync(fs);
            await fs.FlushAsync();
        }
        #endregion

        #region GetFiles
        public static List<FileInfoResult> GetFiles(string folder = null)
        {
            folder ??= _webRootPath;
            DirectoryInfo di = new(folder);

            List<FileInfoResult> results = new();

            SearchDirectory(di, results, folder);

            return results;
        }

        private static void SearchDirectory(DirectoryInfo directory, List<FileInfoResult> results, string folder)
        {
            FileInfoResult result = new() { Dictionary = directory.Name };

            DirectoryInfo[] dirs = directory.GetDirectories();
            if (dirs.Any())
            {
                foreach (var d in dirs)
                    SearchDirectory(d, result.Childs, folder);
            }

            FileInfo[] files = directory.GetFiles();
            if (files.Any())
            {
                result.FileDetail = files.Select(f => new FileInfoDetail
                {
                    Name = f.Name,
                    Path = f.FullName.Replace(folder, "").Replace("\\", "/"),
                    // FullPath = f.FullName.Replace("\\", "/"),
                    Extension = f.Extension,
                    Size = FileSizeFormatter(f.Length)
                }).ToList();
            }

            results.Add(result);
        }
        #endregion

        #region FileSizeFormatter
        private static string FileSizeFormatter(decimal size)
        {
            if (size == 0)
                return "0 bytes";

            string[] suffixes = { "bytes", "kb", "MB", "GB", "TB", "PB" };

            int counter = 0;
            while (Math.Round(size / 1024) >= 1)
            {
                size /= 1024;
                counter++;
            }

            return string.Format("{0:n1} {1}", size.ToString("0.##"), suffixes[counter]);
        }
        #endregion

        #region ReadFile
        public static string ReadFile(string path)
        {
            using StreamReader sr = new(Path.Combine(AppContext.BaseDirectory, path));
            // Read the stream to a string, and write the string to the console.
            return sr.ReadToEnd();
        }
        #endregion

        #region Download
        public async static Task<FileContentResult> Download(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string fileExtension = GetContentType(filePath);

            MemoryStream memory = new();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return new FileContentResult(memory.ToArray(), fileExtension);
        }

        public static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        #endregion
    }

    public class FileInfoResult
    {
        public string Dictionary { get; set; }
        public List<FileInfoDetail> FileDetail { get; set; } = new();
        public List<FileInfoResult> Childs { get; set; } = new();
    }

    public class FileInfoDetail
    {
        public string Name { get; set; }
        public string Path { get; set; }
        // public string FullPath { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; } = "";
    }
}