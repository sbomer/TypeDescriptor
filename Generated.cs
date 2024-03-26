using Framework;
using DataModel;

namespace DataModel {
    partial record class Person : ITypeDescriptionProvider<Person> {
        public static ITypeDescriptor<Person> GetTypeDescriptor() => new PersonTypeDescriptor();
    }
}

class PersonTypeDescriptor : ITypeDescriptor<Person> {
    public string Name => "Person";
    public IEnumerable<IPropertyDescriptor<Person>> Properties => new List<IPropertyDescriptor<Person>> {
        new NameMetadata(),
        new AgeMetadata(),
        new DataMetadata()
    };

    class NameMetadata : IPropertyDescriptor<Person, string> {
        public string Name => "Name";
        public ITypeDescriptor<string> Type => new StringTypeDescriptor();
        public Func<Person, string> Getter => p => p.Name;
    }

    class AgeMetadata : IPropertyDescriptor<Person, int> {
        public string Name => "Age";
        public  ITypeDescriptor<int> Type => new IntTypeDescriptor();
        public Func<Person, int> Getter => p => p.Age;
    }

    class DataMetadata : IPropertyDescriptor<Person, Data> {
        public string Name => "Data";
        public ITypeDescriptor<Data> Type => new DataTypeDescriptor();
        public Func<Person, Data> Getter => p => p.Data;
    }
}

class DataTypeDescriptor : ITypeDescriptor<Data> {
    public string Name => "Data";
    public IEnumerable<IPropertyDescriptor<Data>> Properties => new List<IPropertyDescriptor<Data>> {
        new IdMetadata()
    };

    class IdMetadata : IPropertyDescriptor<Data, int> {
        public string Name => "Id";
        public ITypeDescriptor<int> Type => new IntTypeDescriptor();
        public Func<Data, int> Getter => d => d.Id;
    }
}
