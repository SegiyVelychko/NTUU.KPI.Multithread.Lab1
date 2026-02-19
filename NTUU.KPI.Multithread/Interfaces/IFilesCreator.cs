namespace NTUU.KPI.Multithread.Interfaces;

internal interface IFilesCreator
{
    ValueTask<string> GenerateAsync(string path, int fileCount = 1000);

    ValueTask CleanupAsync(string rootPath);
}
