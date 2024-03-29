using System;
using System.Collections;
using System.Text;

namespace Framework;
interface ICustomTypeDescriptor {
    string GetClassName();
    PropertyDescriptorCollection Properties { get; }
    virtual void Accept(IVisitor visitor, object value) => visitor.Visit(this, value);
}

interface IPropertyDescriptor {
    string Name { get; }  
    void Accept(IVisitor visitor, object value) => visitor.Visit(this, value);
    Func<object, object> Getter { get; }
    ICustomTypeDescriptor Type { get; }
}

class PropertyDescriptorCollection : ICollection {
    private IPropertyDescriptor[] properties;
    public int Count { get; private set; }
    public PropertyDescriptorCollection(IPropertyDescriptor[] properties) {
        this.properties = properties;
        Count = properties.Length;
    }
    // ICollection implementation
    public void CopyTo(Array array, int index)
    {
        Array.Copy(properties, 0, array, index, Count);
    }
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => null!;
    // IEnumerable implementation
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator GetEnumerator() {
        return properties.GetEnumerator();
    }
}

// Primitive type descriptors
class StringTypeDescriptor : ICustomTypeDescriptor {
    public string GetClassName() => "string";
    public PropertyDescriptorCollection Properties => new([]);
}
class IntTypeDescriptor : ICustomTypeDescriptor {
    public string GetClassName() => "int";
    public PropertyDescriptorCollection Properties => new([]);
}

interface ITypeDescriptionProvider<T> {
    public virtual static ICustomTypeDescriptor GetTypeDescriptor() => new EmptyCustomTypeDescriptor();

    private sealed class EmptyCustomTypeDescriptor : ICustomTypeDescriptor {
        public string GetClassName() => typeof(T).Name;
    public PropertyDescriptorCollection Properties => new([]);
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
        public void Visit(ICustomTypeDescriptor typeMetadata, object value)
        {
            var properties = typeMetadata.Properties;
            if (properties.Count == 0) {
                if (value is null) {
                    sb.AppendLine("null");
                    return;
                }
                sb.AppendLine(value.ToString() + " (" + typeMetadata.GetClassName() + ")");
                return;
            }
            
            sb.AppendLine(typeMetadata.GetClassName());
            indent++;
            foreach (IPropertyDescriptor prop in properties)
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
    ICustomTypeDescriptor metadata;
    public StronglyTypedConverter(ICustomTypeDescriptor metadata) => this.metadata = metadata;

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
    void Visit(ICustomTypeDescriptor typeMetadata, object value);
    void Visit(IPropertyDescriptor propertyMetadata, object value);
}
