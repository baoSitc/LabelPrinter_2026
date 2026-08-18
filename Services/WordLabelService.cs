using LabelPrinter.Models;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using Word = Microsoft.Office.Interop.Word;

namespace LabelPrinter.Services
{
    public class WordLabelService
    {
        // =========================================================
        // IN TEM
        // =========================================================

        public void PrintLabel(
            LabelData label,
            LabelSize labelSize,
            string printerName,
            int quantity)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            if (quantity <= 0)
                return;

            string templatePath =
                GetTemplatePath(labelSize);

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    "Không tìm thấy file Word template.",
                    templatePath);
            }

            Word.Application wordApp = null;
            Word.Document document = null;

            try
            {
                // =================================================
                // KHỞI ĐỘNG WORD
                // =================================================

                wordApp = new Word.Application();

                wordApp.Visible = true;
                

                // =================================================
                // MỞ TEMPLATE
                // =================================================

                document =
                    wordApp.Documents.Open(
                        templatePath,
                        ReadOnly: false,
                        Visible: true);

                // =================================================
                // QUAN TRỌNG
                // ACTIVE DOCUMENT
                // =================================================

                document.Activate();

                wordApp.Activate();

                // =================================================
                // THAY PLACEHOLDER
                // =================================================

                ReplacePlaceholders(
                    document,
                    label);

                // =================================================
                // LƯU FILE TẠM
                // =================================================

                string tempFile =
                    Path.Combine(
                        Path.GetTempPath(),
                        $"Label_{Guid.NewGuid():N}.docx");

                document.SaveAs2(
                    tempFile,
                    Word.WdSaveFormat.wdFormatXMLDocument);

                // =================================================
                // CHỌN MÁY IN
                // =================================================

                wordApp.ActivePrinter =
                    printerName;

                // =================================================
                // IN
                // =================================================

                document.PrintOut(
                    Copies: quantity);

                // =================================================
                // ĐÓNG
                // =================================================

                document.Close(
                    Word.WdSaveOptions.wdSaveChanges);

                document = null;

                wordApp.Quit();

                wordApp = null;

                // =================================================
                // XÓA FILE TẠM
                // =================================================

                try
                {
                    File.Delete(tempFile);
                }
                catch
                {
                    // Không quan trọng nếu file chưa xóa được
                }
            }
            finally
            {
                try
                {
                    if (document != null)
                    {
                        document.Close(
                            Word.WdSaveOptions.wdDoNotSaveChanges);
                    }
                }
                catch
                {
                }

                try
                {
                    if (wordApp != null)
                    {
                        wordApp.Quit();
                    }
                }
                catch
                {
                }

                ReleaseComObject(document);
                ReleaseComObject(wordApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        // =========================================================
        // THAY PLACEHOLDER
        // =========================================================

        private void ReplacePlaceholders(
            Word.Document document,
            LabelData label)
        {
            var values =
                new Dictionary<string, string>
                {
                    ["{{TenHang}}"] =
                        GetValue(label.TenHang),

                    ["{{ThanhPhan}}"] =
                        GetValue(label.ThanhPhan),

                    ["{{XuatXu}}"] =
                        GetValue(label.XuatXu),

                    ["{{BaoQuan}}"] =
                        GetValue(label.BaoQuan),

                    ["{{NgaySanXuat}}"] =
                        GetValue(label.NgaySanXuat),

                    ["{{HanSuDung}}"] =
                        GetValue(label.HanSuDung),

                    ["{{NhaPhanPhoi}}"] =
                        GetValue(label.NhaPhanPhoi),

                    ["{{DiaChiNhaPhanPhoi}}"] =
                        GetValue(label.DiaChiNhaPhanPhoi),

                    ["{{NoiSanXuat}}"] =
                        GetValue(label.NoiSanXuat),

                    ["{{DiaChiSanXuat}}"] =
                        GetValue(label.DiaChiSanXuat),

                    ["{{SoBanIn}}"] =
                        label.SoBanIn.ToString()
                };


            // =====================================================
            // BODY
            // =====================================================

            Word.Range range = null;

            try
            {
                range = document.Content;

                foreach (var item in values)
                {
                    ReplaceText(
                        range,
                        item.Key,
                        item.Value);
                }
            }
            finally
            {
                ReleaseComObject(range);
            }
        }


        // =========================================================
        // REPLACE TEXT
        // =========================================================

        private void ReplaceText(
            Word.Range range,
            string findText,
            string replaceText)
        {
            if (range == null)
                return;

            Word.Find find = null;

            try
            {
                find = range.Find;

                find.ClearFormatting();

                find.Replacement.ClearFormatting();

                find.Text =
                    findText;

                find.Replacement.Text =
                    replaceText;

                find.Forward = true;

                find.Wrap =
                    Word.WdFindWrap.wdFindStop;

                find.Format = false;

                find.MatchCase = false;

                find.MatchWholeWord = false;

                find.MatchWildcards = false;

                find.Execute(
                    Replace:
                        Word.WdReplace.wdReplaceAll);
            }
            finally
            {
                ReleaseComObject(find);
            }
        }


        // =========================================================
        // TEMPLATE
        // =========================================================

        private string GetTemplatePath(
            LabelSize labelSize)
        {
            string folder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Templates");

            string fileName;

            switch (labelSize.Id)
            {
                case "A7":
                    fileName = "A7.docx";
                    break;

                case "50x50":
                    fileName = "50x50.docx";
                    break;

                case "50x30":
                    fileName = "50x30.docx";
                    break;

                case "75x50":
                    fileName = "75x50.docx";
                    break;

                case "100x150":
                    fileName = "100x150.docx";
                    break;

                default:
                    throw new Exception(
                        $"Chưa có template cho khổ {labelSize.Id}");
            }

            return Path.Combine(
                folder,
                fileName);
        }


        // =========================================================
        // GET VALUE
        // =========================================================

        private string GetValue(
            string? value)
        {
            return value ?? "";
        }


        // =========================================================
        // RELEASE COM
        // =========================================================

        private void ReleaseComObject(
            object? obj)
        {
            try
            {
                if (obj != null &&
                    Marshal.IsComObject(obj))
                {
                    Marshal.ReleaseComObject(obj);
                }
            }
            catch
            {
            }
        }
    }
}
