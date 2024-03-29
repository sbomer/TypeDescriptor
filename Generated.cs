using Framework;
using DataModel;

namespace DataModel {
    partial record class Person {
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
    public TypeConverter GetConverter() => new StronglyTypedConverter<Person>(this);

    class NameMetadata : PropertyDescriptor {
        public override string Name => "Name";
        public override ICustomTypeDescriptor Type => new StringTypeDescriptor();
        public override object GetValue(object instance) => ((Person) instance).Name;
    }

    class AgeMetadata : PropertyDescriptor {
        public override string Name => "Age";
        public override ICustomTypeDescriptor Type => new IntTypeDescriptor();
        public override object GetValue(object instance) => ((Person) instance).Age;
    }

    class DataMetadata : PropertyDescriptor {
        public override string Name => "Data";
        public override ICustomTypeDescriptor Type => new DataTypeDescriptor();
        public override object GetValue(object instance) => ((Person) instance).Data;
    }
}

class DataTypeDescriptor : ICustomTypeDescriptor {
    public string GetClassName() => "Data";
    public PropertyDescriptorCollection Properties => new([
        new IdMetadata()
    ]);
    public TypeConverter GetConverter() => new StronglyTypedConverter<Data>(this);
    class IdMetadata : PropertyDescriptor {
        public override string Name => "Id";
        public override ICustomTypeDescriptor Type => new IntTypeDescriptor();
        public override object GetValue(object instance) => ((Data) instance).Id;
    }
}
