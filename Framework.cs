using System;
using System.Collections;
using System.Text;

namespace Framework;
interface ICustomTypeDescriptor {
    string GetClassName();
    PropertyDescriptorCollection Properties { get; }
}

abstract class PropertyDescriptor {
    public abstract string Name { get; } 
    public abstract object GetValue(object instance);
    public abstract ICustomTypeDescriptor Type { get; }
}

class PropertyDescriptorCollection : ICollection {
    private PropertyDescriptor[] properties;
    public int Count { get; private set; }
    public PropertyDescriptorCollection(PropertyDescriptor[] properties) {
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
    class StronglyTypedVisitor
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
            foreach (PropertyDescriptor prop in properties)
                Visit(prop, value);
            indent--;

        }

        public void Visit(PropertyDescriptor propertyMetadata, object instance)
        {
            sb.Append(new string(' ', indent * 2) + propertyMetadata.Name + ": ");
            var value = propertyMetadata.GetValue(instance);
            Visit(propertyMetadata.Type, value);
        }
    }

    StronglyTypedVisitor visitor = new();
    ICustomTypeDescriptor metadata;
    public StronglyTypedConverter(ICustomTypeDescriptor metadata) => this.metadata = metadata;

    public override string ConvertToString(object value)
    {
        visitor.Visit(metadata, value);
        return visitor.sb.ToString();
    }
}

abstract class TypeConverter {
    public abstract string ConvertToString(object value);
}
