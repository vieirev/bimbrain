using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class ModelContext
    {
        public DocumentInfo MainDocument { get; }
        public IReadOnlyList<DocumentInfo> Links { get; }
        public bool HasDuplicateNames { get; }

        public ModelContext(Document doc)
        {
            MainDocument = new DocumentInfo(
                doc.Title ?? "Não identificado", "Projeto ativo", true, doc);

            var (links, hasDuplicates) = DiscoverLinks(doc);
            Links = links;
            HasDuplicateNames = hasDuplicates;
        }

        private static (List<DocumentInfo>, bool) DiscoverLinks(Document doc)
        {
            var result = new List<DocumentInfo>();
            var instances = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance))
                .ToElements()
                .Cast<RevitLinkInstance>();

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var nameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var instance in instances)
            {
                var linkDoc = instance.GetLinkDocument();
                var name = linkDoc?.Title ?? instance.Name;

                nameCounts.TryGetValue(name, out var cnt);
                nameCounts[name] = cnt + 1;

                if (!seen.Add(name))
                    continue;

                result.Add(new DocumentInfo(name, "Revit Link", false, linkDoc));
            }

            var hasDuplicates = nameCounts.Values.Any(c => c > 1);
            return (result, hasDuplicates);
        }
    }

    public class DocumentInfo
    {
        public string Name { get; }
        public string Type { get; }
        public bool IsLoaded { get; }
        public bool IsMainDocument { get; }
        public int RoomCount { get; }
        public int LevelCount { get; }
        public int ElementCount { get; }
        public string FilePath { get; }
        public Document Document { get; }

        public DocumentInfo(string name, string type, bool isMain, Document doc)
        {
            Name = name;
            Type = type;
            IsLoaded = doc != null;
            IsMainDocument = isMain;
            FilePath = doc?.PathName ?? "";
            Document = doc;

            if (doc != null)
            {
                RoomCount = CountRooms(doc);
                LevelCount = CountLevels(doc);
                ElementCount = CountElements(doc);
            }
        }

        private static int CountRooms(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .ToElements().Count;
        }

        private static int CountLevels(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .ToElements().Count;
        }

        private static int CountElements(Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .ToElements().Count;
        }
    }
}
