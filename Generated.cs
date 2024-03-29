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
    public TypeConverter GetConverter() => new TypeDescriptorVisitorConverter(this);

    class NameMetadata : PropertyDescriptor {
        public override string Name => "Name";
        public override ICustomTypeDescriptor PropertyType => new StringTypeDescriptor();
        public override object GetValue(object instance) => ((Person) instance).Name;
    }

    class AgeMetadata : PropertyDescriptor {
        public override string Name => "Age";
        public override ICustomTypeDescriptor PropertyType => new IntTypeDescriptor();
        public override object GetValue(object instance) => ((Person) instance).Age;
    }

    class DataMetadata : PropertyDescriptor {
        public override string Name => "Data";
        public override ICustomTypeDescriptor PropertyType => new DataTypeDescriptor();
        public override object GetValue(object instance) => ((Person) instance).Data;
    }
}

class DataTypeDescriptor : ICustomTypeDescriptor {
    public string GetClassName() => "Data";
    public PropertyDescriptorCollection Properties => new([
        new IdMetadata()
    ]);
    public TypeConverter GetConverter() => new TypeDescriptorVisitorConverter(this);
    class IdMetadata : PropertyDescriptor {
        public override string Name => "Id";
        public override ICustomTypeDescriptor PropertyType => new IntTypeDescriptor();
        public override object GetValue(object instance) => ((Data) instance).Id;
    }
}
