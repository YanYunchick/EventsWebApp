using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsWebApp.Domain.Contracts;
using EventsWebApp.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EventsWebApp.Infrastructure;

public class FileService : IFileService
{
    private readonly string _filePath;

    public FileService(IConfiguration configuration)
    {
        _filePath = configuration.GetSection("FileStorage")["Path"]!;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string[] allowedFileExtensions)
    {
        if (file == null)
        {
            throw new FileBadRequestException("File is null.");
        }
        var path = _filePath;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var ext = Path.GetExtension(file.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new FileExtensionBadRequestException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }

        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }


    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new FileBadRequestException("File name is null or empty.");
        }

        var fileNameWithPath = Path.Combine(_filePath, fileNameWithExtension);

        if (!File.Exists(fileNameWithPath))
        {
            throw new Domain.Exceptions.FileNotFoundException(fileNameWithPath);
        }
        File.Delete(fileNameWithPath);
    }

    public async Task<(byte[] fileBytes, string contentType, string filename)> GetFileAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new Domain.Exceptions.FileNotFoundException(fileName);

        var filePath = Path.Combine(_filePath, fileName);

        if (!File.Exists(filePath))
            throw new Domain.Exceptions.FileNotFoundException(filePath);

        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var contentType = $"file/{Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant()}";
        return (fileBytes, contentType, fileName);
    }

}
