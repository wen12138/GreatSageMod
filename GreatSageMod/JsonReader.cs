using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GreatSageMod
{
    public static class JSONParser
    {
        public static T FromJson<T>(this string json)
        {
            bool flag = JSONParser.propertyInfoCache == null;
            if (flag)
            {
                JSONParser.propertyInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            }
            bool flag2 = JSONParser.fieldInfoCache == null;
            if (flag2)
            {
                JSONParser.fieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            }
            bool flag3 = JSONParser.stringBuilder == null;
            if (flag3)
            {
                JSONParser.stringBuilder = new StringBuilder();
            }
            bool flag4 = JSONParser.splitArrayPool == null;
            if (flag4)
            {
                JSONParser.splitArrayPool = new Stack<List<string>>();
            }
            JSONParser.stringBuilder.Length = 0;
            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
                bool flag5 = c == '"';
                if (flag5)
                {
                    i = JSONParser.AppendUntilStringEnd(true, i, json);
                }
                else
                {
                    bool flag6 = char.IsWhiteSpace(c);
                    if (!flag6)
                    {
                        JSONParser.stringBuilder.Append(c);
                    }
                }
            }
            return (T)((object)JSONParser.ParseValue(typeof(T), JSONParser.stringBuilder.ToString()));
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002144 File Offset: 0x00000344
        private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
        {
            JSONParser.stringBuilder.Append(json[startIdx]);
            for (int i = startIdx + 1; i < json.Length; i++)
            {
                bool flag = json[i] == '\\';
                if (flag)
                {
                    if (appendEscapeCharacter)
                    {
                        JSONParser.stringBuilder.Append(json[i]);
                    }
                    JSONParser.stringBuilder.Append(json[i + 1]);
                    i++;
                }
                else
                {
                    bool flag2 = json[i] == '"';
                    if (flag2)
                    {
                        JSONParser.stringBuilder.Append(json[i]);
                        return i;
                    }
                    JSONParser.stringBuilder.Append(json[i]);
                }
            }
            return json.Length - 1;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002210 File Offset: 0x00000410
        private static List<string> Split(string json)
        {
            List<string> list = (JSONParser.splitArrayPool.Count > 0) ? JSONParser.splitArrayPool.Pop() : new List<string>();
            list.Clear();
            bool flag = json.Length == 2;
            List<string> result;
            if (flag)
            {
                result = list;
            }
            else
            {
                int num = 0;
                JSONParser.stringBuilder.Length = 0;
                int i = 1;
                while (i < json.Length - 1)
                {
                    char c = json[i];
                    char c2 = c;
                    if (c2 > ':')
                    {
                        if (c2 <= ']')
                        {
                            if (c2 != '[')
                            {
                                if (c2 != ']')
                                {
                                    goto IL_E9;
                                }
                                goto IL_AA;
                            }
                        }
                        else if (c2 != '{')
                        {
                            if (c2 != '}')
                            {
                                goto IL_E9;
                            }
                            goto IL_AA;
                        }
                        num++;
                        goto IL_E9;
                    IL_AA:
                        num--;
                        goto IL_E9;
                    }
                    if (c2 != '"')
                    {
                        if (c2 != ',' && c2 != ':')
                        {
                            goto IL_E9;
                        }
                        bool flag2 = num == 0;
                        if (!flag2)
                        {
                            goto IL_E9;
                        }
                        list.Add(JSONParser.stringBuilder.ToString());
                        JSONParser.stringBuilder.Length = 0;
                    }
                    else
                    {
                        i = JSONParser.AppendUntilStringEnd(true, i, json);
                    }
                IL_FD:
                    i++;
                    continue;
                IL_E9:
                    JSONParser.stringBuilder.Append(json[i]);
                    goto IL_FD;
                }
                list.Add(JSONParser.stringBuilder.ToString());
                result = list;
            }
            return result;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x0000234C File Offset: 0x0000054C
        internal static object ParseValue(Type type, string json)
        {
            bool flag = type == typeof(string);
            object result;
            if (flag)
            {
                bool flag2 = json.Length <= 2;
                if (flag2)
                {
                    result = string.Empty;
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder(json.Length);
                    int i = 1;
                    while (i < json.Length - 1)
                    {
                        bool flag3 = json[i] == '\\' && i + 1 < json.Length - 1;
                        if (!flag3)
                        {
                            goto IL_10C;
                        }
                        int num = "\"\\nrtbf/".IndexOf(json[i + 1]);
                        bool flag4 = num >= 0;
                        if (!flag4)
                        {
                            bool flag5 = json[i + 1] == 'u' && i + 5 < json.Length - 1;
                            if (flag5)
                            {
                                uint num2 = 0U;
                                bool flag6 = uint.TryParse(json.Substring(i + 2, 4), NumberStyles.AllowHexSpecifier, null, out num2);
                                if (flag6)
                                {
                                    stringBuilder.Append((char)num2);
                                    i += 5;
                                    goto IL_11C;
                                }
                            }
                            goto IL_10C;
                        }
                        stringBuilder.Append("\"\\\n\r\t\b\f/"[num]);
                        i++;
                    IL_11C:
                        i++;
                        continue;
                    IL_10C:
                        stringBuilder.Append(json[i]);
                        goto IL_11C;
                    }
                    result = stringBuilder.ToString();
                }
            }
            else
            {
                bool isPrimitive = type.IsPrimitive;
                if (isPrimitive)
                {
                    object obj = Convert.ChangeType(json, type, CultureInfo.InvariantCulture);
                    result = obj;
                }
                else
                {
                    bool flag7 = type == typeof(decimal);
                    if (flag7)
                    {
                        decimal num3;
                        decimal.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out num3);
                        result = num3;
                    }
                    else
                    {
                        bool flag8 = type == typeof(DateTime);
                        if (flag8)
                        {
                            DateTime dateTime;
                            DateTime.TryParse(json.Replace("\"", ""), CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
                            result = dateTime;
                        }
                        else
                        {
                            bool flag9 = json == "null";
                            if (flag9)
                            {
                                result = null;
                            }
                            else
                            {
                                bool isEnum = type.IsEnum;
                                if (isEnum)
                                {
                                    bool flag10 = json[0] == '"';
                                    if (flag10)
                                    {
                                        json = json.Substring(1, json.Length - 2);
                                    }
                                    try
                                    {
                                        return Enum.Parse(type, json, false);
                                    }
                                    catch
                                    {
                                        return 0;
                                    }
                                }
                                bool isArray = type.IsArray;
                                if (isArray)
                                {
                                    Type elementType = type.GetElementType();
                                    bool flag11 = json[0] != '[' || json[json.Length - 1] != ']';
                                    if (flag11)
                                    {
                                        result = null;
                                    }
                                    else
                                    {
                                        List<string> list = JSONParser.Split(json);
                                        Array array = Array.CreateInstance(elementType, list.Count);
                                        for (int j = 0; j < list.Count; j++)
                                        {
                                            array.SetValue(JSONParser.ParseValue(elementType, list[j]), j);
                                        }
                                        JSONParser.splitArrayPool.Push(list);
                                        result = array;
                                    }
                                }
                                else
                                {
                                    bool flag12 = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
                                    if (flag12)
                                    {
                                        Type type2 = type.GetGenericArguments()[0];
                                        bool flag13 = json[0] != '[' || json[json.Length - 1] != ']';
                                        if (flag13)
                                        {
                                            result = null;
                                        }
                                        else
                                        {
                                            List<string> list2 = JSONParser.Split(json);
                                            IList list3 = (IList)type.GetConstructor(new Type[]
                                            {
                                                typeof(int)
                                            }).Invoke(new object[]
                                            {
                                                list2.Count
                                            });
                                            for (int k = 0; k < list2.Count; k++)
                                            {
                                                list3.Add(JSONParser.ParseValue(type2, list2[k]));
                                            }
                                            JSONParser.splitArrayPool.Push(list2);
                                            result = list3;
                                        }
                                    }
                                    else
                                    {
                                        bool flag14 = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
                                        if (flag14)
                                        {
                                            Type[] genericArguments = type.GetGenericArguments();
                                            Type left = genericArguments[0];
                                            Type type3 = genericArguments[1];
                                            bool flag15 = left != typeof(string);
                                            if (flag15)
                                            {
                                                result = null;
                                            }
                                            else
                                            {
                                                bool flag16 = json[0] != '{' || json[json.Length - 1] != '}';
                                                if (flag16)
                                                {
                                                    result = null;
                                                }
                                                else
                                                {
                                                    List<string> list4 = JSONParser.Split(json);
                                                    bool flag17 = list4.Count % 2 != 0;
                                                    if (flag17)
                                                    {
                                                        result = null;
                                                    }
                                                    else
                                                    {
                                                        IDictionary dictionary = (IDictionary)type.GetConstructor(new Type[]
                                                        {
                                                            typeof(int)
                                                        }).Invoke(new object[]
                                                        {
                                                            list4.Count / 2
                                                        });
                                                        for (int l = 0; l < list4.Count; l += 2)
                                                        {
                                                            bool flag18 = list4[l].Length <= 2;
                                                            if (!flag18)
                                                            {
                                                                string key = list4[l].Substring(1, list4[l].Length - 2);
                                                                object value = JSONParser.ParseValue(type3, list4[l + 1]);
                                                                dictionary[key] = value;
                                                            }
                                                        }
                                                        result = dictionary;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bool flag19 = type == typeof(object);
                                            if (flag19)
                                            {
                                                result = JSONParser.ParseAnonymousValue(json);
                                            }
                                            else
                                            {
                                                bool flag20 = json[0] == '{' && json[json.Length - 1] == '}';
                                                if (flag20)
                                                {
                                                    result = JSONParser.ParseObject(type, json);
                                                }
                                                else
                                                {
                                                    result = null;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x0000290C File Offset: 0x00000B0C
        private static object ParseAnonymousValue(string json)
        {
            bool flag = json.Length == 0;
            object result;
            if (flag)
            {
                result = null;
            }
            else
            {
                bool flag2 = json[0] == '{' && json[json.Length - 1] == '}';
                if (flag2)
                {
                    List<string> list = JSONParser.Split(json);
                    bool flag3 = list.Count % 2 != 0;
                    if (flag3)
                    {
                        result = null;
                    }
                    else
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>(list.Count / 2);
                        for (int i = 0; i < list.Count; i += 2)
                        {
                            dictionary[list[i].Substring(1, list[i].Length - 2)] = JSONParser.ParseAnonymousValue(list[i + 1]);
                        }
                        result = dictionary;
                    }
                }
                else
                {
                    bool flag4 = json[0] == '[' && json[json.Length - 1] == ']';
                    if (flag4)
                    {
                        List<string> list2 = JSONParser.Split(json);
                        List<object> list3 = new List<object>(list2.Count);
                        for (int j = 0; j < list2.Count; j++)
                        {
                            list3.Add(JSONParser.ParseAnonymousValue(list2[j]));
                        }
                        result = list3;
                    }
                    else
                    {
                        bool flag5 = json[0] == '"' && json[json.Length - 1] == '"';
                        if (flag5)
                        {
                            string text = json.Substring(1, json.Length - 2);
                            result = text.Replace("\\", string.Empty);
                        }
                        else
                        {
                            bool flag6 = char.IsDigit(json[0]) || json[0] == '-';
                            if (flag6)
                            {
                                bool flag7 = json.Contains(".");
                                if (flag7)
                                {
                                    double num;
                                    double.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out num);
                                    result = num;
                                }
                                else
                                {
                                    int num2;
                                    int.TryParse(json, out num2);
                                    result = num2;
                                }
                            }
                            else
                            {
                                bool flag8 = json == "true";
                                if (flag8)
                                {
                                    result = true;
                                }
                                else
                                {
                                    bool flag9 = json == "false";
                                    if (flag9)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        result = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002B40 File Offset: 0x00000D40
        private static Dictionary<string, T> CreateMemberNameDictionary<T>(T[] members) where T : MemberInfo
        {
            Dictionary<string, T> dictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            foreach (T t in members)
            {
                bool flag = t.IsDefined(typeof(IgnoreDataMemberAttribute), true);
                if (!flag)
                {
                    string name = t.Name;
                    bool flag2 = t.IsDefined(typeof(DataMemberAttribute), true);
                    if (flag2)
                    {
                        DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(t, typeof(DataMemberAttribute), true);
                        bool flag3 = !string.IsNullOrEmpty(dataMemberAttribute.Name);
                        if (flag3)
                        {
                            name = dataMemberAttribute.Name;
                        }
                    }
                    dictionary.Add(name, t);
                }
            }
            return dictionary;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002C10 File Offset: 0x00000E10
        private static object ParseObject(Type type, string json)
        {
            object uninitializedObject = FormatterServices.GetUninitializedObject(type);
            List<string> list = JSONParser.Split(json);
            bool flag = list.Count % 2 != 0;
            object result;
            if (flag)
            {
                result = uninitializedObject;
            }
            else
            {
                Dictionary<string, FieldInfo> dictionary;
                bool flag2 = !JSONParser.fieldInfoCache.TryGetValue(type, out dictionary);
                if (flag2)
                {
                    dictionary = JSONParser.CreateMemberNameDictionary<FieldInfo>(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                    JSONParser.fieldInfoCache.Add(type, dictionary);
                }
                Dictionary<string, PropertyInfo> dictionary2;
                bool flag3 = !JSONParser.propertyInfoCache.TryGetValue(type, out dictionary2);
                if (flag3)
                {
                    dictionary2 = JSONParser.CreateMemberNameDictionary<PropertyInfo>(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                    JSONParser.propertyInfoCache.Add(type, dictionary2);
                }
                for (int i = 0; i < list.Count; i += 2)
                {
                    bool flag4 = list[i].Length <= 2;
                    if (!flag4)
                    {
                        string key = list[i].Substring(1, list[i].Length - 2);
                        string json2 = list[i + 1];
                        FieldInfo fieldInfo;
                        bool flag5 = dictionary.TryGetValue(key, out fieldInfo);
                        if (flag5)
                        {
                            fieldInfo.SetValue(uninitializedObject, JSONParser.ParseValue(fieldInfo.FieldType, json2));
                        }
                        else
                        {
                            PropertyInfo propertyInfo;
                            bool flag6 = dictionary2.TryGetValue(key, out propertyInfo);
                            if (flag6)
                            {
                                propertyInfo.SetValue(uninitializedObject, JSONParser.ParseValue(propertyInfo.PropertyType, json2), null);
                            }
                        }
                    }
                }
                result = uninitializedObject;
            }
            return result;
        }

        // Token: 0x04000001 RID: 1
        [ThreadStatic]
        private static Stack<List<string>> splitArrayPool;

        // Token: 0x04000002 RID: 2
        [ThreadStatic]
        private static StringBuilder stringBuilder;

        // Token: 0x04000003 RID: 3
        [ThreadStatic]
        private static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache;

        // Token: 0x04000004 RID: 4
        [ThreadStatic]
        private static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache;
    }
}
