using AcadShared.Extensions.AutoCad;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using AecPropertySet = Autodesk.Aec.PropertyData.DatabaseServices.PropertySet;

namespace NutsonCivilPlugin.PropertySet;

/// <summary>
/// Модель представления для управления наборами свойств
/// </summary>
public class PropertySetsManagerViewModel
{
    private readonly Document _doc;

    /// <summary>
    /// Список всех имен наборов свойств в чертеже
    /// </summary>
    public List<string> AllDrawingPropsName;

    /// <summary>
    /// Количество наборов свойств
    /// </summary>
    public int PropCount { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр класса PropertySetsManagerViewModel
    /// </summary>
    public PropertySetsManagerViewModel()
    {
        _doc = Application.DocumentManager.MdiActiveDocument;
        using var tr = _doc.Database.TransactionManager.StartTransaction();

        var NOD = _doc.Database.NamedObjectsDictionaryId.As<DBDictionary>(tr, OpenMode.ForWrite);
        var dict = NOD?.GetAt("AEC_PROPERTY_SET_DEFS").As<DBDictionary>(tr, OpenMode.ForWrite);

        AllDrawingPropsName = dict.Cast<DBDictionaryEntry>().Select(i => i.Key).ToList();
        PropCount = dict?.Count ?? 0;

        tr.Commit();
    }

    /// <summary>
    /// Удаляет выбранные наборы свойств
    /// </summary>
    /// <param name="PropNameForDelete">Список имен наборов свойств для удаления</param>
    public void DeleteSelectedProperty(List<string> PropNameForDelete)
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.Database.TransactionManager.StartTransaction();

        var NOD = _doc.Database.NamedObjectsDictionaryId.As<DBDictionary>(tr, OpenMode.ForWrite);
        var dict = NOD?.GetAt("AEC_PROPERTY_SET_DEFS").As<DBDictionary>(tr, OpenMode.ForWrite);

        PropNameForDelete.ForEach(s => dict?.Remove(s));

        tr.Commit();
    }

    private void RemoveProp(Transaction tr, DBDictionary dict, string PropName)
    {
        var propDefId = (ObjectId)dict[PropName];
        var PropDef = propDefId.As<PropertySetDefinition>(tr, OpenMode.ForWrite);

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
            .Select(id => id.As<DBObject>(tr, OpenMode.ForWrite))
            .SelectMany(obj => PropertyDataServices.GetPropertySets(obj).Cast<ObjectId>())
            .Select(propId => propId.As<AecPropertySet>(tr))
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
                if (prop != null && propDefId == prop.PropertySetDefinition)
                {
                    propsId.Remove(propId);
                    break;
                }
            }
        }
    }
}
