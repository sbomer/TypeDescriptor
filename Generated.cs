using Framework;
using DataModel;

namespace DataModel {
    partial record class Person : ITypeDescriptionProvider<Person> {
        public static ICustomTypeDescriptor GetTypeDescriptor() => new PersonTypeDescriptor();
    }
}

class PersonTypeDescriptor : ICustomTypeDescriptor {
    public string GetClassName() => "Person";
    public PropertyDescriptorCollection Properties => new([
        new NameMetadata(),
        new AgeMetadata(),
        new DataMetadata()
    ]);

    class NameMetadata : IPropertyDescriptor {
        public string Name => "Name";
        public ICustomTypeDescriptor Type => new StringTypeDescriptor();
        public Func<object, object> Getter => p => ((Person) p).Name;
    }

    class AgeMetadata : IPropertyDescriptor {
        public string Name => "Age";
        public  ICustomTypeDescriptor Type => new IntTypeDescriptor();
        public Func<object, object> Getter => p => ((Person) p).Age;
    }

    class DataMetadata : IPropertyDescriptor {
        public string Name => "Data";
        public ICustomTypeDescriptor Type => new DataTypeDescriptor();
        public Func<object, object> Getter => p => ((Person) p).Data;
    }
}

class DataTypeDescriptor : ICustomTypeDescriptor {
    public string GetClassName() => "Data";
    public PropertyDescriptorCollection Properties => new([
        new IdMetadata()
    ]);
    class IdMetadata : IPropertyDescriptor {
        public string Name => "Id";
        public ICustomTypeDescriptor Type => new IntTypeDescriptor();
        public Func<object, object> Getter => d => ((Data) d).Id;
    }
}
