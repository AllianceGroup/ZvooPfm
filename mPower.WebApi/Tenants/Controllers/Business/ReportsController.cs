using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Default.ViewModel.BusinessController.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.DocumentServices.Accounting.Reports;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils;
using mPower.WebApi.Tenants.Model.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Font = iTextSharp.text.Font;

namespace mPower.WebApi.Tenants.Controllers.Business
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class ReportsController : BaseController
    {
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly EntryDocumentService _entryService;
        private readonly BusinessReportDocumentService _businessReportService;
        private readonly TransactionDocumentService _transactionService;
        private readonly HtmlToPdfWriter _htmlToPdfWriter;

        public ReportsController(LedgerDocumentService ledgerService, EntryDocumentService entryService, 
            BusinessReportDocumentService businessReportService, TransactionDocumentService transactionService, HtmlToPdfWriter htmlToPdfWriter,
            ICommandService command, IApplicationTenant tenant): base(command, tenant)
        {
            _ledgerDocumentService = ledgerService;
            _entryService = entryService;
            _businessReportService = businessReportService;
            _transactionService = transactionService;
            _htmlToPdfWriter = htmlToPdfWriter;
        }

        #region Balance Sheet

        [HttpPost("getBalanceSheet")]
        public BalanceSheetModel GetBalanceSheetReport([FromBody] ReportModel model)
        {
            var report = BuildBalanceSheetModel(model.From, model.To, model.Format, model.Dates);

            return report;
        }

        private BalanceSheetModel BuildBalanceSheetModel(string @from, string to, DateRangeFormatEnum format, int dates)
        {
            var ledger = _ledgerDocumentService.GetById(GetLedgerId());

            #region Dates Handling

            DateTime fromDate;
            DateTime toDate;

            //If user have selected All (Id = 2) from Dates 
            //we are taking ledger first transaction booked date as from and Today as to
            if (dates == 2)
            {
                from = _entryService.GetLedgerMinTransactionDate(ledger.Id).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
                to = DateTime.Now.AddDays(1).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            }
            else if // if user selected Custom dates format and not selected To date
                (dates == 4 && string.IsNullOrEmpty(to))
            {
                from = ledger.CreatedDate.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
                to = DateTime.Now.AddDays(1).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            }

            if (string.IsNullOrEmpty(to))
            {
                var dateFormat = GetReportDateFormats(ledger.FiscalYearStart, ledger.CreatedDate).Single(x => x.Id == dates);
                fromDate = DateTime.Parse(dateFormat.From);
                toDate = DateTime.Parse(dateFormat.To);
            }
            else
            {
                if (string.IsNullOrEmpty(from))
                    from = ledger.CreatedDate.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);

                fromDate = DateTime.Parse(from);
                toDate = DateTime.Parse(to);
            }

            #endregion

            IEnumerable<LedgerAccountBalanceByDay> accountsAggregatedByDay =
                _businessReportService.GetBalanceSheetReportData(fromDate, toDate, ledger);

            var ranges = DateUtil.SplitDateRange(fromDate, toDate, format);
            var includeTotal = ranges.Count == 0 || ranges.Count > 1;

            var model = new BalanceSheetModel
            {
                From = fromDate,
                To = toDate,
                Format = format,
                Formats = GetReportFormats(),
                ReportDateFormats = GetReportDateFormats(ledger.FiscalYearStart, ledger.CreatedDate),
                CurrentReportDateFormat = dates,
                LedgerName = ledger.Name,
                TotalDatesRange =
                    $"As of {toDate.GetFormattedDate(DateTimeFormat.MMMM_space_dd_space_yyyy)}",
                Headers = ranges.Select(x => x.FormattedRange).ToList()
            };
            model.ReportDateFormatsJson = JsonConvert.SerializeObject(model.ReportDateFormats);

            if (includeTotal)
                model.Headers.Add("Total");


            var incomeMatrix = BuildMatrix(ranges, accountsAggregatedByDay.Where(x => x.AccountType == AccountTypeEnum.Income),
                                           "Income", includeTotal);
            var expenseMatrix = BuildMatrix(ranges, accountsAggregatedByDay.Where(x => x.AccountType == AccountTypeEnum.Expense),
                                            "Expense", includeTotal);

            var netIncome = incomeMatrix.TotalRow - expenseMatrix.TotalRow;
            netIncome.Title = "Net Income (Loss)";

            var liabilityMatrix = BuildMatrix(ranges,
                                              accountsAggregatedByDay.Where(x => x.AccountType == AccountTypeEnum.Liability).OrderBy(x => x.Order),
                                              "Liabilities", includeTotal);
            var equityMatrix = BuildMatrix(ranges, accountsAggregatedByDay.Where(x => x.AccountType == AccountTypeEnum.Equity).OrderBy(x => x.Order),
                                           "Equity", includeTotal, netIncome);
            var assetsMatrix = BuildMatrix(ranges, accountsAggregatedByDay.Where(x => x.AccountType == AccountTypeEnum.Asset).OrderBy(x => x.Order),
                                           "ASSETS", includeTotal);
            model.LiabilityMatrix = liabilityMatrix;
            model.EquityMatrix = equityMatrix;
            model.AssetsMatrix = assetsMatrix;
            var liabilityEquityMatrix = new Matrix(ranges.Count == 0 ? 1 : ranges.Count, "LIABILITIES & EQUITY", includeTotal);
            liabilityEquityMatrix.AddRow(liabilityMatrix);
            liabilityEquityMatrix.AddRow(equityMatrix);
            liabilityEquityMatrix.CalculateTotalRow(string.Empty, "Total LIABILITIES & EQUITY", null);
            model.LiabilityEquityMatrix = liabilityEquityMatrix;

            return model;
        }

        #endregion

        #region Profit and Loss
        [HttpPost("getProfitLoss")]
        public ProfitLossModel GetProfitLossReport([FromBody]ReportModel model)
        {
            var report = BuildProfitLossModel(model.From, model.To, model.Format, model.Dates);
            return report;
        }

        private ProfitLossModel BuildProfitLossModel(string @from, string to, DateRangeFormatEnum format, int dates)
        {
            var ledger = _ledgerDocumentService.GetById(GetLedgerId());

            #region Dates Handling

            DateTime fromDate;
            DateTime toDate;

            //If user have selected All (Id = 2) from Dates 
            //we are taking first transaction booked date as from and Today as to
            if (dates == 2)
            {
                from = _entryService.GetLedgerMinTransactionDate(ledger.Id).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
                to = DateTime.Now.AddDays(1).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            }
            // if user selected Custom dates format and not selected dates range
            else if (dates == 4 && (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))) // custom
                dates = 1; // default dates format

            //If user selected custom dates format and not inputed dates we taking dates according to dates format
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                var dateFormat = GetReportDateFormats(ledger.FiscalYearStart, ledger.CreatedDate).Single(x => x.Id == dates);
                fromDate = DateTime.Parse(dateFormat.From);
                toDate = DateTime.Parse(dateFormat.To);
            }
            else
            {
                fromDate = DateTime.Parse(from);
                toDate = DateTime.Parse(to);
            }

            #endregion

            var ranges = DateUtil.SplitDateRange(fromDate, toDate, format);

            var includeTotal = (format != DateRangeFormatEnum.Total && ranges.Count > 1) || ranges.Count == 0;
            var model = new ProfitLossModel
            {
                From = fromDate,
                To = toDate,
                Format = format,
                Formats = GetReportFormats(),
                ReportDateFormats = GetReportDateFormats(ledger.FiscalYearStart, ledger.CreatedDate),
                CurrentReportDateFormat = dates,
                LedgerName = ledger.Name,
                Headers = ranges.Select(x => x.FormattedRange).ToList(),
                TotalDatesRange = new DateRange(fromDate, toDate, false).FormattedRange
            };
            model.ReportDateFormatsJson = JsonConvert.SerializeObject(model.ReportDateFormats);
            if (includeTotal)
                model.Headers.Add("TOTAL");

            IEnumerable<LedgerAccountBalanceByDay> accountsAggregatedByDay =
                _businessReportService.GetProfitLossReportData(fromDate, toDate, ledger);

            var incomeMatrix = BuildMatrix(ranges, accountsAggregatedByDay.Where(x => x.AccountType == AccountTypeEnum.Income),
                                           "Income", includeTotal);
            var expenseMatrix = BuildMatrix(ranges, accountsAggregatedByDay.Where(x => x.AccountType == AccountTypeEnum.Expense),
                                            "Expense", includeTotal);

            model.IncomeMatrix = incomeMatrix;
            model.ExpenseMatrix = expenseMatrix;

            model.NetIncome = incomeMatrix.TotalRow - expenseMatrix.TotalRow;
            model.NetIncome.Title = "Net Income";

            return model;
        }

        #endregion

        #region Transaction detail

        [HttpPost("getTransactionDetail")]
        public TransactionDetailModel GetTransactionDetail([FromBody]DetailModel model)
        {
            var transactionDetail = 
                BuildTransactionDetailModel(model.From, model.To, model.Id, model.Dates, model.P);

            return transactionDetail;
        }

        private TransactionDetailModel BuildTransactionDetailModel(DateTime? @from, DateTime? to, string id, int dates, int p)
        {
            var ledger = _ledgerDocumentService.GetById(GetLedgerId());
            if (ledger?.Users.FirstOrDefault(x => x.Id == GetUserId()) == null)
            {
                throw new Exception(
                    $"Can't find ledger by following account Id '{id}', or ledger not belong to the current user (id: {GetUserId()})");
            }

            #region Dates Handling

            DateTime fromDate;
            DateTime toDate;

            //If user have selected All (Id = 2) from Dates 
            //we are taking ledger first transaction booked date as from and Today as to
            if (dates == 2)
            {
                from = _entryService.GetLedgerMinTransactionDate(ledger.Id);
                to = DateTime.Now.AddDays(1);
            }
            // if user selected Custom dates format and not selected dates range
            else if (dates == 4 && (from == null || to == null)) // custom
            {
                dates = 9; // default dates format
            }

            if (from == null || to == null)
            {
                var dateFormat = GetReportDateFormats(ledger.FiscalYearStart, ledger.CreatedDate).Single(x => x.Id == dates);
                fromDate = DateTime.Parse(dateFormat.From);
                toDate = DateTime.Parse(dateFormat.To);
            }
            else
            {
                fromDate = from.Value;
                toDate = to.Value;
            }

            #endregion

            var accountIds = new List<string> { id };

            if (p == 1) // include sub accounts
            {
                var subAccounts = ledger.Accounts.Where(x => x.ParentAccountId == id);
                accountIds.AddRange(subAccounts.Select(x => x.Id));
            }

            var model = new TransactionDetailModel
            {
                From = from,
                To = to,
                p = p,
                ParentAccountId = id,
                LedgerName = ledger.Name,
                AccountIds = accountIds,
                ParentAccountName = ledger.Accounts.Single(x => x.Id == id).Name,
                ReportDateFormats = GetReportDateFormats(ledger.FiscalYearStart, ledger.CreatedDate),
                CurrentReportDateFormat = dates,
                TotalDatesFormatted = new DateRange(fromDate, toDate, false).FormattedRange
            };

            model.ReportDateFormatsJson = JsonConvert.SerializeObject(model.ReportDateFormats);
            var filter = new TransactionFilter()
            {
                AccountIds = accountIds,
                EntryFromDate = fromDate,
                EntryToDate = toDate,
                SortByFiled = TransactionsSortFieldEnum.BookedDate,
                LedgerId = ledger.Id
            };
            var accountTransactions = _transactionService.GetByFilter(filter);

            model.Transactions = accountTransactions;

            var initialBalance = fromDate == ledger.CreatedDate
                                      ? 0
                                      : _businessReportService.GetAccountBalance(ledger.Id, id, null, fromDate);

            model.InitialBalance = initialBalance;
            return model;
        }

        #endregion

        #region Excel Generation
        [HttpGet("exportProfitLossToExcel")]
        public IActionResult ExportProfitLossToExcel(ReportModel reportModel)
        {
            var model = BuildProfitLossModel(reportModel.From, reportModel.To, reportModel.Format, reportModel.Dates);
            var memoryStream = new MemoryStream();

            var newFile = new FileInfo("x.xlsx");
            using (var package = new ExcelPackage(newFile))
            {
                var worksheet = package.Workbook.Worksheets.Add("Profit & Loss");

                UpdateColumnsWidth(worksheet, 12);
                //Add the headers
                worksheet.Cells[1, 1].Value = model.LedgerName;
                worksheet.Cells[2, 1].Value = "Profit & Loss";
                worksheet.Cells[3, 1].Value = model.TotalDatesRange;

                using (var range = worksheet.Cells[1, 1, 3, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 16;
                    range.Style.Font.Color.SetColor(Color.DarkBlue);
                }


                for (var i = 0; i < model.Headers.Count; i++)
                {
                    worksheet.Cells[4, i + 2].Value = model.Headers[i];
                }
                var rowIndex = 5;
                RenderMatrixToExcel(worksheet, model.IncomeMatrix, model.ColumnsCount, ref rowIndex);
                rowIndex++;
                RenderMatrixToExcel(worksheet, model.ExpenseMatrix, model.ColumnsCount, ref rowIndex);
                rowIndex++;
                AddMatrixRowToExcel(worksheet, model.NetIncome, model.ColumnsCount, rowIndex);

                worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:E5"].Style.Font.Bold = true;
                package.Workbook.Properties.Company = "Mpowering";

                package.SaveAs(memoryStream);

                memoryStream.Position = 0;
            }

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpGet("exportBalanceSheetToExcel")]
        public IActionResult ExportBalanceSheetToExcel(ReportModel reportModel)
        {
            var model = BuildBalanceSheetModel(reportModel.From, reportModel.To, reportModel.Format, reportModel.Dates);
            var memoryStream = new MemoryStream();

            var newFile = new FileInfo("x.xlsx");
            using (var package = new ExcelPackage(newFile))
            {
                var worksheet = package.Workbook.Worksheets.Add("Balance Sheet");
                UpdateColumnsWidth(worksheet, 12);
                worksheet.Cells[1, 1].Value = model.LedgerName;
                worksheet.Cells[2, 1].Value = "Balance Sheet";
                worksheet.Cells[3, 1].Value = model.TotalDatesRange;

                using(var range = worksheet.Cells[1, 1, 3, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 16;
                    range.Style.Font.Color.SetColor(Color.DarkBlue);
                }


                for(var i = 0; i < model.Headers.Count; i++)
                    worksheet.Cells[4, i + 2].Value = model.Headers[i];

                var rowIndex = 5;
                RenderMatrixToExcel(worksheet, model.AssetsMatrix, model.ColumnsCount, ref rowIndex);
                rowIndex++;
                RenderMatrixToExcel(worksheet, model.LiabilityEquityMatrix, model.ColumnsCount, ref rowIndex);

                worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:E5"].Style.Font.Bold = true;
                package.Workbook.Properties.Company = "Mpowering";

                package.SaveAs(memoryStream);
                memoryStream.Position = 0;
            }

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpGet("exportTransactionDetailToExcel")]
        public IActionResult ExportTransactionDetailToExcel(DetailModel detailModel)
        {
            var model = BuildTransactionDetailModel(detailModel.From, detailModel.To, detailModel.Id, detailModel.Dates, detailModel.P);
            var memoryStream = new MemoryStream();

            var newFile = new FileInfo("x.xlsx");
            using(var package = new ExcelPackage(newFile))
            {
                var worksheet = package.Workbook.Worksheets.Add("Transaction Detail");
                UpdateColumnsWidth(worksheet, 12);
                worksheet.Cells[1, 1].Value = model.LedgerName;
                worksheet.Cells[2, 1].Value = $"Transaction Detail By Account {model.ParentAccountName}";
                worksheet.Cells[3, 1].Value = model.TotalDatesFormatted;

                using(var range = worksheet.Cells[1, 1, 3, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 16;
                    range.Style.Font.Color.SetColor(Color.DarkBlue);
                }

                worksheet.Cells[4, 1].Value = "Date";
                worksheet.Cells[4, 2].Value = "Name";
                worksheet.Cells[4, 3].Value = "Debit";
                worksheet.Cells[4, 4].Value = "Credit";
                worksheet.Cells[4, 5].Value = "Balance";

                var balance = model.InitialBalance;
                var rowIndex = 5;
                foreach(var item in model.Transactions)
                {
                    var entry = item.Entries.Single(x => model.AccountIds.Contains(x.AccountId));
                    balance += (entry.DebitAmountInCents - entry.CreditAmountInCents);
                    worksheet.Cells[rowIndex, 1].Value = entry.BookedDateString;
                    worksheet.Cells[rowIndex, 2].Value = item.Id;
                    worksheet.Cells[rowIndex, 3].Value = AccountingFormatter.ConvertToDollarsThenFormat(entry.DebitAmountInCents);
                    worksheet.Cells[rowIndex, 4].Value = AccountingFormatter.ConvertToDollarsThenFormat(entry.CreditAmountInCents);
                    worksheet.Cells[rowIndex, 5].Value = AccountingFormatter.ConvertToDollarsThenFormat(balance);
                    rowIndex++;
                }

                worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A5:E5"].Style.Font.Bold = true;
                package.Workbook.Properties.Company = "Mpowering";

                package.SaveAs(memoryStream);
                memoryStream.Position = 0;
            }

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        private static void RenderMatrixToExcel(ExcelWorksheet worksheet, Matrix matrix, int columnsCount, ref int rowIndex)
        {
            if(!matrix.IsZeroBalanceMatrix())
            {
                worksheet.Cells[rowIndex, 1].Value = matrix.Title;
                for(var i = 0; i < matrix.Rows.Count; i++)
                {
                    var isLastRow = i == matrix.Rows.Count - 1;
                    if(matrix.Rows[i].IsMatrix)
                    {
                        rowIndex++;
                        RenderMatrixToExcel(worksheet, matrix.Rows[i] as Matrix, columnsCount, ref rowIndex);
                    }
                    else
                    {
                        rowIndex++;
                        AddMatrixRowToExcel(worksheet, matrix.Rows[i], columnsCount, rowIndex);
                    }
                }

                rowIndex++;
                AddMatrixRowToExcel(worksheet, matrix.TotalRow, columnsCount, rowIndex);
            }
        }

        private static void AddMatrixRowToExcel(ExcelWorksheet worksheet, IMatrixRow row, int columnsCount, int rowIndex)
        {
            worksheet.Cells[rowIndex, 1].Value = row.Title;
            for(var i = 0; i < columnsCount; i++)
            {
                worksheet.Cells[rowIndex, i + 2].Value = AccountingFormatter.ConvertToDollarsThenFormat(row[i].Amount);
            }
        }

        private static void UpdateColumnsWidth(ExcelWorksheet worksheet, float width)
        {
            for(var i = 1; i < 100; i++)
                worksheet.Column(i).Width = width;

            worksheet.Column(1).Width = 40;
        }

        #endregion

        #region Pdf Generation

        [HttpGet("generateProfitLossPdf")]
        public IActionResult GenerateProfitLossPdf(ReportModel reportModel)
        {
            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var text = FontFactory.GetFont("Arial", 9, Font.NORMAL);

            var model = BuildProfitLossModel(reportModel.From, reportModel.To, reportModel.Format, reportModel.Dates);
            var document = new Document(PageSize.A4, 15, 15, 15, 15);
            var stream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, stream);
            writer.CloseStream = false;
            document.Open();

            document.Add(new Paragraph(model.LedgerName, titleFont));
            document.Add(new Paragraph("Profit & Loss", subTitleFont));
            document.Add(new Paragraph(model.TotalDatesRange, subTitleFont));

            var table = new PdfPTable(model.ColumnsCount + 1)
            {
                HorizontalAlignment = 0,
                SpacingBefore = 10,
                SpacingAfter = 10,
                TotalWidth = 560f,
                LockedWidth = true,
            };
            table.DefaultCell.Border = 0;

            var cells = new List<string> { string.Empty };
            cells.AddRange(model.Headers);
            foreach(var cell in cells)
                table.AddCell(new PdfPCell(new Phrase(cell, text)));

            RenderMatrixToPdf(table, model.IncomeMatrix, model.ColumnsCount, text);
            RenderMatrixToPdf(table, model.ExpenseMatrix, model.ColumnsCount, text);
            RenderMatrixRowToPdf(model.NetIncome, table, model.ColumnsCount, text);

            document.Add(table);
            document.Close();

            stream.Position = 0;
            return File(stream, "application/pdf");
        }

        [HttpGet("generateBalanceSheetPdf")]
        public IActionResult GenerateBalanceSheetPdf(ReportModel reportModel)
        {
            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var text = FontFactory.GetFont("Arial", 9, Font.NORMAL);

            var model = BuildBalanceSheetModel(reportModel.From, reportModel.To, reportModel.Format, reportModel.Dates);
            var document = new Document(PageSize.A4, 15, 15, 15, 20);
            var stream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, stream);
            writer.CloseStream = false;
            document.Open();

            document.Add(new Paragraph(model.LedgerName, titleFont));
            document.Add(new Paragraph("Balance Sheet", subTitleFont));
            document.Add(new Paragraph(model.TotalDatesRange, subTitleFont));

            var table = new PdfPTable(model.ColumnsCount + 1)
            {
                HorizontalAlignment = 0,
                SpacingBefore = 10,
                SpacingAfter = 10,
                TotalWidth = 560f,
                LockedWidth = true
            };
            table.DefaultCell.Border = 0;

            var cells = new List<string> { string.Empty };
            cells.AddRange(model.Headers);
            foreach(var cell in cells)
                table.AddCell(new PdfPCell(new Phrase(cell, text)));

            RenderMatrixToPdf(table, model.AssetsMatrix, model.ColumnsCount, text);
            RenderMatrixToPdf(table, model.LiabilityEquityMatrix, model.ColumnsCount, text);

            document.Add(table);
            document.Close();

            stream.Position = 0;
            return File(stream, "application/pdf");
        }

        [HttpGet("generateTransactionDetailToPdf")]
        public IActionResult GenerateTransactionDetailToPdf(DetailModel detailModel)
        {
            const int numberColumns = 5;
            var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var subTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var text = FontFactory.GetFont("Arial", 9, Font.NORMAL);

            var model = BuildTransactionDetailModel(detailModel.From, detailModel.To, detailModel.Id, detailModel.Dates, detailModel.P);
            var memoryStream = new MemoryStream();

            var document = new Document(PageSize.A4, 15, 15, 15, 20);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            writer.CloseStream = false;
            document.Open();

            document.Add(new Paragraph(model.LedgerName, titleFont));
            document.Add(new Paragraph($"Transaction Detail By Account {model.ParentAccountName}", subTitleFont));
            document.Add(new Paragraph(model.TotalDatesFormatted, subTitleFont));

            var table = new PdfPTable(numberColumns)
            {
                HorizontalAlignment = 0,
                SpacingBefore = 10,
                SpacingAfter = 10,
                TotalWidth = 560f,
                LockedWidth = true
            };
            table.DefaultCell.Border = 0;
            table.AddCell(new PdfPCell(new Phrase("Date", text)));
            table.AddCell(new PdfPCell(new Phrase("Memo", text)));
            table.AddCell(new PdfPCell(new Phrase("Debit", text)));
            table.AddCell(new PdfPCell(new Phrase("Credit", text)));
            table.AddCell(new PdfPCell(new Phrase("Balance", text)));

            var balance = model.InitialBalance;
            foreach(var item in model.Transactions)
            {
                var entry = item.Entries.Single(x => model.AccountIds.Contains(x.AccountId));
                balance += (entry.DebitAmountInCents - entry.CreditAmountInCents);

                table.AddCell(new PdfPCell(new Phrase(entry.BookedDateString, text)));
                table.AddCell(new PdfPCell(new Phrase(item.Memo, text)));
                table.AddCell(new PdfPCell(new Phrase(AccountingFormatter.ConvertToDollarsThenFormat(entry.DebitAmountInCents), text)));
                table.AddCell(new PdfPCell(new Phrase(AccountingFormatter.ConvertToDollarsThenFormat(entry.CreditAmountInCents), text)));
                table.AddCell(new PdfPCell(new Phrase(AccountingFormatter.ConvertToDollarsThenFormat(balance), text)));
            }

            document.Add(table);
            document.Close();
            memoryStream.Position = 0;

            return File(memoryStream, "application/pdf");
        }

        private static void RenderMatrixRowToPdf(IMatrixRow row, PdfPTable table, int columnsCount, Font text)
        {
            var cells = new List<string> {row.Title};
            for (var i = 0; i < columnsCount; i++)
                cells.Add(AccountingFormatter.ConvertToDollarsThenFormat(row[i].Amount));
            foreach (var cell in cells)
                table.AddCell(new PdfPCell(new Phrase(cell, text)));
        }

        private static void RenderMatrixToPdf(PdfPTable table, Matrix matrix, int columnsCount, Font text)
        {
            if(!matrix.IsZeroBalanceMatrix())
            {
                var cells = new List<string> { matrix.Title };
                for(var i = 0; i < columnsCount; i++)
                    cells.Add(string.Empty);
                foreach(var cell in cells)
                    table.AddCell(new PdfPCell(new Phrase(cell, text)));

                for(var i = 0; i < matrix.Rows.Count; i++)
                {
                    if(matrix.Rows[i].IsMatrix)
                        RenderMatrixToPdf(table, matrix.Rows[i] as Matrix, columnsCount, text);
                    else
                        RenderMatrixRowToPdf(matrix.Rows[i], table, columnsCount, text);
                }

                RenderMatrixRowToPdf(matrix.TotalRow, table, columnsCount, text);
            }
        }

        #endregion

        #region private
        private static Matrix BuildMatrix(List<DateRange> dateRanges, IEnumerable<LedgerAccountBalanceByDay> accounts, string accountName, bool includeTotal, MatrixRow insertRow = null)
        {
            //if user select All dates options ranges count is equal to zero
            if(dateRanges.Count == 0)
                dateRanges.Add(DateRange.CreateEmpty());

            var rangesCount = dateRanges.Count;
            var matrix = new Matrix(rangesCount, accountName, includeTotal);

            foreach(var account in accounts)
            {
                if(account.SubAccounts.Count > 0)
                {
                    var subMatrix = new Matrix(rangesCount, account.Name, includeTotal);

                    foreach(var subAccount in account.SubAccounts)
                        subMatrix.AddRow(subAccount.AccountId, subAccount.Name, AggregateByDateRanges(subAccount.AmountPerDay, dateRanges));

                    //in case if parent account has his own transactions
                    if(account.AmountPerDay.Count > 0)
                        subMatrix.AddRow(account.AccountId, $"{account.Name} - Other", AggregateByDateRanges(account.AmountPerDay, dateRanges));

                    subMatrix.CalculateTotalRow(account.AccountId, $"Total {account.Name}", dateRanges);
                    matrix.AddRow(subMatrix);
                }
                else
                    matrix.AddRow(account.AccountId, account.Name, AggregateByDateRanges(account.AmountPerDay, dateRanges));
            }

            if(insertRow != null)
                matrix.AddRow(insertRow);

            matrix.CalculateTotalRow(string.Empty, $"Total {accountName}", null);

            return matrix;
        }

        private static List<DateFormat> GetReportDateFormats(DateTime fiscalYearStart, DateTime startDate)
        {
            var fiscalQuarter = new FiscalQuarter(fiscalYearStart);

            var result = new List<DateFormat>();
            var thisMonthFirstDay = DateTime.Now.GetStartOfMonth().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var thisMonthLastDay = DateTime.Now.GetEndOfMonth().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var today = DateTime.Now.Date.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var startThisFiscalQuarter = fiscalQuarter.GetStartOfCurrentQuarter().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var endThisFiscalQuarter = fiscalQuarter.GetEndOfCurrentQuarter().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var lastMonthStart = DateUtil.GetStartOfLastMonth().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var lastMonthEnd = DateUtil.GetEndOfLastMonth().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);

            var currFiscalYearStart = DateUtil.GetCurrentFiscalYearStart(fiscalYearStart);
            var startThisFiscalYear = currFiscalYearStart.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var endThisFiscalYear = currFiscalYearStart.AddYears(1).AddMilliseconds(-1).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);
            var startDay = startDate.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy);

            var from = thisMonthFirstDay;
            var to = today;
            result.Add(new DateFormat() { Id = 1, Name = "This month to date", From = from, To = to });
            result.Add(new DateFormat() { Id = 2, Name = "All", From = startDay, To = today });
            from = thisMonthFirstDay;
            to = thisMonthLastDay;
            result.Add(new DateFormat() { Id = 3, Name = "This Month", From = from, To = to });
            result.Add(new DateFormat() { Id = 4, Name = "Custom", From = String.Empty, To = String.Empty });
            result.Add(new DateFormat() { Id = 5, Name = "Today", From = today, To = today });
            result.Add(new DateFormat() { Id = 6, Name = "This Fiscal Quarter", From = startThisFiscalQuarter, To = endThisFiscalQuarter });
            result.Add(new DateFormat() { Id = 7, Name = "This Fiscal Quarter-to-date", From = startThisFiscalQuarter, To = today });
            result.Add(new DateFormat() { Id = 8, Name = "This Fiscal Year", From = startThisFiscalYear, To = endThisFiscalYear });
            result.Add(new DateFormat() { Id = 9, Name = "This Fiscal Year-to-date", From = startThisFiscalYear, To = today });

            result.Add(new DateFormat() { Id = 10, Name = "Last Month", From = lastMonthStart, To = lastMonthEnd });
            result.Add(new DateFormat() { Id = 11, Name = "Last Month-to-date", From = lastMonthStart, To = today });
            result.Add(new DateFormat()
            {
                Id = 12,
                Name = "Last Fiscal Quarter",
                From = fiscalQuarter.GetStartOfLastQuarter().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy),
                To = fiscalQuarter.GetEndOfLastQuarter().GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy)
            });
            result.Add(new DateFormat()
            {
                Id = 13,
                Name = "Last Fiscal Year",
                From = currFiscalYearStart.AddYears(-1).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy),
                To = currFiscalYearStart.AddMilliseconds(-1).GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy)
            });

            return result;
        }

        private static Dictionary<int, string> GetReportFormats()
        {
            var formats = new Dictionary<int, string> { { 1, "Total" }, { 2, "Month" }, { 3, "Quarter" }, { 4, "Year" } };

            return formats;
        }

        private static RowValue[] AggregateByDateRanges(IEnumerable<DateAmount> amounts, List<DateRange> dateRanges)
        {
            var result = new RowValue[dateRanges.Count];

            for(var i = 0; i < dateRanges.Count; i++)
            {
                result[i] = new RowValue
                {
                    Amount = dateRanges[i].IsEmpty()
                        ? amounts.Sum(item => item.Amount) // for the empty date range we just calculating all values
                        : amounts.Where(item => item.Date >= dateRanges[i].From && item.Date <= dateRanges[i].To)
                            .Sum(item => item.Amount),
                    Range = dateRanges[i]
                };
            }

            return result;
        }

        private IActionResult PdfResponse(string fileName, string view, object model)
        {
            var stream = new MemoryStream();
            _htmlToPdfWriter.GeneratePdf(null, stream, Size.Empty);

            stream.Position = 0;
            return File(stream, "application/pdf");
        }
        #endregion
    }
}
