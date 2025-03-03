using System.Globalization;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Data.Models;
using ClosedXML.Excel;
using Serilog;

namespace Ajinomoto.Arc.Business.Modules
{
    public partial class IncomingPaymentService
    {
        private ResultBase ImportInvoiceTemplate01(string filePath)
        {
            var result = new ResultBase
            {
                Success = false,
                Message = ""
            };
            List<string> sapFileNeedToDeleteList = new List<string>();

            try
            {
                var userLogin = _profileService.GetUserLogin();

                var now = DateTime.Now;
                var currentUser = userLogin.Username;
                var currentApp = userLogin.App;

                using (XLWorkbook wb = new XLWorkbook(filePath))
                {
                    // IXLWorksheet ws = wb.Worksheet(1); // Use the first sheet (Sheet1)
                    var ws = wb.Worksheets.FirstOrDefault();
                    if (ws == null)
                    {
                        result.Message = "The required worksheet cannot be found.";
                        return result;
                    }
                    // Define expected headers based on the Excel file structure
                    var expectedHeaders = new List<string>
                    {
                        "Cabang", "Customer Name", "Sales Grup", "Fiscal Year", "ID Customer Sold To",
                        "Document Number", "No Invoice", "No PO", "Amt in loc.cur.", "Ship To", "Store",
                        "Text", "Doc. Date", "Baseline Date", "Net due dt", "No Submitted",
                        "Status Tukar Faktur", "Status", "Action", "BusA", "Tgl Kirim Barang / Invoice",
                        "Tempat Tukar Faktur", "TgL Kirim Berkas Ke KA-MT", "Tgl terima DO back",
                        "Tgl terima faktur pajak", "Tgl completed doc", "Tgl Tukar Faktur", "tanggal bayar",
                        "Tgl Terima Berkas", "Kirim Barang s.d. Kirim Berkas", "Lama Kirim Berkas s.d. Terima Berkas",
                        "Lama Terima Berkas s.d. TTF", "Lama Kirim Berkas s.d. TTF", "Lama Kirim Barang s.d. TTF",
                        "Ideal Tukar Faktur", "TOP Outlet", "TOP Database", "Jatuh Tempo Outlet",
                        "Jatuh Tempo (Database SAP)", "Overdue Database SAP", "Status Overdue Database",
                        "Overdue Outlet s.d. Hari Ini", "Status Internal (Database SAP)",
                        "Status External (TOP Outlet)", "Keterangan", "Action", "Realisasi"
                    };

                    // Validate headers
                    for (int col = 1; col <= expectedHeaders.Count; col++)
                    {
                        var headerValue = ws.Cell(1, col).Value.ToString();
                        if (!string.Equals(headerValue?.Trim(), expectedHeaders[col - 1].Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            result.Message = $"Header mismatch at column {col}. Expected: {expectedHeaders[col - 1]}, Found: {headerValue}";
                            return result;
                        }
                    }

                    var initialRow = 2; // Data starts from row 2 (row 1 is headers)
                    var lastRow = ws.LastRowUsed().RowNumber();
                        // Console.WriteLine("Worksheet cek: " + cabang);

                    for (int i = initialRow; i <= lastRow; i++)
                    {
                        // Read data from each column
                        var cabang = ws.Cell(i, 1).Value.ToString();
                        var customerName = ws.Cell(i, 2).Value.ToString();
                        var salesGrup = ws.Cell(i, 3).Value.ToString();
                        var fiscalYear = ws.Cell(i, 4).Value.ToString();
                        var idCustomerSoldTo = ws.Cell(i, 5).Value.ToString();
                        var documentNumber = ws.Cell(i, 6).Value.ToString();
                        var noInvoice = ws.Cell(i, 7).Value.ToString();
                        var noPO = ws.Cell(i, 8).Value.ToString();
                        var amtInLocCur = ws.Cell(i, 9).Value.ToString();
                        var shipTo = ws.Cell(i, 10).Value.ToString();
                        var store = ws.Cell(i, 11).Value.ToString();
                        var text = ws.Cell(i, 12).Value.ToString();
                        var docDate = ws.Cell(i, 13).Value.ToString();
                        var baselineDate = ws.Cell(i, 14).Value.ToString();
                        var netDueDt = ws.Cell(i, 15).Value.ToString();
                        var noSubmitted = ws.Cell(i, 16).Value.ToString();
                        var statusTukarFaktur = ws.Cell(i, 17).Value.ToString();
                        var status = ws.Cell(i, 18).Value.ToString();
                        var action = ws.Cell(i, 19).Value.ToString();
                        var busA = ws.Cell(i, 20).Value.ToString();
                        var tglKirimBarang = ws.Cell(i, 21).Value.ToString();
                        var tempatTukarFaktur = ws.Cell(i, 22).Value.ToString();
                        var tglKirimBerkasKeKAMT = ws.Cell(i, 23).Value.ToString();
                        var tglTerimaDOBack = ws.Cell(i, 24).Value.ToString();
                        var tglTerimaFakturPajak = ws.Cell(i, 25).Value.ToString();
                        var tglCompletedDoc = ws.Cell(i, 26).Value.ToString();
                        var tglTukarFaktur = ws.Cell(i, 27).Value.ToString();
                        var tanggalBayar = ws.Cell(i, 28).Value.ToString();
                        var tglTerimaBerkas = ws.Cell(i, 29).Value.ToString();
                        var kirimBarangSdKirimBerkas = ws.Cell(i, 30).Value.ToString();
                        var lamaKirimBerkasSdTerimaBerkas = ws.Cell(i, 31).Value.ToString();
                        var lamaTerimaBerkasSdTTF = ws.Cell(i, 32).Value.ToString();
                        var lamaKirimBerkasSdTTF = ws.Cell(i, 33).Value.ToString();
                        var lamaKirimBarangSdTTF = ws.Cell(i, 34).Value.ToString();
                        var idealTukarFaktur = ws.Cell(i, 35).Value.ToString();
                        var topOutlet = ws.Cell(i, 36).Value.ToString();
                        var topDatabase = ws.Cell(i, 37).Value.ToString();
                        var jatuhTempoOutlet = ws.Cell(i, 38).Value.ToString();
                        var jatuhTempoDatabaseSAP = ws.Cell(i, 39).Value.ToString();
                        var overdueDatabaseSAP = ws.Cell(i, 40).Value.ToString();
                        var statusOverdueDatabase = ws.Cell(i, 41).Value.ToString();
                        var overdueOutletSdHariIni = ws.Cell(i, 42).Value.ToString();
                        var statusInternalSAP = ws.Cell(i, 43).Value.ToString();
                        var statusExternalTOPOutlet = ws.Cell(i, 44).Value.ToString();
                        var keterangan = ws.Cell(i, 45).Value.ToString();
                        var actionCabangMT = ws.Cell(i, 46).Value.ToString();
                        var realisasiCabangMT = ws.Cell(i, 47).Value.ToString();

                        // Process the data (e.g., save to database)
                        var invoiceDetails = new InvoiceDetails
                        {
                            // Generate a UUID for the primary key
                            InvoiceDetailsId = Guid.NewGuid().ToString("N"), // 32-character UUID without hyphens

                            // Map all fields from the input parameters
                            CabangId = cabang,
                            CustomerName = customerName,
                            SalesGrup = salesGrup,
                            FiscalYear = fiscalYear,
                            IDCustomerSoldTo = TryParseInt(idCustomerSoldTo, "IDCustomerSoldTo", i),
                            DocumentNumber = TryParseInt(documentNumber, "DocumentNumber", i),
                            NoInvoice = noInvoice,
                            NoPO = noPO,
                            AmtInLocCur = TryParseDoubleWithSeparator(amtInLocCur, "AmtInLocCur", i), // Handle thousand separators
                            ShipTo = TryParseInt(shipTo, "ShipTo", i),
                            Store = store,
                            TextDesc = text, // Adjusted to match the database column name
                            DocDate = TryParseCustomDate(docDate, "DocDate", i), // Handle custom date format
                            BaselineDate = TryParseCustomDate(baselineDate, "BaselineDate", i),
                            NetDueDt = TryParseCustomDate(netDueDt, "NetDueDt", i),
                            NoSubmitted = noSubmitted,
                            StatusTukarFaktur = string.IsNullOrEmpty(statusTukarFaktur) ? "Not Ok" : statusTukarFaktur, // Default value
                            Status = string.IsNullOrEmpty(status) ? "Not Created" : status, // Default value
                            Action = string.IsNullOrEmpty(action) ? "Draft" : action, // Default value
                            BusA = TryParseInt(busA, "BusA", i),
                            TglKirimBarang = TryParseCustomDate(tglKirimBarang, "TglKirimBarang", i),
                            TempatTukarFaktur = tempatTukarFaktur,
                            TgKirimBerkasKeKAMT = TryParseCustomDate(tglKirimBerkasKeKAMT, "TgKirimBerkasKeKAMT", i),
                            TglTerimaDOBack = TryParseCustomDate(tglTerimaDOBack, "TglTerimaDOBack", i),
                            TglTerimaFakturPajak = TryParseCustomDate(tglTerimaFakturPajak, "TglTerimaFakturPajak", i),
                            TglCompletedDoc = TryParseCustomDate(tglCompletedDoc, "TglCompletedDoc", i),
                            TglTukarFaktur = TryParseCustomDate(tglTukarFaktur, "TglTukarFaktur", i),
                            TanggalBayar = TryParseCustomDate(tanggalBayar, "TanggalBayar", i),
                            TglTerimaBerkas = TryParseCustomDate(tglTerimaBerkas, "TglTerimaBerkas", i),
                            IdealTukarFaktur = TryParseInt(idealTukarFaktur, "IdealTukarFaktur", i),
                            TOPOutlet = TryParseInt(topOutlet, "TOPOutlet", i),
                            OverdueDatabaseSAP = overdueDatabaseSAP,
                            StatusOverdueDatabase = statusOverdueDatabase,
                            StatusInternalSAP = statusInternalSAP,
                            StatusExternalTOPOutlet = statusExternalTOPOutlet,
                            Keterangan = keterangan,
                            ActionCabangMT = actionCabangMT,
                            RealisasiCabangMT = realisasiCabangMT,

                            // Set default values for created_at and created_by
                            CreatedBy = currentUser // Convert currentUser to int
                        };

                        // Save to database (example)
                        Console.WriteLine("Worksheet cek: " + invoiceDetails);
                        _domainService.InsertInvoiceDetails(invoiceDetails);
                    }
                }

                result.Success = true;
                result.Message = "Invoice details uploaded successfully.";
                _domainService.SaveChanges();

                return result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Method: ImportInvoiceTemplate01, filePath: {filePath}, Message: {ex.Message}");
                result.Message = $"Method: ImportInvoiceTemplate01, filePath: {filePath}, Message: {ex.Message}";
                // result.Message = "An error occurred while processing the file.";
                return result;
            }
            finally
            {
                if (result.Success)
                {
                    foreach (var sapFilePath in sapFileNeedToDeleteList)
                    {
                        File.Delete(sapFilePath);
                    }
                }

                File.Delete(filePath);
            }
        }

        // Helper methods for parsing with logging
        private int? TryParseInt(string value, string fieldName, int row)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                return int.Parse(value);
            }
            catch (FormatException)
            {
                Log.Logger.Error($"Invalid format for {fieldName} in row {row}. Value: {value}");
                return null; // Return null if parsing fails
            }
            catch (OverflowException)
            {
                Log.Logger.Error($"Value for {fieldName} in row {row} is too large or too small for an Int32. Value: {value}");
                return null; // Return null if the value is out of range
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Error parsing {fieldName} in row {row}: {ex.Message}");
                return null; // Return null for any other exception
            }
        }

