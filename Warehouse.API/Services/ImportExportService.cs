using System.Diagnostics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;

namespace Warehouse.API.Services
{
    public class ImportExportService
    {
        public List<SetProduct> ImportProducts(byte[] data)
        {
            var result = new List<SetProduct>();
            using var ms = new MemoryStream(data);
            var wb = new XSSFWorkbook(ms);
            ISheet excelSheet = wb.GetSheetAt(0);

            for (var i = 0; i <= excelSheet.LastRowNum; i++)
            {
                try
                {
                    IRow row = excelSheet.GetRow(i);
                    var name = row.GetCell(0).ToString();
                    if (!string.IsNullOrEmpty(name))
                    {
                        var description = row.GetCell(1)?.ToString();
                        result.Add(new SetProduct
                        {
                            Name = name,
                            Description = description ?? string.Empty,
                        });
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                }
            }

            return result;
        }
    }
}
