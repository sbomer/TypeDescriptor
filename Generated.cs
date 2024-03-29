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

    class NameMetadata : PropertyDescriptor {
        public override string Name => "Name";
        public override ICustomTypeDescriptor Type => new StringTypeDescriptor();
        public override Func<object, object> Getter => p => ((Person) p).Name;
    }

    class AgeMetadata : PropertyDescriptor {
        public override string Name => "Age";
        public override ICustomTypeDescriptor Type => new IntTypeDescriptor();
        public override Func<object, object> Getter => p => ((Person) p).Age;
    }

    class DataMetadata : PropertyDescriptor {
        public override string Name => "Data";
        public override ICustomTypeDescriptor Type => new DataTypeDescriptor();
        public override Func<object, object> Getter => p => ((Person) p).Data;
    }
}

class DataTypeDescriptor : ICustomTypeDescriptor {
    public string GetClassName() => "Data";
    public PropertyDescriptorCollection Properties => new([
        new IdMetadata()
    ]);
    class IdMetadata : PropertyDescriptor {
        public override string Name => "Id";
        public override ICustomTypeDescriptor Type => new IntTypeDescriptor();
        public override Func<object, object> Getter => d => ((Data) d).Id;
    }
}
