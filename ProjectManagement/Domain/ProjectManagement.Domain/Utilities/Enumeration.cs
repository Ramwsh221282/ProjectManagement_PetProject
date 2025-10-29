using System.Reflection;

namespace ProjectManagement.Domain.Utilities;

/// <summary>
/// базовое "умное" перечисление
/// </summary>
public abstract class Enumeration<TEnum>
    where TEnum : Enumeration<TEnum>
{
    /// <summary>
    /// Фабрики перечисления
    /// </summary>
    private static readonly Dictionary<int, Func<TEnum>> _factories = InitializeFactories();

    /// <summary>
    /// Название перечисления
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Значение перечисления
    /// </summary>
    public int Value { get; }

    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }

    /// <summary>
    /// Созданием перечисления по имени
    /// </summary>
    /// <param name="name">Название перечисления</param>
    /// <returns>TEnum - перечисление из семейства</returns>
    /// <exception cref="ArgumentException">Исключение, если перечисления с таким названием не существует</exception>
    public static TEnum FromName(string name)
    {
        var dictionary = _factories;
        
        foreach (KeyValuePair<int, Func<TEnum>> keyPair in _factories)
        {
            TEnum enumeration = keyPair.Value();
            if (enumeration.Name == name)
                return enumeration;
        }

        throw new ArgumentException("Данное перечисление не поддерживавется.");
    }

    /// <summary>
    /// Создание перечисления по ключу
    /// </summary>
    /// <param name="value">Ключ перечисления</param>
    /// <returns>TEnum - перечисление из семейства</returns>
    /// <exception cref="ArgumentException">Исключение, если перечисления с таким ключом не существует</exception>
    public static TEnum FromValue(int value) =>
        !_factories.TryGetValue(value, out Func<TEnum>? factory)
            ? throw new ArgumentException("Данное перечисление не поддерживавется.")
            : factory();

    /// <summary>
    /// Инициализация словаря фабрик перечислений
    /// </summary>
    /// <returns>Словарь фабрик перечислений</returns>
    private static Dictionary<int, Func<TEnum>> InitializeFactories()
    {
        // получение всех подтипов семейства перечислений TEnum
        Type enumType = typeof(TEnum);
        Type[] subtypes = [.. enumType.Assembly.GetTypes().Where(t => t.IsSubclassOf(enumType))];

        // заполнение словаря фабрик
        Dictionary<int, Func<TEnum>> factories = [];
        foreach (Type entry in subtypes)
        {
            ConstructorInfo constructorInfo = entry
                .GetConstructors()
                .First(c => c.GetParameters().Length == 0);

            TEnum enumeration = (TEnum)constructorInfo.Invoke(null);
            int key = enumeration.Value;

            // конструктор конкретного перечисления, используем в лямбде
            Func<TEnum> factory = () => (TEnum)constructorInfo.Invoke(null);
            factories.Add(key, factory);
        }

        return factories;
    }
}
