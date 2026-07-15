using Autodesk.Revit.DB;
using BIMBrain.Knowledge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Classification
{
    public class ElementClassifier
    {
        private static readonly ClassificationRepository _repository = new ClassificationRepository();

        public ElementClassification Classify(Element element)
        {
            var byCategory = ClassifyByCategory(element);
            if (byCategory.Classification != ElementClassificationType.Unknown)
            {
                return byCategory;
            }

            if (element is FamilyInstance fi)
            {
                var symbolName = fi.Symbol?.Name;
                var byType = ResolveByName(symbolName);
                if (byType != ElementClassificationType.Unknown)
                {
                    return AliasResult(element.Id, byType, 80, "Classificado pelo alias do tipo.");
                }

                var familyName = fi.Symbol?.Family?.Name;
                var byFamily = ResolveByName(familyName);
                if (byFamily != ElementClassificationType.Unknown)
                {
                    return AliasResult(element.Id, byFamily, 70, "Classificado pelo alias da família.");
                }
            }

            return new ElementClassification
            {
                ElementId = element?.Id ?? ElementId.InvalidElementId,
                Classification = ElementClassificationType.Unknown,
                Confidence = 0,
                Reason = "Categoria ainda não mapeada."
            };
        }

        private static ElementClassificationType ResolveByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ElementClassificationType.Unknown;
            }

            var exact = _repository.FindByAlias(name);
            if (exact != ElementClassificationType.Unknown)
            {
                return exact;
            }

            return ResolveByContains(name);
        }

        private static ElementClassificationType ResolveByContains(string name)
        {
            foreach (var kvp in _repository.GetAllAliases())
            {
                if (kvp.Value.Any(a => name.IndexOf(a, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (Enum.TryParse<ElementClassificationType>(kvp.Key, out var t))
                    {
                        return t;
                    }
                }
            }

            return ElementClassificationType.Unknown;
        }

        private static ElementClassification AliasResult(
            ElementId id, ElementClassificationType type, double confidence, string reason)
        {
            return new ElementClassification
            {
                ElementId = id,
                Classification = type,
                Confidence = confidence,
                Reason = reason
            };
        }

        private static ElementClassification ClassifyByCategory(Element element)
        {
            var classification = ElementClassificationType.Unknown;

            if (element?.Category != null)
            {
                switch (element.Category.BuiltInCategory)
                {
                    case BuiltInCategory.OST_ElectricalEquipment:
                        classification = ElementClassificationType.Panel;
                        break;
                    case BuiltInCategory.OST_ElectricalCircuit:
                        classification = ElementClassificationType.Circuit;
                        break;
                    case BuiltInCategory.OST_Conduit:
                        classification = ElementClassificationType.Conduit;
                        break;
                    case BuiltInCategory.OST_CableTray:
                        classification = ElementClassificationType.CableTray;
                        break;
                    case BuiltInCategory.OST_Rooms:
                        classification = ElementClassificationType.Room;
                        break;
                    case BuiltInCategory.OST_Levels:
                        classification = ElementClassificationType.Level;
                        break;
                    default:
                        classification = ElementClassificationType.Unknown;
                        break;
                }
            }

            if (classification == ElementClassificationType.Unknown)
            {
                return new ElementClassification
                {
                    ElementId = element?.Id ?? ElementId.InvalidElementId,
                    Classification = ElementClassificationType.Unknown,
                    Confidence = 0,
                    Reason = "Categoria ainda não mapeada."
                };
            }

            return new ElementClassification
            {
                ElementId = element.Id,
                Classification = classification,
                Confidence = 100,
                Reason = "Classificado pela categoria Revit."
            };
        }

        public ElementClassification Classify(ElementId id)
        {
            return new ElementClassification
            {
                ElementId = id ?? ElementId.InvalidElementId,
                Classification = ElementClassificationType.Unknown,
                Confidence = 0,
                Reason = "Classification engine not implemented."
            };
        }

        public List<ElementClassification> Classify(IEnumerable<Element> elements)
        {
            var result = new List<ElementClassification>();

            if (elements == null)
            {
                return result;
            }

            foreach (var element in elements)
            {
                result.Add(Classify(element));
            }

            return result;
        }
    }
}
