using BIMBrain.Classification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BIMBrain.Knowledge
{
    public class ClassificationRepository
    {
        private static readonly Dictionary<string, List<string>> _aliases = LoadAliases();

        public List<string> GetAliases(ElementClassificationType type)
        {
            return _aliases.TryGetValue(type.ToString(), out var list)
                ? new List<string>(list)
                : new List<string>();
        }

        public IReadOnlyDictionary<string, List<string>> GetAllAliases()
        {
            return _aliases;
        }

        public bool HasAlias(ElementClassificationType type, string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return false;
            }

            if (!_aliases.TryGetValue(type.ToString(), out var list))
            {
                return false;
            }

            return list.Any(a => string.Equals(a, alias, StringComparison.OrdinalIgnoreCase));
        }

        public ElementClassificationType FindByAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return ElementClassificationType.Unknown;
            }

            foreach (var kvp in _aliases)
            {
                if (kvp.Value.Any(a => string.Equals(a, alias, StringComparison.OrdinalIgnoreCase)))
                {
                    return Enum.TryParse<ElementClassificationType>(kvp.Key, out var t)
                        ? t
                        : ElementClassificationType.Unknown;
                }
            }

            return ElementClassificationType.Unknown;
        }

        private static Dictionary<string, List<string>> LoadAliases()
        {
            try
            {
                var root = FindKnowledgeRoot();
                if (root == null)
                {
                    return new Dictionary<string, List<string>>();
                }

                var path = Path.Combine(root, "classification", "aliases.json");
                if (!File.Exists(path))
                {
                    return new Dictionary<string, List<string>>();
                }

                var json = File.ReadAllText(path);
                var dict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
                return dict ?? new Dictionary<string, List<string>>();
            }
            catch
            {
                return new Dictionary<string, List<string>>();
            }
        }

        private static string FindKnowledgeRoot()
        {
            var candidates = new List<string>();

            try
            {
                var asmLocation = typeof(ClassificationRepository).Assembly.Location;
                if (!string.IsNullOrEmpty(asmLocation))
                {
                    candidates.Add(Path.GetDirectoryName(asmLocation));
                }
            }
            catch
            {
            }

            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(baseDir))
                {
                    candidates.Add(baseDir);
                }
            }
            catch
            {
            }

            foreach (var start in candidates)
            {
                var dir = start;
                for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
                {
                    try
                    {
                        var candidate = Path.Combine(dir, "knowledge");
                        if (Directory.Exists(candidate))
                        {
                            return candidate;
                        }
                    }
                    catch
                    {
                    }

                    var parent = Directory.GetParent(dir);
                    dir = parent?.FullName;
                }
            }

            return null;
        }
    }
}
