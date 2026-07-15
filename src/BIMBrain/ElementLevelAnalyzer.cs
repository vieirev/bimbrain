using Autodesk.Revit.DB;
using BIMBrain.Classification;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class ElementLevelAnalyzer
    {
        private readonly Document _doc;
        private readonly ElementClassifier _classifier = new ElementClassifier();

        public ElementLevelAnalyzer(Document doc)
        {
            _doc = doc;
        }

        public Dictionary<string, int> Analyze(ElementClassificationType type)
        {
            var candidateCategories = new[]
            {
                BuiltInCategory.OST_ElectricalFixtures,
                BuiltInCategory.OST_LightingFixtures,
                BuiltInCategory.OST_LightingDevices,
                BuiltInCategory.OST_ElectricalEquipment
            };

            var result = new Dictionary<string, int>();

            foreach (var category in candidateCategories)
            {
                var elements = new FilteredElementCollector(_doc)
                    .OfCategory(category)
                    .WhereElementIsNotElementType()
                    .ToElements();

                foreach (var element in elements)
                {
                    var classification = _classifier.Classify(element);
                    if (classification.Classification != type)
                        continue;

                    var levelName = GetLevelName(element);
                    if (levelName == null)
                        continue;

                    result.TryGetValue(levelName, out var count);
                    result[levelName] = count + 1;
                }
            }

            return result;
        }

        private string GetLevelName(Element element)
        {
            var param = element.get_Parameter(BuiltInParameter.LEVEL_PARAM);
            if (param != null && param.HasValue)
            {
                var levelId = param.AsElementId();
                if (levelId != null && levelId != ElementId.InvalidElementId)
                {
                    var level = _doc.GetElement(levelId) as Level;
                    if (level != null)
                        return level.Name;
                }
            }

            if (element.LevelId != null && element.LevelId != ElementId.InvalidElementId)
            {
                var level = _doc.GetElement(element.LevelId) as Level;
                if (level != null)
                    return level.Name;
            }

            return null;
        }
    }
}
