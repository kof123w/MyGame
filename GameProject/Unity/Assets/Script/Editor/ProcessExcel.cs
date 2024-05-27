using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEditor;

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
        
        //加载excel
        string testExcel = $"{excelPath}\\测试用配置.xlsx";
        var fileInfo = new FileInfo(testExcel);
        using (ExcelPackage pack = new ExcelPackage(fileInfo))
        {
            ExcelWorkbook workBook = pack.Workbook;
            var currentWorksheet = workBook.Worksheets.First();
            currentWorksheet.Workbook.CalcMode = ExcelCalcMode.Automatic; 
            UnityEngine.Debug.Log(currentWorksheet.Cells["A1"].Value);
        }
    } 
}