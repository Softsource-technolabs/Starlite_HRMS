using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StarLine.Core.Common;

namespace StarLine.Core.CommonService
{
    public class FileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<ApiPostResponse<string>> StoreFile(IFormFile file, string path)
        {
            if (file == null || file.Length == 0)
                return new ApiPostResponse<string> { Message = "No file selected.", Success = false };

            // create path to wwwroot/uploads
            string uploadsFolder = Path.Combine(_environment.WebRootPath, path.TrimStart('/', '\\'));

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // generate unique file name
            string extension = Path.GetExtension(file.FileName);

            string fileName;
            string filePath;

            // Keep generating a new GUID filename until one is not found in the folder
            do
            {
                fileName = Guid.NewGuid().ToString() + extension;
                filePath = Path.Combine(uploadsFolder, fileName);
            } while (System.IO.File.Exists(filePath));

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new ApiPostResponse<string> { Data = fileName, Message = "File saved successfully", Success = true };
        }
        public async Task<ApiPostResponse<string>> StoreFile(IFormFile file, string existingFile, string path)
        {
            if (file == null || file.Length == 0)
                return new ApiPostResponse<string> { Message = "No file selected.", Success = false };

            // create path to wwwroot/uploads
            string uploadsFolder = Path.Combine(_environment.WebRootPath, path.TrimStart('/', '\\'));

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            //Remove Existing file
            File.Delete(existingFile);

            // generate unique file name
            string extension = Path.GetExtension(existingFile);
            string fileName = Path.GetFileName(existingFile);

            fileName = fileName + extension;
            string filePath= Path.Combine(uploadsFolder, fileName);
            
            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new ApiPostResponse<string> { Data = fileName, Message = "File update successfully", Success = true };
        }
        public async Task<ApiPostResponse<string>> StoreFile(byte[] file, string path)
        {
            if (file == null || file.Length == 0)
                return new ApiPostResponse<string> { Success=false,Message ="Certificate not found" };
            path = path.Replace('/', '\\');
            // create path to wwwroot/uploads
            string uploadsFolder = Path.Combine(_environment.WebRootPath, path.TrimStart('/', '\\'));
            
            string directoryPath = Path.GetDirectoryName(uploadsFolder);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            // Save the file
            await File.WriteAllBytesAsync(uploadsFolder, file);
            return new ApiPostResponse<string> {Data = uploadsFolder,Success = true, Message = "Certificate stored provided location" };
        }
    }
}
