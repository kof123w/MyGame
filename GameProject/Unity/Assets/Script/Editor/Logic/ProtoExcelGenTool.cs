using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Google.Protobuf;
using JetBrains.Annotations;
using OfficeOpenXml;
using Debug = UnityEngine.Debug;

namespace MyGame.Editor
{
    public static class ProtoExcelGenTool
    {
        private const string OutPutPathGlo = "Assets/Resources/Config";
        private const string ExcelIndexGlo = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string VoPathGlo = "Assets\\Script\\GameScript\\Excel\\ConfigVO";
        private static string ProtoVoPathGlo = string.Empty;

        private const int ExploreRowGlo = 2; //注释掉的列索引
        private const int ExploreColumnGlo = 1; //注释掉的列索引
        private const int DataTypeColumnGlo = 4; //字段类型
        private const int DataNameColumnGlo = 5; //字段名
        private const int ClientIsUseGlo = 3; //客户端是否使用字段判断 
        private const int DataFuncColumnGlo = 6; //字段功能注释

        public static void ReBuildConfigVo(string excelPathGlo, string protocPath)
        {
            if (!Directory.Exists(OutPutPathGlo))
            {
                Directory.CreateDirectory(OutPutPathGlo);
            }

            if (string.IsNullOrEmpty(excelPathGlo))
            {
                DLogger.Error("Excel path is empty!!!");
                return;
            }

            ProtoVoPathGlo = $"{excelPathGlo}/ClientConfigProto";

            ClearProtoVoFile();
            if (Directory.Exists(excelPathGlo))
            {
                string[] files = Directory.GetFiles(excelPathGlo);
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    string fileName = Path.GetFileName(file);
                    if (fileName.Contains('$'))
                    {
                        continue;
                    }

                    var fileInfo = new FileInfo($"{excelPathGlo}\\{fileName}");
                    using (ExcelPackage pack = new ExcelPackage(fileInfo))
                    {  
                        foreach (ExcelWorksheet currentWorksheet in pack.Workbook.Worksheets)
                        {
                            string filePath = null,className = null;
                            int maxRow = 0;
                            GetExcelMsg(currentWorksheet,ref filePath,ref className,ref maxRow);
                            //生成配置proto VO模板文件 
                            GenProtoVoProtoFile(currentWorksheet,pack.File.Name, maxRow, className); 
                            //生成cache bin
                            GenVoClassBinCache(currentWorksheet,className);
                        } 
                    }
                }
            }

            Debug.Log("Proto生成完成!");
            if (Directory.Exists(ProtoVoPathGlo))
            {
                string[] files = Directory.GetFiles(ProtoVoPathGlo);
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    var fileInfo = new FileInfo($"{ProtoVoPathGlo}\\{Path.GetFileName(file)}");
                    CompileProtoToCSharp(fileInfo.FullName, protocPath, VoPathGlo);
                }
            }

