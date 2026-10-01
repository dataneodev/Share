using System.Xml.Linq;

namespace Vero.Shared.Translate
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TranslatorAttribute : Attribute
    {
        public Type Translator;
        public string Name;

        public TranslatorAttribute(Type translator)
        {
            Translator = translator;
            Name = "Name";
        }
    }
}
