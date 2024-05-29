using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEditor;
using Debug = UnityEngine.Debug;
 
public static class ProcessExcel
{
    //配置表路径
    private const string ExcelPathGlo = "F:\\GitHubProject\\MyGame\\Excel";
    private const string OutPutPathGlo = "Assets\\StreamingAssets\\Config";
    private const string ExcelIndexGlo = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string VoPathGlo = "Assets\\Script\\Excel\\ConfigVO";
    private const int ExploreRowGlo = 2;  //注释掉的列索引
    private const int ExploreColumnGlo = 1;  //注释掉的列索引
    private const int DataTypeColumnGlo = 4; //字段类型
    private const int DataNameColumnGlo = 5;  //字段名
    private const int ClientIsUseGlo = 3;     //客户端是否使用字段判断 
    private const int DataFuncColumnGlo = 6;  //字段功能注释
    [MenuItem("ProcessExcel/ReBuildConfig")]
    public static void RebuildConfig()
    {
       
        if (!Directory.Exists(OutPutPathGlo))
        {
            Directory.CreateDirectory(OutPutPathGlo);
        } 
        //保持vo文件夹的干净
        ClearVoFile(); 
        if (Directory.Exists(ExcelPathGlo))
        {
            string[] files = Directory.GetFiles(ExcelPathGlo); 
            for (int i = 0; i < files.Length;i++)
            {
                string file = files[i];
                string fileName = Path.GetFileName(file);
                string excelName = $"{ExcelPathGlo}\\{fileName}";
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
                    
                    Debug.Log($"{outPutFileName}");
                    
                    //生成配置VO模板 
                    GenVoClass(currentWorksheet,maxRow,outPutFileName);

                    //生成配置单例模板
                    GenVoClassMgr(currentWorksheet,maxRow,outPutFileName);
                    
                    string filePath = $"{OutPutPathGlo}\\{outPutFileName}.bin";
                  
                    ClearPathFile(filePath); 
                    //创建出来这个文件
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath);
                    } 
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

    private static void ClearPathFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    //生成vo class
    private static void GenVoClass(ExcelWorksheet worksheet,int maxRow,string tableName)
    {
        
        string fileName = $"{VoPathGlo}\\{tableName}.cs";
        if (!File.Exists(fileName))
        {
            File.Exists(fileName);
            Debug.Log("创建文件:"+fileName);
        }

        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine("namespace Config");
            sw.WriteLine("{");
            sw.WriteLine($"  class {tableName}");
            sw.WriteLine("   {");
            
            for (int i = 1; i <= maxRow; i++)
            {
                //看看这一列是否被注释掉了
                var rowStr = GetRowStrByIndex(i);
                var explore = worksheet.Cells[$"{rowStr}{ExploreRowGlo}"].Value as string;
                if(explore!=null && explore.Contains("#")) continue;
                var clientIsUseStr = worksheet.Cells[$"{rowStr}{ClientIsUseGlo}"].Value as string;
                if(!string.IsNullOrEmpty(clientIsUseStr) && !clientIsUseStr.Contains("C")) continue;
                
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
 
    //生成vo mgr class
    private static void GenVoClassMgr(ExcelWorksheet worksheet,int maxRow,string tableName)
    {
        string fileName = $"{VoPathGlo}\\{tableName}Mgr.cs";
        if (!File.Exists(fileName))
        {
            File.Exists(fileName);
            Debug.Log("创建文件:"+fileName);
        }
 
        //主字段名
        string mainDataName = worksheet.Cells["C1"].Value as string;
        string mainDataType = worksheet.Cells["B4"].Value as string; 
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using System.IO;");
            sw.WriteLine("namespace Config");
            sw.WriteLine("{");
            sw.WriteLine($"     class {tableName}Mgr : Singleton<{tableName}Mgr>");
            sw.WriteLine("      {");
            sw.WriteLine($"             private  Dictionary<{mainDataType},{tableName}> m_dict = new Dictionary<{mainDataType},{tableName}>();");
            sw.WriteLine($"             public {tableName}Mgr()");
            sw.WriteLine("              {"); 
            sw.WriteLine($"                      using (StreamReader sr = new StreamReader(\"{fileName}\")");
            sw.WriteLine("                      {");
            sw.WriteLine("                              while (sr.Peek() >= 0)");
            sw.WriteLine("                              {");
            sw.WriteLine("                                      string line = sr.ReadLine();");
            sw.WriteLine("                                      string[] splitArr = line.Split('\\t');");
            sw.WriteLine($"                                     {tableName} data = new {tableName}();"); 
            for (int i = 1; i <= maxRow; i++)
            {
                //看看这一列是否被注释掉了
                var rowStr = GetRowStrByIndex(i);
                var explore = worksheet.Cells[$"{rowStr}{ExploreRowGlo}"].Value as string;
                if(explore!=null && explore.Contains("#")) continue;
                var clientIsUseStr = worksheet.Cells[$"{rowStr}{ClientIsUseGlo}"].Value as string;
                if(!string.IsNullOrEmpty(clientIsUseStr) && !clientIsUseStr.Contains("C")) continue;
                
                string dataType = worksheet.Cells[$"{rowStr}{DataTypeColumnGlo}"].Value as string;
                string dataName = worksheet.Cells[$"{rowStr}{DataNameColumnGlo}"].Value as string;
                if (dataType.Contains("string"))
                {
                    sw.WriteLine($"                                     data.{dataName}=splitArr[{i - 1}];");
                }
                else
                {
                    sw.WriteLine($"                                     data.{dataName}={dataType}.Parse(splitArr[{i - 1}]);");
                } 
            }
            sw.WriteLine($"                                     m_dict.Add(data.{mainDataName},data); "); 
            sw.WriteLine("                              }");
            sw.WriteLine("                      {");
            sw.WriteLine("              }");
            
            sw.WriteLine($"             public {tableName} Get{tableName}Config({mainDataType} id)");
            sw.WriteLine("              {");
            sw.WriteLine($"                     {tableName} res = null;");
            sw.WriteLine("                      if(m_dict.TryGetValue(id,out res)) return res;");
            sw.WriteLine("                      return null;");
            sw.WriteLine("              }");
            sw.WriteLine("      }");  
            sw.WriteLine("}");
        }
    }
}