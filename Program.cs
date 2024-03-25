using System.Diagnostics.CodeAnalysis;
using System.Text;

var person = new Person("John", 30, new Data(1));
var str = TypeDescriptor.GetConverter<Person>().ConvertFromString(person);
Console.WriteLine(str);

class TypeDescriptor {
    static HashSet<Type> knownTypes = new();

    public static TypeConverter GetConverter<T>() where T : IMetadataProvider<T> {
        return new StronglyTypedConverter<T>(T.GetMetadata());
    }
}

abstract class TypeConverter {
    public abstract string ConvertFromString(object value);
}

class StronglyTypedConverter<T> : TypeConverter {
    class StronglyTypedVisitor : IVisitor
    {
        public StringBuilder sb = new();
        int indent = 0;
        public void Visit<TType>(ITypeMetadata<TType> typeMetadata, TType value)
        {
            var properties = typeMetadata.Properties;
            if (properties.Count() == 0) {
                if (value is null) {
                    sb.AppendLine("null");
                    return;
                }
                sb.AppendLine(value.ToString() + " (" + typeMetadata.Name + ")");
                return;
            }
            
            sb.AppendLine(typeMetadata.Name);
            indent++;
            foreach (var prop in properties)
                prop.Accept(this, value);
            indent--;

        }

        public void Visit<TType, TPropertyType>(IPropertyMetadata<TType, TPropertyType> propertyMetadata, TType value)
        {
            sb.Append(new string(' ', indent * 2) + propertyMetadata.Name + ": ");
            var v = propertyMetadata.Getter(value);
            propertyMetadata.Type.Accept(this, v);
        }
    }

    StronglyTypedVisitor visitor = new();
    ITypeMetadata<T> metadata;
    public StronglyTypedConverter(ITypeMetadata<T> metadata) => this.metadata = metadata;

    public override string ConvertFromString(object value)
    {
        metadata.Accept(visitor, (T)value);
        return visitor.sb.ToString();
    }
}

class PersonTypeMetadata : ITypeMetadata<Person> {
    public string Name => "Person";
    public IEnumerable<IPropertyMetadata<Person>> Properties => new List<IPropertyMetadata<Person>> {
        new NameMetadata(),
        new AgeMetadata(),
        new DataMetadata()
    };

    class NameMetadata : IPropertyMetadata<Person, string> {
        public string Name => "Name";
        public ITypeMetadata<string> Type => new StringTypeMetadata();
        public Func<Person, string> Getter => p => p.Name;
    }

    class AgeMetadata : IPropertyMetadata<Person, int> {
        public string Name => "Age";
        public  ITypeMetadata<int> Type => new IntTypeMetadata();
        public Func<Person, int> Getter => p => p.Age;
    }

    class DataMetadata : IPropertyMetadata<Person, Data> {
        public string Name => "Data";
        public ITypeMetadata<Data> Type => new DataTypeMetadata();
        public Func<Person, Data> Getter => p => p.Data;
    }
}

class StringTypeMetadata : ITypeMetadata<string> {
    public string Name => "string";
    public IEnumerable<IPropertyMetadata<string>> Properties => new List<IPropertyMetadata<string>>();
}
class IntTypeMetadata : ITypeMetadata<int> {
    public string Name => "int";
    public IEnumerable<IPropertyMetadata<int>> Properties => new List<IPropertyMetadata<int>>();
}
class DataTypeMetadata : ITypeMetadata<Data> {
    public string Name => "Data";
    public IEnumerable<IPropertyMetadata<Data>> Properties => new List<IPropertyMetadata<Data>> {
        new IdMetadata()
    };

    class IdMetadata : IPropertyMetadata<Data, int> {
        public string Name => "Id";
        public ITypeMetadata<int> Type => new IntTypeMetadata();
        public Func<Data, int> Getter => d => d.Id;
    }
}

record class Person(string Name, int Age, Data Data) : IMetadataProvider<Person> {
    public static ITypeMetadata<Person> GetMetadata() => new PersonTypeMetadata();
}
record class Data(int Id);

interface ITypeMetadata<T> {
    string Name { get; }
    IEnumerable<IPropertyMetadata<T>> Properties { get; }
    void Accept(IVisitor visitor, T value) => visitor.Visit(this, value);
}

interface IPropertyMetadata<TType> {
    string Name { get; }  
    void Accept(IVisitor visitor, TType value);
}

interface IPropertyMetadata<TType, TPropertyType> : IPropertyMetadata<TType> {
    
    Func<TType, TPropertyType> Getter { get; }
    ITypeMetadata<TPropertyType> Type { get; }
    void IPropertyMetadata<TType>.Accept(IVisitor visitor, TType value) => visitor.Visit(this, value);
}

interface IVisitor {
    void Visit<TType>(ITypeMetadata<TType> typeMetadata, TType value);
    void Visit<TType, TPropertyType>(IPropertyMetadata<TType, TPropertyType> propertyMetadata, TType value);
}

interface IMetadataProvider<T> {
    static abstract ITypeMetadata<T> GetMetadata();
}
