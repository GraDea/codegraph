using System.Text.RegularExpressions;

namespace CodeGraph.Common;

public class Repository
{
    public string SolutionPath { get; private set; }
    public string ApplicationName { get; private set; }
    
    public Repository(string path)
    {
        var solutionPath = Directory.GetFiles(path, "*.sln").FirstOrDefault();

        if (solutionPath == null)
        {
            throw new FileNotFoundException("Solution not found");
        }
        
        SolutionPath = solutionPath;
        ApplicationName = GetMcsName(path);
    }
    
    string GetMcsName(string directory)
    {
        var path = Path.Combine(directory, "app.toml");
        if (!File.Exists(path))
        {
            return "monolith";
        }
        
        var appTomlContent = File.ReadAllText(path);
        var match = Regex.Match(appTomlContent, @"name.*?=.*?\""(.*?)\""[\n\r]");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return Path.GetFileName(SolutionPath);
    }
}