using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Debug = UnityEngine.Debug;

public static class ExcelTool
{
    private const string OutPutPathGlo = "Assets\\StreamingAssets\\Config";
    private const string ExcelIndexGlo = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string VoPathGlo = "Assets\\Script\\GameScript\\Excel\\ConfigVO";
    private const int ExploreRowGlo = 2; //注释掉的列索引
    private const int ExploreColumnGlo = 1; //注释掉的列索引
    private const int DataTypeColumnGlo = 4; //字段类型
    private const int DataNameColumnGlo = 5; //字段名
    private const int ClientIsUseGlo = 3; //客户端是否使用字段判断 
    private const int DataFuncColumnGlo = 6; //字段功能注释

    //[MenuItem("ProcessExcel/ReBuildConfig")]
    public static void RebuildConfig(string excelPathGlo)
    {
        if (!Directory.Exists(OutPutPathGlo))
        {
            var stream = Directory.CreateDirectory(OutPutPathGlo);
        }

        //保持vo文件夹的干净
        ClearVoFile();
        //报错config文件夹干净
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

                string excelName = $"{excelPathGlo}\\{fileName}";
                var fileInfo = new FileInfo(excelName);
                using (ExcelPackage pack = new ExcelPackage(fileInfo))
                {
                    ExcelWorkbook workBook = pack.Workbook;
                    var currentWorksheet = workBook.Worksheets.First();
                    currentWorksheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
                    //获取这张表最大的列索引
                    int maxRow = 0;
                    int row = 1;
                    string indexStr = GetRowStrByIndex(row);
                    string str = currentWorksheet.Cells[$"{indexStr}4"].Value as string;
                    while (!string.IsNullOrEmpty(str))
                    {
                        row++;
                        indexStr = GetRowStrByIndex(row);
                        str = currentWorksheet.Cells[$"{indexStr}4"].Value as string;
                    }

                    maxRow = row - 1;

                    //A2是表名
                    string outPutFileName = currentWorksheet.Cells["B1"].Value as string;
                    string filePath = $"{OutPutPathGlo}\\{outPutFileName}.bin";
                    string fileMgrPath = $"Assets\\\\StreamingAssets\\\\Config\\\\{outPutFileName}.bin";
                    //生成配置VO模板 
                    GenVoClass(currentWorksheet, maxRow, outPutFileName);
                    //生成配置cahce
                    GenVoClassBinCache(currentWorksheet, maxRow, outPutFileName, fileMgrPath);
                    //创建出来这个文件
                    GenConfigBind(currentWorksheet, filePath, maxRow);
                }
            }
        }
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

    private static void ClearVoFile()
    {
        if (Directory.Exists(VoPathGlo))
        {
            string[] voFiles = Directory.GetFiles(VoPathGlo);
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

    private static void ClearPathFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    //生成vo class
    private static void GenVoClass(ExcelWorksheet worksheet, int maxRow, string tableName)
    {
        string fileName = $"{VoPathGlo}\\{tableName}.cs";
        Debug.Log(fileName);
        if (!File.Exists(fileName))
        {
            var fileStream = File.Create(fileName);
            fileStream.Close();
            Debug.Log("创建文件:" + fileName);
        }

        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine("namespace Config");
            sw.WriteLine("{");
            sw.WriteLine($"  public class {tableName}");
            sw.WriteLine("   {");

            for (int i = 1; i <= maxRow; i++)
            {
                //看看这一列是否被注释掉了
                var rowStr = GetRowStrByIndex(i);
                var explore = worksheet.Cells[$"{rowStr}{ExploreRowGlo}"].Value as string;
                if (explore != null && explore.Contains("#")) continue;
                var clientIsUseStr = worksheet.Cells[$"{rowStr}{ClientIsUseGlo}"].Value as string;
                if (!string.IsNullOrEmpty(clientIsUseStr) && !clientIsUseStr.Contains("C")) continue;

                string dataType = worksheet.Cells[$"{rowStr}{DataTypeColumnGlo}"].Value as string;
                string dataName = worksheet.Cells[$"{rowStr}{DataNameColumnGlo}"].Value as string;
                string dataExplore = worksheet.Cells[$"{rowStr}{DataFuncColumnGlo}"].Value as string;
                string pre = " {get;set;}";
                sw.WriteLine($"    public {dataType} {dataName} {pre} //{dataExplore}");
            }

            sw.WriteLine("   }");
            sw.WriteLine("}");
        }
    }

    //生成后缀
    private static string m_GenSuffix = "BinCache";
    //生成vo mgr class
    private static void GenVoClassBinCache(ExcelWorksheet worksheet, int maxRow, string tableName, string binFile)
    {
        string fileName = $"{VoPathGlo}\\{tableName}BinCahce.cs";

        //主字段名
        string mainDataName = worksheet.Cells["C1"].Value as string;
        string mainDataType = worksheet.Cells["B4"].Value as string;
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using System.IO;");
            sw.WriteLine("namespace Config");
            sw.WriteLine("{");
            sw.WriteLine($"\tclass {tableName}{m_GenSuffix}:CacheObject<{tableName}>");
            sw.WriteLine("\t{"); 
            sw.WriteLine($"\t\tpublic {tableName}{m_GenSuffix}()");
            sw.WriteLine("\t\t{");
            sw.WriteLine($"\t\t\tFileStream fileStream = new FileStream(\"{binFile}\",FileMode.Open);");
            sw.WriteLine($"\t\t\tBinaryReader binaryReader = new BinaryReader(fileStream);");
            sw.WriteLine($"\t\t\tstring[] strArray = binaryReader.ReadString().Split('\\n');");
            sw.WriteLine($"\t\t\tfor (int i = 0; i < strArray.Length; i++)");
            sw.WriteLine("\t\t\t{");
            sw.WriteLine($"\t\t\t\tvar str = strArray[i];");
            sw.WriteLine($"\t\t\t\tif(string.IsNullOrEmpty(str)) continue;");
            sw.WriteLine($"\t\t\t\tvar element = str.Split('|');");
            sw.WriteLine($"\t\t\t\t{tableName} config = new {tableName}();");
            for (int i = 1; i <= maxRow; i++)
            {
                //看看这一列是否被注释掉了
                var rowStr = GetRowStrByIndex(i);
                var explore = worksheet.Cells[$"{rowStr}{ExploreRowGlo}"].Value as string;
                if (explore != null && explore.Contains("#")) continue;
                var clientIsUseStr = worksheet.Cells[$"{rowStr}{ClientIsUseGlo}"].Value as string;
                if (!string.IsNullOrEmpty(clientIsUseStr) && !clientIsUseStr.Contains("C")) continue;

                string dataType = worksheet.Cells[$"{rowStr}{DataTypeColumnGlo}"].Value as string;
                string dataName = worksheet.Cells[$"{rowStr}{DataNameColumnGlo}"].Value as string;
                if (dataType.Contains("string"))
                {
                    sw.WriteLine($"\t\t\t\tconfig.{dataName}=element[{i - 1}];");
                }
                else
                {
                    sw.WriteLine($"\t\t\t\tconfig.{dataName}={dataType}.Parse(element[{i - 1}]);");
                }
            }
            sw.WriteLine($"\t\t\t\tCacheList.Add(config);");
            sw.WriteLine("\t\t\t}");
            sw.WriteLine($"\t\t\tbinaryReader.Close();");
            sw.WriteLine($"\t\t\tbinaryReader.Dispose();");
            sw.WriteLine($"\t\t\tfileStream.Close();");
            sw.WriteLine($"\t\t\tfileStream.Dispose();");
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t}");
            sw.WriteLine("}");
        }
    }

    //生成config文件
    private static void GenConfigBind(ExcelWorksheet worksheet, string filePath, int maxRow)
    {
        if (!File.Exists(filePath))
        {
            var tmpFileStream = File.Create(filePath);
            tmpFileStream.Close();
            Debug.Log($"创建Bin文件{filePath}");
        }
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fileStream);
        int curCulomn = 7;
        string writeTxt = "";
        while (true)
        {
            string val;  
            string dataType = worksheet.Cells[$"{GetRowStrByIndex(1)}4"].Value as string;

            if (dataType == null)
            {
                Debug.Log($"配置表{filePath}异常，无配置主ID");
            } 

            bool checkIsNoVal = false;
            GetVal(dataType, worksheet, GetRowStrByIndex(1), curCulomn,ref checkIsNoVal);
            if (checkIsNoVal)
            {
                break;
            }

            //判断是否被注释
            val = worksheet.Cells[$"{GetRowStrByIndex(0)}{curCulomn}"].Value as string;
            if (string.IsNullOrEmpty(val) || !val.Contains("#"))
            {
                for (int i = 1; i <= maxRow; i++)
                {
                    dataType = worksheet.Cells[$"{GetRowStrByIndex(i)}4"].Value as string;
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

                    bool isNoVal = false;
                    string strVal = GetVal(dataType,worksheet,GetRowStrByIndex(i),curCulomn,ref isNoVal);
                    writeTxt =i != 1 ? $"{writeTxt}|{strVal}" : $"{writeTxt}{strVal}"; 
                   
                } 
                writeTxt += "\n"; 
            } 
            curCulomn++;
        } 
        bw.Write(writeTxt);
        bw.Close();
        fileStream.Close();  
    }

    private static string GetVal(string dataType,ExcelWorksheet worksheet,string row,int curCulomn,ref bool isNoVal)
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
}