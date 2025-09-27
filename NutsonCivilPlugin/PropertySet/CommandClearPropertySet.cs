using System.Diagnostics;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Shared;

namespace NutsonCivilPlugin.PropertySet;

/// <summary>
/// Команда для очистки набора свойств
/// </summary>
class CommandClearPropertySet : CivilCommand
{
    /// <summary>
    /// Выполняет команду очистки набора свойств
    /// </summary>
    /// <param name="parameter">Параметр команды</param>
    public override void Execute() => new PropertySetView().ShowDialog();
}

/// <summary>
/// Менеджер для работы с наборами свойств
/// </summary>
static class PropertySetManager
{
    /// <summary>
    /// Получает и удаляет определения наборов свойств по имени
    /// </summary>
    public static void GetAllPropSetDef()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        const string name = "Параметры колодцев";

        using var loc = doc.LockDocument();
        using var tr = doc.Database.TransactionManager.StartTransaction();

        var NOD = doc.Database.NamedObjectsDictionaryId.As<DBDictionary>(tr, OpenMode.ForWrite);
        var dict = NOD?.GetAt("AEC_PROPERTY_SET_DEFS").As<DBDictionary>(tr, OpenMode.ForWrite);

        if (dict != null)
        {
            var propNameForDelete = dict.Cast<DBDictionaryEntry>()
                .Select(item => item.Key)
                .Where(key => key != name && key.Contains(name));

            foreach (var key in propNameForDelete)
            {
                dict.Remove(key);
            }
        }

        tr.Commit();
    }

    /// <summary>
    /// Альтернативный метод для получения и очистки определений наборов свойств
    /// </summary>
    public static void GetAllPropSetDef2()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        var dictionary = new DictionaryPropertySetDefinitions(doc.Database);
        var step = 200;
        var prev = 0;
        for (var i = 0; i < ((dictionary.Records.Count / step) + 1) * step; i += step)
        {
            prev = DelPropDef(doc, prev, i, dictionary);
        }

        Debug.Print("ok");
    }

    /// <summary>
    /// Удаляет определения наборов свойств в указанном диапазоне
    /// </summary>
    /// <param name="doc">Активный документ</param>
    /// <param name="prev">Начальный индекс</param>
    /// <param name="limit">Конечный индекс</param>
    /// <param name="dictionary">Словарь определений наборов свойств</param>
    /// <returns>Последний обработанный индекс</returns>
    private static int DelPropDef(
        Document doc,
        int prev,
        int limit,
        DictionaryPropertySetDefinitions dictionary
    )
    {
        var res = 0;
        using var loc = doc.LockDocument();
        using var tr = doc.Database.TransactionManager.StartTransaction();

        var list = dictionary.Records;
        for (var curIndx = prev; curIndx < limit; curIndx++)
        {
            if (curIndx > dictionary.Records.Count)
            {
                break;
            }

            var objId = list[curIndx];
            var setDef = tr.GetObject(objId, OpenMode.ForWrite) as PropertySetDefinition;
            if (
                setDef != null
                && setDef.LocalizedName.Contains("(")
                && setDef.LocalizedName.Contains(")")
            )
            {
                setDef.Definitions.Clear();
                setDef.AppliesToFilter.Clear();
                var f = setDef.AppliesToFilter;
                f.Clear();
                setDef.SetAppliesToFilter(f, false);
                Debug.Print(@"num: {0}, name {1}", curIndx, setDef.LocalizedName);
            }

            res = curIndx;
            tr.Commit();
        }

        return res;
    }
}
