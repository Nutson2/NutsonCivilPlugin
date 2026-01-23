namespace Shared;

/// <summary>
/// Методы расширения кортежей
/// </summary>
public static class TupleExtensions
{
    /// <summary>
    /// Деконструктор для кортежа из 2 элементов для применения в качестве аргументов функции
    /// </summary>
    /// <typeparam name="T1">Тип аргумента 1</typeparam>
    /// <typeparam name="T2">Тип аргумента 2</typeparam>
    /// <typeparam name="T3">Возвращаемый тип</typeparam>
    /// <param name="tuple">Кортеж из 2 элементов</param>
    /// <param name="func">Функция для применения</param>
    public static T3 ApplyTo<T1, T2, T3>(this (T1, T2) tuple, Func<T1, T2, T3> func)
    {
        return func(tuple.Item1, tuple.Item2);
    }

    /// <summary>
    /// Деконструктор для кортежа из 2 элементов для применения в качестве аргументов функции
    /// </summary>
    /// <typeparam name="T1">Тип аргумента 1</typeparam>
    /// <typeparam name="T2">Тип аргумента 2</typeparam>
    /// <param name="tuple">Кортеж из 2 элементов</param>
    /// <param name="action">Функция для применения</param>
    public static void ApplyTo<T1, T2>(this (T1, T2) tuple, Action<T1, T2> action)
    {
        action(tuple.Item1, tuple.Item2);
    }

    /// <summary>
    /// Деконструктор для кортежа из 3 элементов для применения в качестве аргументов функции
    /// </summary>
    /// <typeparam name="T1">Тип аргумента 1</typeparam>
    /// <typeparam name="T2">Тип аргумента 2</typeparam>
    /// <typeparam name="T3">Тип аргумента 3</typeparam>
    /// <typeparam name="T4">Возвращаемый тип</typeparam>
    /// <param name="tuple">Кортеж из 3 элементов</param>
    /// <param name="func">Функция для применения</param>
    public static T4 ApplyTo<T1, T2, T3, T4>(this (T1, T2, T3) tuple, Func<T1, T2, T3, T4> func)
    {
        return func(tuple.Item1, tuple.Item2, tuple.Item3);
    }
}