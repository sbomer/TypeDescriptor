using System.Diagnostics.CodeAnalysis;
using System.Text;

var person = new Person("John", 30, new Data(1));

TypeDescriptor.Register(typeof(Person));
TypeDescriptor.Register(typeof(Data));
var str = TypeDescriptor.GetConverter(typeof(Person)).ConvertFromString(person);
Console.WriteLine(str);

class TypeDescriptor {
    static HashSet<Type> knownTypes = new();

    public static void Register([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type) {
        knownTypes.Add(type);
    }

    public static TypeConverter GetConverter(Type type) {
        return new ReflectionTypeConverter();
    }

    class ReflectionTypeConverter : TypeConverter {
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075", Justification = "Properties are kept for registered types.")]
        public override string ConvertFromString(object value)
        {
            StringBuilder sb = new();
            Append(value);
            return sb.ToString();

            void Append(object? obj, int indent = 0) {
                if (obj == null) {
                    sb.AppendLine("null");
                    return;
                }
                var type = obj.GetType();
                if (type == typeof(string) || type == typeof(int)) {
                    sb.AppendLine($"{obj}");
                    return;
                }

                if (!knownTypes.Contains(type))
                    throw new InvalidOperationException($"Type {type.Name} not registered");

                sb.AppendLine($"{type.Name}:");
                indent++;

                foreach (var prop in type.GetProperties()) {
                    var propValue = prop.GetValue(obj);
                    sb.Append($"{new string(' ', indent * 2)}{prop.Name}: ");
                    Append(propValue, indent);
                }
            }
        }
    }
}

abstract class TypeConverter {
    public abstract string ConvertFromString(object value);
}

record class Person(string Name, int Age, Data Data);
record class Data(int Id);
