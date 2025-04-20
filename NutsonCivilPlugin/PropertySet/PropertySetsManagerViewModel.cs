using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using AecPropertySet = Autodesk.Aec.PropertyData.DatabaseServices.PropertySet;

namespace NutsonCivilPlugin.PropertySet;

public class PropertySetsManagerViewModel
{
    private readonly Document _doc;
    public List<string> AllDrawingPropsName;
    public int PropCount { get; set; }

    public PropertySetsManagerViewModel()
    {
        _doc = Application.DocumentManager.MdiActiveDocument;
        using var tr = _doc.Database.TransactionManager.StartTransaction();

        var NOD = _doc.Database.NamedObjectsDictionaryId.As<DBDictionary>(OpenMode.ForWrite);
        var dict = NOD?.GetAt("AEC_PROPERTY_SET_DEFS").As<DBDictionary>(OpenMode.ForWrite);

        AllDrawingPropsName = dict.Cast<DBDictionaryEntry>().Select(i => i.Key).ToList();
        PropCount = dict?.Count ?? 0;

        tr.Commit();
    }

    public void DeleteSelectedProperty(List<string> PropNameForDelete)
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.Database.TransactionManager.StartTransaction();

        var NOD = _doc.Database.NamedObjectsDictionaryId.As<DBDictionary>(OpenMode.ForWrite);
        var dict = NOD?.GetAt("AEC_PROPERTY_SET_DEFS").As<DBDictionary>(OpenMode.ForWrite);

        PropNameForDelete.ForEach(s => dict?.Remove(s));

        tr.Commit();
    }

    private void RemoveProp(Transaction tr, DBDictionary dict, string PropName)
    {
        var propDefId = (ObjectId)dict[PropName];
        var PropDef = propDefId.As<PropertySetDefinition>(OpenMode.ForWrite);

        if (PropDef is null || PropDef.AppliesToFilter.Count == 0)
        {
            return;
        }

        var classNames = PropDef.AppliesToFilter;

        var filter = classNames
            .Cast<string>()
            .Select(s => s.Replace("Db", "_").ToUpper())
            .Select(s => new TypedValue((int)DxfCode.Start, s))
            .ToArray();

        var result = _doc.Editor.SelectAll(new SelectionFilter(filter));
        if (result.Status != PromptStatus.OK)
        {
            return;
        }

        result
            .Value.GetObjectIds()
            .Cast<ObjectId>()
            .Select(id => id.As<DBObject>(OpenMode.ForWrite))
            .SelectMany(obj => PropertyDataServices.GetPropertySets(obj).Cast<ObjectId>())
            .Select(propId => propId.As<AecPropertySet>())
            .OfType<AecPropertySet>()
            .Where(prop => propDefId == prop.PropertySetDefinition)
            .ToList();

        foreach (var selObjId in result.Value.GetObjectIds())
        {
            var selObj = tr.GetObject(selObjId, OpenMode.ForWrite);
            var propsId = PropertyDataServices.GetPropertySets(selObj);
            foreach (ObjectId propId in propsId)
            {
                var prop = tr.GetObject(propId, OpenMode.ForRead) as AecPropertySet;
                if (propDefId == prop.PropertySetDefinition)
                {
                    propsId.Remove(propId);
                    break;
                }
            }
        }
    }
}
