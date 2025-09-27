namespace NutsonCivilPlugin.AddPipeOnPV;

public static class FuncExtention
{
    public static Func<T2, Tout> Apply<T1, T2, Tout>(this Func<T1, T2, Tout> func, T1 obj) =>
        (obj2) => func(obj, obj2);
}
