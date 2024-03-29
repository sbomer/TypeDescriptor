using Framework;
using DataModel;

namespace DataModel {
    partial record class Person : ITypeDescriptionProvider<Person> {
        public static ITypeDescriptor GetTypeDescriptor() => new PersonTypeDescriptor();
    }
}

class PersonTypeDescriptor : ITypeDescriptor {
    public string Name => "Person";
    public IEnumerable<IPropertyDescriptor> Properties => new List<IPropertyDescriptor> {
        new NameMetadata(),
        new AgeMetadata(),
        new DataMetadata()
    };

    class NameMetadata : IPropertyDescriptor {
        public string Name => "Name";
        public ITypeDescriptor Type => new StringTypeDescriptor();
        public Func<object, object> Getter => p => ((Person) p).Name;
    }

    class AgeMetadata : IPropertyDescriptor {
        public string Name => "Age";
        public  ITypeDescriptor Type => new IntTypeDescriptor();
        public Func<object, object> Getter => p => ((Person) p).Age;
    }

    class DataMetadata : IPropertyDescriptor {
        public string Name => "Data";
        public ITypeDescriptor Type => new DataTypeDescriptor();
        public Func<object, object> Getter => p => ((Person) p).Data;
    }
}

class DataTypeDescriptor : ITypeDescriptor {
    public string Name => "Data";
    public IEnumerable<IPropertyDescriptor> Properties => new List<IPropertyDescriptor> {
        new IdMetadata()
    };

    class IdMetadata : IPropertyDescriptor {
        public string Name => "Id";
        public ITypeDescriptor Type => new IntTypeDescriptor();
        public Func<object, object> Getter => d => ((Data) d).Id;
    }
}
