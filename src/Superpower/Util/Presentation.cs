using System.ComponentModel;
using System.Reflection;

namespace Superpower.Util
{
    static class Presentation
    {
        public static string FormatKind<TTokenKind>(TTokenKind kind)
        {
            var kindTypeInfo = typeof (TTokenKind).GetTypeInfo();
            if (kindTypeInfo.IsEnum)
            {
                var field = kindTypeInfo.GetDeclaredField(kind.ToString());
                if (field != null)
                {
                    var description = field.GetCustomAttribute<DescriptionAttribute>();
                    if (description != null)
                        return description.Description;
                }

                return kind.ToString().ToLower();
            }

            return kind.ToString().ToLower();
        }
    }
}