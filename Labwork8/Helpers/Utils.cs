// Utils.cs (Helpers klasöründe)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Labwork5.Helpers
{
    public sealed class Utils
    {
        private static readonly Utils _instance = new Utils();
        private Utils() { }
        public static Utils Instance => _instance;

        public string ExportToJson<T>(List<T> data, List<string>? selectedColumns = null)
        {
            if (selectedColumns == null || selectedColumns.Count == 0)
            {
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }

            var filteredData = data.Select(item =>
            {
                var obj = new Dictionary<string, object?>();
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (selectedColumns.Contains(prop.Name))
                    {
                        obj[prop.Name] = prop.GetValue(item);
                    }
                }
                return obj;
            }).ToList();

            return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}