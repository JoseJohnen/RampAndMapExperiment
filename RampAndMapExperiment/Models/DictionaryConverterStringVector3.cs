using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;

namespace RampAndMapExperiment.Models
{
    public class DictionaryConverterStringVector3 : System.Text.Json.Serialization.JsonConverter<Dictionary<string, Vector3>>
    {
        public override Dictionary<string, Vector3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string strJson = string.Empty;
            try
            {
                strJson = reader.GetString();
                //JsonDocument jsonDoc = JsonDocument.ParseValue(ref reader);
                //strJson = jsonDoc.RootElement.GetRawText();

                //if it comes empty, in one way or another
                if(string.IsNullOrWhiteSpace(strJson) || strJson == "{}")
                {
                    return new Dictionary<string, Vector3>();
                }

                string[] strArray = strJson.Replace("\", \"", "\",\"").Replace("}\"}","").Split("}\",\"");
                strArray[0] = strArray[0].Substring(strArray[0].IndexOf("\"")+1);

                JsonSerializerOptions serializeOptions = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new Vector3Converter()
                    },
                };
                string[] strArrayItem = new string[2];
                Dictionary<string, Vector3> result_dictionary = new Dictionary<string, Vector3>();
                string af = string.Empty;
                foreach (string item in strArray)
                {
                    strArrayItem = item.Split(":",2);
                    af = strArrayItem[1] +"}\"";
                    Vector3 v3 = JsonSerializer.Deserialize<Vector3>(af, serializeOptions);
                    result_dictionary.Add(strArrayItem[0].Replace("\"", ""),v3);
                }
                return result_dictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: (DictionaryConverterStringVector3) Read(): {0} Message: {1}", strJson, ex.Message);
                return default;
            }
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, Vector3> dic_str_vector3, JsonSerializerOptions options)
        {
            try
            {
                int i = 0;
                JsonSerializerOptions serializeOptions = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new Vector3Converter()
                    },
                };
                string strJson = "{";
                foreach (KeyValuePair<string, Vector3> item in dic_str_vector3)
                {
                    strJson += "\"" + item.Key + "\":"; //{";
                    strJson += JsonSerializer.Serialize(item.Value, serializeOptions);
                    //strJson += "}";
                    if (i < (dic_str_vector3.Count - 1))
                    {
                        strJson += ", ";
                    }
                    i++;
                }
                strJson += "}";
                writer.WriteStringValue(strJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: (DictionaryConverterStringVector3) Write(): " + ex.Message);
            }
        }
    }

}