        private double? TryParseDoubleWithSeparator(string value, string fieldName, int row)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                // Remove thousand separators (e.g., commas) and parse
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                Log.Logger.Error($"Invalid format for {fieldName} in row {row}. Value: {value}");
                return null; // Return null if parsing fails
            }
            catch (OverflowException)
            {
                Log.Logger.Error($"Value for {fieldName} in row {row} is too large or too small for a Double. Value: {value}");
                return null; // Return null if the value is out of range
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Error parsing {fieldName} in row {row}: {ex.Message}");
                return null; // Return null for any other exception
            }
        }

        private DateOnly? TryParseCustomDate(string value, string fieldName, int row)
        {
            if (string.IsNullOrEmpty(value))
            return null;

            try
            {
            // Handle Excel serial dates (e.g., 45472)
            if (double.TryParse(value, out double serialDate))
            {
                return DateOnly.FromDateTime(DateTime.FromOADate(serialDate));
            }

            // Handle custom date format (e.g., "yyyy-MM-dd")
            return DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
            Log.Logger.Error($"Invalid format for {fieldName} in row {row}. Value: {value}");
            return null; // Return null if parsing fails
            }
            catch (Exception ex)
            {
            Log.Logger.Error($"Error parsing {fieldName} in row {row}: {ex.Message}");
            return null; // Return null for any other exception
            }
        }
    }
}