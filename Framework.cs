using System;
using System.Text;

namespace Framework;
interface ITypeDescriptor {
    public abstract string Name { get; }
    public abstract IEnumerable<IPropertyDescriptor> Properties { get; }
    public virtual void Accept(IVisitor visitor, object value) => visitor.Visit(this, value);
}

interface IPropertyDescriptor {
    string Name { get; }  
    void Accept(IVisitor visitor, object value) => visitor.Visit(this, value);
    Func<object, object> Getter { get; }
    ITypeDescriptor Type { get; }
}

// Primitive type descriptors
class StringTypeDescriptor : ITypeDescriptor {
    public string Name => "string";
    public IEnumerable<IPropertyDescriptor> Properties => new List<IPropertyDescriptor>();
}
class IntTypeDescriptor : ITypeDescriptor {
    public string Name => "int";
    public IEnumerable<IPropertyDescriptor> Properties => new List<IPropertyDescriptor>();
}

interface ITypeDescriptionProvider<T> {
    public virtual static ITypeDescriptor GetTypeDescriptor() => new EmptyCustomTypeDescriptor();

    private sealed class EmptyCustomTypeDescriptor : ITypeDescriptor {
        public string Name => typeof(T).Name;
    public IEnumerable<IPropertyDescriptor> Properties => new List<IPropertyDescriptor>();
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
        public void Visit(ITypeDescriptor typeMetadata, object value)
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

        public void Visit(IPropertyDescriptor propertyMetadata, object value)
        {
            sb.Append(new string(' ', indent * 2) + propertyMetadata.Name + ": ");
            var v = propertyMetadata.Getter(value);
            propertyMetadata.Type.Accept(this, v);
        }
    }

    StronglyTypedVisitor visitor = new();
    ITypeDescriptor metadata;
    public StronglyTypedConverter(ITypeDescriptor metadata) => this.metadata = metadata;

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
    void Visit(ITypeDescriptor typeMetadata, object value);
    void Visit(IPropertyDescriptor propertyMetadata, object value);
}
