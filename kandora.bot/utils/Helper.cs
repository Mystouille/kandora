using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace kandora.bot.utils;

public static class Helper
{
    public static string[] ToStringArray(this int[] stringArray)
    {
        return stringArray.Select(_ => _.ToString()).ToArray();
    }

    public static T[] ToArray<T>(this JsonArray intArray)
    {
        return intArray.Select(_ => _.GetValue<T>()).ToArray();
    }

    public static string[] ToStringArray(this JsonArray stringArrayWithNumbers)
    {
        var stringList = new List<string>();
        for(int i = 0; i < stringArrayWithNumbers.Count; i++)
        {
            string result;
            bool value = stringArrayWithNumbers[i].AsValue().TryGetValue<string>(out result);
            if (!value)
                result = stringArrayWithNumbers[i].GetValue<float>().ToString();

            stringList.Add(result);
        }
        return stringList.ToArray();
    }

    public static JsonNode SelectByPath(this JsonNode node, string path)
    {
        if (node.GetPath() == path) return node;

        if (node is JsonArray)
        {
            JsonArray array = (JsonArray)node;
            for (int i = 0; i < array.Count; i++)
            {
                JsonNode result = node[i].SelectByPath(path);
                if (result != null) return result;
            }
        }
        if (node is JsonObject)
        {
            JsonObject array = (JsonObject)node;
            foreach (KeyValuePair<string, JsonNode> result in array)
            {
                JsonNode nodeResult = result.Value.SelectByPath(path);
                if (nodeResult != null) return nodeResult;
            }
        }

        return null;
    }
}
