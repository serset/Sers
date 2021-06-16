using System;

namespace Vit.Extensions
{
    public static partial class AttributeExtensions
    {
        public static T GetAttribute<T>(this Func<Type, System.Attribute> GetAttribute)
          where T : System.Attribute
        {
            return GetAttribute(typeof(T)) as T;
        }

    }
}
