using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace StarLine.Core.CommonService
{
    public class HierarchyService
    {
        private readonly IWebHostEnvironment _env;
        private string path = "";
        public HierarchyService(IWebHostEnvironment env)
        {
            _env = env;
            path = Path.Combine(_env.WebRootPath, "HRMSDocs", "Hierarchy.json");
        }

        public async Task<Dictionary<int, string>> GetHierarchyLevelsAsync()
        {
            if (!File.Exists(path))
                return new Dictionary<int, string>();

            var json = await File.ReadAllTextAsync(path);
            var levels = JsonSerializer.Deserialize<Dictionary<int, string>>(json);

            return levels ?? new Dictionary<int, string>();
        }

        public async Task AddHierarchyLevelAsync(int level, string name)
        {
            var levels = await GetHierarchyLevelsAsync();

            if (!levels.ContainsKey(level))
            {
                levels[level] = name;
            }
            var json = JsonSerializer.Serialize(levels, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }
    }
}
