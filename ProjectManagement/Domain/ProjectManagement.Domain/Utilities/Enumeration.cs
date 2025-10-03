using System.Reflection;

namespace ProjectManagement.Domain.Utilities;

public abstract class Enumeration<TEnum> where TEnum : Enumeration<TEnum>
{
    private static Dictionary<int, Func<TEnum>> _factories = InitializeFactories();
    public string Name { get; }
    public int Value { get; }

    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name;
    }
    
    public static TEnum FromName(string name)
    {
        foreach (KeyValuePair<int, Func<TEnum>> keyPair in _factories)
        {
            TEnum enumeration = keyPair.Value();
            if (enumeration.Name == name)
                return enumeration;
        }

        throw new ArgumentException("Данное перечисление не поддерживавется.");
    }

    public static TEnum FromValue(int value) =>
        !_factories.TryGetValue(value, out Func<TEnum>? factory) 
            ? throw new ArgumentException("Данное перечисление не поддерживавется.") 
            : factory();

    private static Dictionary<int, Func<TEnum>> InitializeFactories()
    {
        Type enumType = typeof(TEnum);
        
        Type[] subtypes = enumType.Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(enumType))
            .ToArray();

        Dictionary<int, Func<TEnum>> factories = [];
        foreach (Type entry in subtypes)
        {
            ConstructorInfo constructorInfo = entry.GetConstructors().First(c => c.GetParameters().Length == 0);
            TEnum enumeration = (TEnum)constructorInfo.Invoke(null);

            int key = enumeration.Value;
            Func<TEnum> factory = () => (TEnum)constructorInfo.Invoke(null);
            factories.Add(key, factory);
        }
        
        return factories;
    }
}