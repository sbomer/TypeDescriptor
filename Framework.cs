using System;
using System.Text;

namespace Framework;
interface ITypeDescriptor<T> {
    public abstract string Name { get; }
    public abstract IEnumerable<IPropertyDescriptor<T>> Properties { get; }
    public virtual void Accept(IVisitor visitor, T value) => visitor.Visit(this, value);
}

interface IPropertyDescriptor<TType> {
    string Name { get; }  
    void Accept(IVisitor visitor, TType value);
}

interface IPropertyDescriptor<TType, TPropertyType> : IPropertyDescriptor<TType> {
    Func<TType, TPropertyType> Getter { get; }
    ITypeDescriptor<TPropertyType> Type { get; }
    void IPropertyDescriptor<TType>.Accept(IVisitor visitor, TType value) => visitor.Visit(this, value);
}

// Primitive type descriptors
class StringTypeDescriptor : ITypeDescriptor<string> {
    public string Name => "string";
    public IEnumerable<IPropertyDescriptor<string>> Properties => new List<IPropertyDescriptor<string>>();
}
class IntTypeDescriptor : ITypeDescriptor<int> {
    public string Name => "int";
    public IEnumerable<IPropertyDescriptor<int>> Properties => new List<IPropertyDescriptor<int>>();
}

interface ITypeDescriptionProvider<T> {
    public virtual static ITypeDescriptor<T> GetTypeDescriptor() => new EmptyCustomTypeDescriptor();

    private sealed class EmptyCustomTypeDescriptor : ITypeDescriptor<T> {
        public string Name => typeof(T).Name;
        public IEnumerable<IPropertyDescriptor<T>> Properties => new List<IPropertyDescriptor<T>>();
    }
}

class TypeDescriptor {
    public static TypeConverter GetConverter<T>() where T : ITypeDescriptionProvider<T> {
        return new StronglyTypedConverter<T>(T.GetTypeDescriptor());
    }
}

class StronglyTypedConverter<T> : TypeConverter {
    class StronglyTypedVisitor : IVisitor
    {
        public StringBuilder sb = new();
        int indent = 0;
        public void Visit<TType>(ITypeDescriptor<TType> typeMetadata, TType value)
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

        public void Visit<TType, TPropertyType>(IPropertyDescriptor<TType, TPropertyType> propertyMetadata, TType value)
        {
            sb.Append(new string(' ', indent * 2) + propertyMetadata.Name + ": ");
            var v = propertyMetadata.Getter(value);
            propertyMetadata.Type.Accept(this, v);
        }
    }

    StronglyTypedVisitor visitor = new();
    ITypeDescriptor<T> metadata;
    public StronglyTypedConverter(ITypeDescriptor<T> metadata) => this.metadata = metadata;

    public override string ConvertToString(object value)
    {
        metadata.Accept(visitor, (T)value);
        return visitor.sb.ToString();
    }
}

abstract class TypeConverter {
    public abstract string ConvertToString(object value);
}

interface IVisitor {
    void Visit<TType>(ITypeDescriptor<TType> typeMetadata, TType value);
    void Visit<TType, TPropertyType>(IPropertyDescriptor<TType, TPropertyType> propertyMetadata, TType value);
}
