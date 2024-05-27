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
    private const string excelPath = "F:\\GitHubProject\\MyGame\\Excel";
    private const string outPutPath = "Assets\\StreamingAssets\\Config";
    private const string excelIndex = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    
    [MenuItem("ProcessExcel/ReBuildConfig")]
    public static void RebuildConfig()
    {
        if (!Directory.Exists(outPutPath))
        {
            Directory.CreateDirectory(outPutPath);
        }

        if (Directory.Exists(excelPath))
        {
            string[] files = Directory.GetFiles(excelPath);
            for (int i = 0; i < files.Length;i++)
            {
                string file = files[i];
                string fileName = Path.GetFileName(file);
                string excelName = $"{excelPath}\\{fileName}";
                var fileInfo = new FileInfo(excelName);
                using (ExcelPackage pack = new ExcelPackage(fileInfo))
                {
                    ExcelWorkbook workBook = pack.Workbook;
                    var currentWorksheet = workBook.Worksheets.First();
                    currentWorksheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
                    //A2是表名
                    string outPutFileName = currentWorksheet.Cells["B2"].Value as string;

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
                     

                    //生成配置VO模板
                    // todo


                    //生成配置单例模板
                    // todo

                    string filePath = $"{outPutPath}\\{outPutFileName}.bin";
                    //先删掉这个问题
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

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
            str = $"{excelIndex[index]}{str}";
            isCon = n / base26 > 0;
            n = n / base26 - 1;
        } while (isCon);

        return str;
    }
}