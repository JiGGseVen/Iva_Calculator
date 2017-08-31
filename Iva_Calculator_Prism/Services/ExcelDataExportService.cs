using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using Iva_Calculator_Prism.Models;
using Microsoft.Win32;

namespace Iva_Calculator_Prism.Services
{
    public static class ExcelDataExportService
    {
        public static void ExportDataToExcel(List<Product> items)
        {
            _Workbook workbook = CreateAndFillWorkbooktData(items);
            var result = WriteDataToFile(workbook);

            if(result)
                MessageBox.Show("Exportacao com sucesso");
            else
                MessageBox.Show("Problema ao exportar valores");
        }

        private static bool WriteDataToFile(_Workbook workbook)
        {
            //Getting the location and file name of the excel to save from user. 
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Ficheiros (*.xlsx)|*.xlsx";
            saveDialog.FilterIndex = 2;

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                return true;
            }else
            {
                return false;
            }
        }

        private static _Workbook CreateAndFillWorkbooktData(List<Product> items)
        {
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            worksheet = workbook.ActiveSheet;

            worksheet.Name = "ExportedFromDatGrid";

            int cellRowIndex = 1;
            int cellColumnIndex = 1;

            worksheet.Cells[cellRowIndex, 1] = "Product Name";
            worksheet.Cells[cellRowIndex, 2] = "Valor Inicial";
            worksheet.Cells[cellRowIndex, 3] = "Valor Desconto";
            worksheet.Cells[cellRowIndex, 4] = "Valor Devolucao";
            worksheet.Cells[cellRowIndex, 5] = "Valor Final";
            
            cellRowIndex++;

            var boughtproducts = items.OfType<BoughtProduct>();
            var soldproducts = items.OfType<SoldProduct>();

            foreach (BoughtProduct product in boughtproducts)
            {
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.ProductName;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.BuyAmount;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.DiscountAmount;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.ReturnAmount;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.FinalAmount;
                
                cellColumnIndex = 1;
                cellRowIndex++;
            }

            worksheet.Columns[1].AutoFit();
            worksheet.Columns[2].AutoFit();
            worksheet.Columns[3].AutoFit();
            worksheet.Columns[4].AutoFit();
            worksheet.Columns[5].AutoFit();
            cellRowIndex++;

            foreach (SoldProduct product in soldproducts)
            {
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.ProductName;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.SoldAmount;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.DiscountAmount;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.ReturnAmount;
                cellColumnIndex++;
                worksheet.Cells[cellRowIndex, cellColumnIndex] = product.FinalAmount;

                cellColumnIndex = 1;
                cellRowIndex++;
            }

            return workbook;
        }

        private static void ProcessBoughtProducts()
        {

        }

        private static void ProcessSoldProducts()
        {

        }
    }
}
