using System.Diagnostics.CodeAnalysis;
using System.Text;
using Framework;
using DataModel;

var provider = new PersonTypeDescriptionProvider();
TypeDescriptor.AddProvider(provider, typeof(Person));

var person = new Person("John", 30, new Data(1));
var str = TypeDescriptor.GetConverter(typeof(Person)).ConvertToString(person);
Console.WriteLine(str);

class PersonTypeDescriptionProvider : TypeDescriptionProvider {
    public override ICustomTypeDescriptor GetTypeDescriptor() => new PersonTypeDescriptor();
}
