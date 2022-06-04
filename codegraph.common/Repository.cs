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
        var appTomlContent = File.ReadAllText(Path.Combine(directory, "app.toml"));
        var match = Regex.Match(appTomlContent, "name.*?=(.*?)\n");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return Path.GetFileName(SolutionPath);
    }
}