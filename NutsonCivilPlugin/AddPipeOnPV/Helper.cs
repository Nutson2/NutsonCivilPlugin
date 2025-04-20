using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;

namespace NutsonCivilPlugin.AddPipeOnPV;

public static class Helper
{
    public static ObjectId SelectPV(Document doc)
    {
        var promptEntityOptions = new PromptEntityOptions("\nВыберите вид профиля")
        {
            AllowNone = false,
        };
        promptEntityOptions.SetRejectMessage("\nНеобходимо выбрать вид профиля");
        promptEntityOptions.AddAllowedClass(typeof(ProfileView), true);

        PromptEntityResult res = doc.Editor.GetEntity(promptEntityOptions);

        return res.Status == PromptStatus.OK ? res.ObjectId : ObjectId.Null;
    }
}