            Debug.Log("Proto Vo类生成完毕!");
        }

        public static void ReBuildConfig(string excelPathGlo)
        {
            ClearConfigFile();
            if (Directory.Exists(excelPathGlo))
            {
                string[] files = Directory.GetFiles(excelPathGlo);
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    string fileName = Path.GetFileName(file);
                    if (fileName.Contains('$'))
                    {
                        continue;
                    }

                    var fileInfo = new FileInfo($"{excelPathGlo}\\{fileName}");
                    using (ExcelPackage pack = new ExcelPackage(fileInfo))
                    {
                        foreach (ExcelWorksheet currentWorksheet in pack.Workbook.Worksheets)
                        {
                            string filePath = null,className = null;
                            int maxRow = 0; 
                            GetExcelMsg(currentWorksheet, ref filePath,ref className,ref maxRow);
                            //生成配置proto配置文件 
                            GenProtoVoConfigBin(currentWorksheet, filePath, className, maxRow);
                        } 
                    }
                }
            }
        }


        private static void GetExcelMsg(ExcelWorksheet currentWorksheet,  ref string filePath,ref string className,ref int maxRow)
        { 
            //var currentWorksheet = workBook.Worksheets.First();
            currentWorksheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            //获取这张表最大的列索引 
            int row = 1;
            string indexStr = GetRowStrByIndex(row);
            string str = currentWorksheet.Cells[$"{indexStr}4"].Value as string;
            while (!string.IsNullOrEmpty(str))
            {
                row++;
                indexStr = GetRowStrByIndex(row);
                str = currentWorksheet.Cells[$"{indexStr}4"].Value as string;
            }

            className = currentWorksheet.Cells["B1"].Value as string;
            filePath = $"{OutPutPathGlo}/{className}.bin";
            maxRow = row - 1; 
        }

        private static string GetRowStrByIndex(int n)
        {
            string str = "";
            //先转成26进制再进行索引
            int base26 = 26;
            bool isCon = false;
            do
            {
                int index = n % base26;
                str = $"{ExcelIndexGlo[index]}{str}";
                isCon = n / base26 > 0;
                n = n / base26 - 1;
            } while (isCon);

            return str;
        }

        private static void ClearProtoVoFile()
        {
            if (Directory.Exists(ProtoVoPathGlo))
            {
                string[] voFiles = Directory.GetFiles(ProtoVoPathGlo);
                foreach (string voFile in voFiles)
                {
                    File.Delete(voFile);
                }
            }
            else
            {
                Directory.CreateDirectory(ProtoVoPathGlo);
            }
        } 

        private static void ClearConfigFile()
        {
            if (Directory.Exists(OutPutPathGlo))
            {
                string[] voFiles = Directory.GetFiles(OutPutPathGlo);
                foreach (string voFile in voFiles)
                {
                    if (voFile.Contains(".meta"))
                    {
                        continue;
                    }

                    File.Delete(voFile);
                }
            }
        }

        //生成 proto vo 文件
        private static void GenProtoVoProtoFile(ExcelWorksheet worksheet,string excelName, int maxRow, string tableName)
        {
            string fileName = $"{ProtoVoPathGlo}\\{tableName}.proto";
            Debug.Log(fileName);
            if (!File.Exists(fileName))
            {
                var fileStream = File.Create(fileName);
                fileStream.Close();
                Debug.Log("创建文件:" + fileName);
            }

            using (StreamWriter sw = new StreamWriter(fileName))
            { 
                sw.WriteLine($"//Gen by {excelName}\\{worksheet.Name}");
                sw.WriteLine("syntax = \"proto3\";");
                sw.WriteLine($"message {tableName}");
                sw.WriteLine("{");
                for (int i = 1; i <= maxRow; i++)
                {
                    //看看这一列是否被注释掉了
                    var rowStr = GetRowStrByIndex(i);
                    var explore = worksheet.Cells[$"{rowStr}{ExploreRowGlo}"].Value as string;
                    if (explore != null && explore.Contains("#")) continue;
                    var clientIsUseStr = worksheet.Cells[$"{rowStr}{ClientIsUseGlo}"].Value as string;
                    if (!string.IsNullOrEmpty(clientIsUseStr) && !clientIsUseStr.Contains("C")) continue;

                    string dataType =
                        DataTypeConverter(worksheet.Cells[$"{rowStr}{DataTypeColumnGlo}"].Value as string);
                    string dataName = worksheet.Cells[$"{rowStr}{DataNameColumnGlo}"].Value as string;
                    sw.WriteLine($"    {dataType} {dataName} = {i};");
                }

                sw.WriteLine("}");
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine($"message {tableName}List");
                sw.WriteLine("{");
                sw.WriteLine($"    repeated {tableName} dataList = 1;");
                sw.WriteLine("}");
            }
        }

        private static void MatchType(ref Type listType, ref Type itemType, string tableName)
        {
            // 获取所有用户程序集（排除系统程序集）
            var userAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("GameScript_Excel"));

            foreach (var assembly in userAssemblies)
            {
                var types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    var interfaces = type.GetInterfaces();
                    for (int j = 0; j < interfaces.Length; j++)
                    {
                        if (interfaces[j].Name.Equals("IMessage"))
                        {
                            if (type.Name.Equals(tableName))
                            {
                                itemType = type;
                            }
                            else if (type.Name.Equals($"{tableName}List"))
                            {
                                listType = type;
                            }
                        }
                    }
                }
            }
        }
        
        //生成 proto vo cache bin
        private static void GenVoClassBinCache(ExcelWorksheet worksheet, string tableName)
        {
            string fileName = $"{VoPathGlo}\\{tableName}BinCahce.cs";
            string fileMgrPath = $"Assets\\\\Resources\\\\Config\\\\{tableName}.bin";
            //主字段名
            string mainDataName = worksheet.Cells["C1"].Value as string;
            string mainDataType = worksheet.Cells["B4"].Value as string;
            
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using System.IO;");
                sw.WriteLine("using Google.Protobuf;");
                sw.WriteLine("namespace Config");
                sw.WriteLine("{");
                sw.WriteLine($"  class {tableName}BinCache:CacheObject<{tableName}>");
                sw.WriteLine("  {");
                sw.WriteLine($"    public {tableName}BinCache()");
                sw.WriteLine("    {");
                sw.WriteLine($"      byte[] data = File.ReadAllBytes(\"{fileMgrPath}\");");
                sw.WriteLine($"      var list = {tableName}List.Parser.ParseFrom(data);");
                sw.WriteLine($"      var enumerator = list.DataList.GetEnumerator();"); 
                sw.WriteLine($"      while (enumerator.MoveNext())"); 
                sw.WriteLine("      {"); 
                sw.WriteLine("         CacheList.Add(enumerator.Current);");  
                sw.WriteLine("       }"); 
                sw.WriteLine($"       enumerator.Dispose();"); 
                sw.WriteLine("    }");
                sw.WriteLine("  }");
                sw.WriteLine("}");
            }
            
            /*using (StreamWriter sw = new StreamWriter(fileName))
            { 
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using System.IO;");
                sw.WriteLine("using Google.Protobuf;");
                sw.WriteLine("namespace Config");
                sw.WriteLine("{");
                sw.WriteLine($"  class {tableName}BinCache:CacheObject<{tableName}>");
                sw.WriteLine("  {");
                sw.WriteLine($"    public {tableName}BinCache()");
                sw.WriteLine("    {");
                sw.WriteLine($"      byte[] data = File.ReadAllBytes(\"{fileMgrPath}\");");
                sw.WriteLine($"      using (var input = new CodedInputStream(data))");
                sw.WriteLine("      {");
                sw.WriteLine("        // 读取消息长度（对于 protobuf，通常需要手动管理长度）");
                sw.WriteLine("        int size = input.ReadInt32(); // 注意：这里的 size 是消息的实际大小，不包括长度字段本身。");
                sw.WriteLine("        byte[] messageData = input.ReadRawBytes(size); // 读取实际的消息数据。");
                sw.WriteLine("        using (var subInput = new CodedInputStream(messageData)){");
                sw.WriteLine($"          {tableName}List message = new {tableName}List(); // 创建消息实例");
                sw.WriteLine($"          message.MergeFrom(subInput); // 从子输入流中合并数据到消息实例中"); 
                sw.WriteLine($"          var enumerator = message.DataList.GetEnumerator();"); 
                sw.WriteLine($"          while (enumerator.MoveNext())"); 
                sw.WriteLine("          {"); 
                sw.WriteLine("            CacheList.Add(enumerator.Current);");  
                sw.WriteLine("          }"); 
                sw.WriteLine($"          enumerator.Dispose();"); 
                sw.WriteLine("        }"); 
                sw.WriteLine("      }");
                sw.WriteLine("    }");
                sw.WriteLine("  }");
                sw.WriteLine("}");
            }*/
        }

        //生成 proto vo bin 文件
        private static void GenProtoVoConfigBin(ExcelWorksheet worksheet, string filePath, string className, int maxRow)
        {
            if (!File.Exists(filePath))
            {
                var tmpFileStream = File.Create(filePath);
                tmpFileStream.Close();
                Debug.Log($"创建Bin文件{filePath}");
            }

            Type listType = null;
            Type itemType = null;
            MatchType(ref listType, ref itemType, className);

            if (listType == null || itemType == null)
            {
                Debug.LogError($"搜索{className}的程序集失败!");
                return;
            }

            //列表类型
            var listObject = Activator.CreateInstance(listType);
            
            int curCulomn = 7; 
            while (true)
            {
                string val;
                string dataType = worksheet.Cells[$"{GetRowStrByIndex(1)}4"].Value as string;

                if (dataType == null)
                {
                    Debug.Log($"配置表{filePath}异常，无配置主ID");
                }

                bool checkIsNoVal = false;
                GetVal(dataType, worksheet, GetRowStrByIndex(1), curCulomn, ref checkIsNoVal);
                if (checkIsNoVal)
                {
                    break;
                }

                //判断是否被注释
                val = worksheet.Cells[$"{GetRowStrByIndex(0)}{curCulomn}"].Value as string;
                var itemObject = Activator.CreateInstance(itemType);
                if (string.IsNullOrEmpty(val) || !val.Contains("#"))
                {
                    for (int i = 1; i <= maxRow; i++)
                    {
                        dataType = worksheet.Cells[$"{GetRowStrByIndex(i)}4"].Value as string;
                        var proptyName = worksheet.Cells[$"{GetRowStrByIndex(i)}5"].Value as string;
                        //判断是否被注释了
                        val = worksheet.Cells[$"{GetRowStrByIndex(i)}{2}"].Value as string;
                        if (!string.IsNullOrEmpty(val) && val.Contains("#"))
                        {
                            continue;
                        }

                        //是否包含客户端
                        val = worksheet.Cells[$"{GetRowStrByIndex(i)}{3}"].Value as string;
                        if (!string.IsNullOrEmpty(val) && !val.Contains("C"))
                        {
                            continue;
                        }

                        if (dataType == null)
                        {
                            Debug.Log($"配置表{filePath}异常，无配置主ID");
                            continue;
                        }
                         
                        SetVal(dataType, worksheet, GetRowStrByIndex(i), curCulomn,proptyName,itemObject,itemType);
                    }
                }
                AddItemToListCached(listObject,"DataList",itemObject);
                curCulomn++;
            }

            var calcCalculateSize = listType.GetMethod("CalculateSize");
            if (calcCalculateSize != null)
            {
                var size = calcCalculateSize.Invoke(listObject, new object[] { });
                //序列化写入文件 
                byte[] data = new byte[(int)size];
                using (CodedOutputStream cos = new CodedOutputStream(data))
                {
                    var method = listType.GetMethod("WriteTo"); 
                    if (method != null)
                    {
                        method.Invoke(listObject, new[] { cos }); 
                        File.WriteAllBytes(filePath, data); 
                    }
                }
            } 
        }


        private static readonly Dictionary<Type, Dictionary<string, Action<object, object>>> Cache =
            new Dictionary<Type, Dictionary<string, Action<object, object>>>();

        public static void AddItemToListCached(object listContainer, string listPropertyName, object item)
        {
            Type containerType = listContainer.GetType();

            if (!Cache.TryGetValue(containerType, out var propertyActions))
            {
                propertyActions = new Dictionary<string, Action<object, object>>();
                Cache[containerType] = propertyActions;
            }

            if (!propertyActions.TryGetValue(listPropertyName, out var addAction))
            {
                PropertyInfo listProperty = containerType.GetProperty(listPropertyName);
                if (listProperty == null)
                    throw new ArgumentException($"属性 {listPropertyName} 不存在");

                Type listType = listProperty.PropertyType;
                Type itemType = listType.GetGenericArguments()[0];
                MethodInfo addMethod = listType.GetMethod("Add",new []{item.GetType()});

                // 创建高效的委托
                var containerParam = Expression.Parameter(typeof(object));
                var itemParam = Expression.Parameter(typeof(object));

                var castContainer = Expression.Convert(containerParam, containerType);
                var propertyAccess = Expression.Property(castContainer, listProperty);
                var castItem = Expression.Convert(itemParam, itemType);
                var callAdd = Expression.Call(propertyAccess, addMethod, castItem);

                addAction = Expression.Lambda<Action<object, object>>(callAdd, containerParam, itemParam).Compile();
                propertyActions[listPropertyName] = addAction;
            }

            addAction(listContainer, item);
        }

        public static void CompileProtoToCSharp(string protoFilePath, string protocPath, string outputDir)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = protocPath,
                    Arguments =
                        $"--csharp_out={outputDir} --proto_path={Path.GetDirectoryName(protoFilePath)} {protoFilePath}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"protoc failed with exit code {process.ExitCode}");
            }
        }

        private static string GetVal(string dataType, ExcelWorksheet worksheet, string row, int curCulomn,
            ref bool isNoVal)
        {
            if (dataType.Contains("string"))
            {
                string str = worksheet.Cells[$"{row}{curCulomn}"].Value as string;
                isNoVal = string.IsNullOrEmpty(str);
                return str;
            }

            if (dataType.Contains("uint"))
            {
                uint uintval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<uint>();
                isNoVal = uintval == 0;
                return uintval.ToString();
            }
            else if (dataType.Contains("ulong"))
            {
                ulong ulongval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<ulong>();
                isNoVal = ulongval == 0;
                return ulongval.ToString();
            }
            else if (dataType.Contains("int"))
            {
                int intval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<int>();
                isNoVal = intval == 0;
                return intval.ToString();
            }
            else if (dataType.Contains("long"))
            {
                long longval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<long>();
                isNoVal = longval == 0;
                return longval.ToString();
            }
            else if (dataType.Contains("float"))
            {
                float floatval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<float>();
                isNoVal = floatval == 0;
                return floatval.ToString();
            }
            else if (dataType.Contains("double"))
            {
                double doubleval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<uint>();
                isNoVal = doubleval == 0;
                return doubleval.ToString();
            }

            return string.Empty;
        }

        private static void SetVal(string dataType, ExcelWorksheet worksheet, string row, int curCulomn,string propName, object itemObj,Type itemType)
        {
            bool isNoVal = false;
            if (dataType.Contains("string"))
            {
                string str = worksheet.Cells[$"{row}{curCulomn}"].Value as string;
                isNoVal = string.IsNullOrEmpty(str);
                if (!isNoVal)
                {
                    var propertyInfo = itemType.GetProperty(propName);
                    if (propertyInfo != null) propertyInfo.SetValue(itemObj, str);
                }

                return;
            }

            if (dataType.Contains("uint"))
            {
                uint uintval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<uint>();
                isNoVal = uintval == 0;
                if (!isNoVal)
                {
                    var propertyInfo = itemType.GetProperty(propName);
                    if (propertyInfo != null) propertyInfo.SetValue(itemObj, uintval);
                }
                return;
            }
            else if (dataType.Contains("ulong"))
            {
                ulong ulongval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<ulong>();
                isNoVal = ulongval == 0;
                if (!isNoVal)
                {
                    var propertyInfo = itemType.GetProperty(propName);
                    if (propertyInfo != null) propertyInfo.SetValue(itemObj, ulongval);
                }
                return;
            }
            else if (dataType.Contains("int"))
            {
                int intval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<int>();
                isNoVal = intval == 0;
                if (!isNoVal)
                {
                    var propertyInfo = itemType.GetProperty(propName);
                    if (propertyInfo != null) propertyInfo.SetValue(itemObj, intval);
                }
                return;
            }
            else if (dataType.Contains("long"))
            {
                long longval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<long>();
                isNoVal = longval == 0;
                if (!isNoVal)
                {
                    var propertyInfo = itemType.GetProperty(propName);
                    if (propertyInfo != null) propertyInfo.SetValue(itemObj, longval);
                }
                return;
            }
            else if (dataType.Contains("float"))
            {
                float floatval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<float>();
                isNoVal = floatval == 0;
                if (!isNoVal)
                {
                    var propertyInfo = itemType.GetProperty(propName);
                    if (propertyInfo != null) propertyInfo.SetValue(itemObj, floatval);
                }
                return;
            }
            else if (dataType.Contains("double"))
            {
                double doubleval = worksheet.Cells[$"{row}{curCulomn}"].GetValue<uint>();
                isNoVal = doubleval == 0;
                if (!isNoVal)
                {
                    var propertyInfo = itemType.GetProperty(propName);
                    if (propertyInfo != null) propertyInfo.SetValue(itemObj, doubleval);
                }
                return;
            }
        }

        private static string DataTypeConverter(string dataType)
        {
            switch (dataType)
            {
                case "double":
                    return "double";
                case "int":
                    return "int32";
                case "long":
                    return "int64";
                case "uint":
                    return "sint32";
                case "ulong":
                    return "sint64";
                case "bool":
                    return "bool";
                case "string":
                    return "string";
            }

            return dataType;
        }
    }
}