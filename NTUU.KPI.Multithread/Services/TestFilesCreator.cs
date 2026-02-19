using Bogus;
using NTUU.KPI.Multithread.Interfaces;

namespace NTUU.KPI.Multithread.Services;

internal sealed class TestFilesCreator : IFilesCreator
{
    private readonly HashSet<string> _fileFormats = ["txt", "md", "log", "csv", "json"];

    private readonly Faker _faker = new();

    public async ValueTask<string> GenerateAsync(string path, int fileCount = 1000)
    {
        var root = Path.Combine(path, "test_data");

        if (Directory.Exists(root))
            return root;

        var directories = CreateDirectoryPaths(current: root, depth: 0, maxDepth: 5).ToArray();

        foreach (var dir in directories)
            Directory.CreateDirectory(dir);

        for (int i = 0; i < fileCount; i++)
        {
            var dir = directories[Random.Shared.Next(directories.Length)];

            var ext = _fileFormats.ElementAt(Random.Shared.Next(_fileFormats.Count));
            var fileName = _faker.System.FileName(ext);

            var filePath = Path.Combine(dir, $"{i}_{fileName}");
            var content = _faker.Lorem.Paragraphs(Random.Shared.Next(3, 15));
            await File.WriteAllTextAsync(filePath, content);
        }

        return root;
    }

    public ValueTask CleanupAsync(string rootPath)
    {
        if (Directory.Exists(rootPath))
            Directory.Delete(rootPath, recursive: true);

        return ValueTask.CompletedTask;
    }

    private IEnumerable<string> CreateDirectoryPaths(
        string current,
        int depth,
        int maxDepth)
    {
        yield return current;

        if (depth >= maxDepth)
            yield break;

        int childCount = Random.Shared.Next(2, 6);
        for (int i = 0; i < childCount; i++)
        {
            var childName = depth == 0
                ? _faker.Commerce.Department()
                : _faker.Commerce.Product();

            var childPath = Path.Combine(current, $"{childName}_{i}");

            foreach (var dir in CreateDirectoryPaths(childPath, depth + 1, maxDepth))
                yield return dir;
        }
    }
}
