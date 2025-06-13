using System.Diagnostics;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Shared.Extensions;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace NutsonCivilPlugin.PropertySet;

class CommandClearPropertySet : System.Windows.Input.ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        try
        {
            new PropertySetView().ShowDialog();
        }
        catch (Exception ex)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.ToString());
        }
    }
}

static class PropertySetManager
{
    public static void GetAllPropSetDef()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        const string name = "Параметры колодцев";

        using var loc = doc.LockDocument();
        using var tr = doc.Database.TransactionManager.StartTransaction();

        var NOD = doc.Database.NamedObjectsDictionaryId.As<DBDictionary>(OpenMode.ForWrite);
        var dict = NOD?.GetAt("AEC_PROPERTY_SET_DEFS").As<DBDictionary>(OpenMode.ForWrite);

        var propNameForDelete = new List<string>();

        foreach (var item in dict)
        {
            var key = item.Key;
            if (key != name && key.Contains(name))
            {
                propNameForDelete.Add(key);
            }
        }

        foreach (var key in propNameForDelete)
        {
            dict.Remove(key);
        }

        tr.Commit();
    }

    public static void GetAllPropSetDef2()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        var dictionary = new DictionaryPropertySetDefinitions(doc.Database);
        var step = 200;
        var prev = 0;
        for (var i = 0; i < ((dictionary.Records.Count / step) + 1) * step; i += step)
        {
            prev = delPropDef(doc, prev, i, dictionary);
        }

        Debug.Print("ok");
    }

    private static int delPropDef(
        Document doc,
        int prev,
        int limit,
        DictionaryPropertySetDefinitions dictionary
    )
    {
        var res = 0;
        using (var loc = doc.LockDocument())
        {
            using var tr = doc.Database.TransactionManager.StartTransaction();
            System.Collections.IList list = dictionary.Records;
            for (var curIndx = prev; curIndx < limit; curIndx++)
            {
                if (curIndx > dictionary.Records.Count)
                {
                    break;
                }

                var objId = (ObjectId)list[curIndx];
                var setDef = tr.GetObject(objId, OpenMode.ForWrite) as PropertySetDefinition;
                if (setDef.LocalizedName.Contains("(") && setDef.LocalizedName.Contains(")"))
                {
                    setDef.Definitions.Clear();
                    setDef.AppliesToFilter.Clear();
                    var f = setDef.AppliesToFilter;
                    f.Clear();
                    setDef.SetAppliesToFilter(f, false);
                    Debug.Print(@"num: {0}, name {1}", curIndx, setDef.LocalizedName);
                }

                res = curIndx;
            }

            tr.Commit();
        }

        return res;
    }
}
