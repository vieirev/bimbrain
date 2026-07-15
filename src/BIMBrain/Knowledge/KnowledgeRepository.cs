using System;
using System.IO;
using System.Linq;

namespace BIMBrain.Knowledge
{
    public class KnowledgeRepository
    {
        public KnowledgeDocument GetByRuleName(string ruleName)
        {
            var doc = new KnowledgeDocument { RuleId = ruleName, Exists = false };

            if (string.IsNullOrWhiteSpace(ruleName))
            {
                return doc;
            }

            var knowledgeRoot = FindKnowledgeRoot();
            if (knowledgeRoot == null)
            {
                return doc;
            }

            var standard = ruleName.Contains("-") ? ruleName.Split('-')[0] : ruleName;
            var filePath = Path.Combine(knowledgeRoot, "standards", standard, "rules", ruleName + ".md");

            if (!File.Exists(filePath))
            {
                return doc;
            }

            doc.Markdown = File.ReadAllText(filePath);
            doc.Standard = standard;
            doc.Title = ExtractTitle(doc.Markdown) ?? ruleName;
            doc.Exists = true;

            return doc;
        }

        private static string ExtractTitle(string markdown)
        {
            using (var reader = new StringReader(markdown))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("#"))
                    {
                        return trimmed.TrimStart('#').Trim();
                    }
                }
            }

            return null;
        }

        private static string FindKnowledgeRoot()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;

            for (int i = 0; i < 8; i++)
            {
                var candidate = Path.Combine(dir, "knowledge");
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                var parent = Directory.GetParent(dir);
                if (parent == null)
                {
                    break;
                }

                dir = parent.FullName;
            }

            return null;
        }
    }
}
