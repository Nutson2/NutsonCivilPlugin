using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

namespace Test;

public class CommandTest : CivilCommand
{
    [CommandMethod("TestPipe")]
    public override void Execute()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        using var loc = doc.LockDocument();
        using var tr = doc.TransactionManager.StartTransaction();

        var pipeId = SelectElement(doc, typeof(Pipe));
        var pipe = pipeId.As<Pipe>(OpenMode.ForWrite);
        if (pipe == null)
        {
            return;
        }

        pipe.StartPoint = pipe.StartPoint.WithZ(pipe.StartPoint.Z - 4);
        pipe.EndPoint = pipe.EndPoint.WithZ(pipe.EndPoint.Z - 2);

        tr.Commit();
    }

    private ObjectId SelectElement(Document doc, Type ObjectType)
    {
        var promptEntityOptions = new PromptEntityOptions($"\nВыберите вид {ObjectType.Name}")
        {
            AllowNone = false,
        };
        promptEntityOptions.SetRejectMessage($"\nНеобходимо выбрать {ObjectType.Name}");
        promptEntityOptions.AddAllowedClass(ObjectType, true);

        var res = doc.Editor.GetEntity(promptEntityOptions);
        return res.Status == PromptStatus.OK ? res.ObjectId : ObjectId.Null;
    }
}
