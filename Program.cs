using System.Diagnostics.CodeAnalysis;
using System.Text;
using Framework;
using DataModel;

var person = new Person("John", 30, new Data(1));
var str = TypeDescriptor.GetConverter<Person>().ConvertToString(person);
Console.WriteLine(str);
