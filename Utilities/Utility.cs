namespace Utilities;

public static class Utility
{
    private const string InputFileName = "input.txt";

    public static StreamReader ReadInputFileForProject(string currentDirectory)
    {
        var projectPath = new DirectoryInfo(currentDirectory).Parent?.Parent?.Parent?.FullName;
        var fullInputPath = Path.Combine(projectPath ?? throw new InvalidOperationException(), InputFileName);
        return new StreamReader(fullInputPath);
    }
}