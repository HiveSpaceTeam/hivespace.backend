using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HiveSpace.Common.Utilities
{
    public static class CommonFunction
    {
        public static object ConvertToActualType(object value)
        {
            if (value is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.Number:
                        if (jsonElement.TryGetInt64(out long longValue)) return longValue;
                        if (jsonElement.TryGetDouble(out double doubleValue)) return doubleValue;
                        break;
                    case JsonValueKind.String:
                        string strValue = jsonElement.GetString();
                        if (DateTime.TryParse(strValue, out DateTime dateValue)) return dateValue;
                        return strValue;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return jsonElement.GetBoolean();
                    case JsonValueKind.Null:
                        return null;
                }
                return jsonElement;
            }

            // Nếu không phải JsonElement, trả về giá trị gốc
            return value;
        }


    }
}
