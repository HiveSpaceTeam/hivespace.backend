using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HiveSpace.Application.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        // Get the type of the enum
        var enumType = enumValue.GetType();

        // Get the member information for the enum value
        var memberInfo = enumType.GetMember(enumValue.ToString());

        if (memberInfo.Length > 0)
        {
            // Get the Display attribute, if it exists
            var displayAttribute = memberInfo[0].GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                return displayAttribute.Name ?? enumValue.ToString();
            }
        }

        // Fallback to the enum value's name if no Display attribute is present
        return enumValue.ToString();
    }
}