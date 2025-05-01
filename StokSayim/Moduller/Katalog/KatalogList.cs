using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using StokSayim.Data.Repositories;
using StokSayim.Data.Services;
using DevExpress.XtraGrid.Views.Grid;
using StokSayim.Models;
using StokSayim.Helpers;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraBars.Docking2010.Customization;

namespace StokSayim.Moduller.Katalog
{
    public partial class KatalogList : DevExpress.XtraEditors.XtraUserControl
    {
        FlyoutProperties properties = new FlyoutProperties();

        private CatalogRepository _catalogRepository;
        private BulkImportService _bulkImportService;
        private int _selectedBrandId;
        private string _selectedBrandName;

        private int _pageSize = 100; // Varsayılan olarak 100 kayıt
        private int _totalRecords = 0; // Toplam kayıt sayısı

        public KatalogList(CatalogRepository catalogRepository, BulkImportService bulkImportService, int brandId, string brandName)
        {
            InitializeComponent();
            properties.Style = FlyoutStyle.Popup;

            _catalogRepository = catalogRepository;
            _bulkImportService = bulkImportService;
            _selectedBrandId = brandId;
            _selectedBrandName = brandName;
        }

   
        private void KatalogList_Load(object sender, EventArgs e)
        {         
            labelControl.Text = $"{_selectedBrandName} Kataloğu";       
            LoadCatalogData();
        }

        private void LoadCatalogData()
        {
            try
            {
                // Önce toplam kayıt sayısını al
                var allItems = _catalogRepository.GetAllByBrandDirect(_selectedBrandId);
                _totalRecords = allItems.Count;
                lblToplamUrun.Text = string.Format("Toplam Kayıt: {0:#,##0}", _totalRecords);

                // Sayfalama mantığına göre kısıtlı sayıda veri al
                var catalogItems = allItems;
                if (_pageSize > 0 && _pageSize < _totalRecords)
                {
                    catalogItems = allItems.Take(_pageSize).ToList();
                }

                // Özel alanları bulmak için bir katalog öğesinden tüm CustomFields anahtarlarını al
                var customFieldKeys = _catalogRepository.GetUniqueCustomFieldKeys(_selectedBrandId);

                // DataTable oluştur
                DataTable dataTable = new DataTable();

                // Standart sütunları ekle
                dataTable.Columns.Add("ItemID", typeof(long));
                dataTable.Columns.Add("Barcode", typeof(string));
                dataTable.Columns.Add("Description", typeof(string));
                dataTable.Columns.Add("Category", typeof(string));
                dataTable.Columns.Add("BrandID", typeof(int));
                dataTable.Columns.Add("CustomFields", typeof(string));

                // Özel alanlar için sütunlar ekle
                foreach (var key in customFieldKeys)
                {
                    dataTable.Columns.Add(key, typeof(string));
                }

                // Verileri DataTable'a ekle
                foreach (var item in catalogItems)
                {
                    DataRow row = dataTable.NewRow();

                    // Standart alanları ekle
                    row["ItemID"] = item.ItemID;
                    row["Barcode"] = item.Barcode ?? string.Empty;
                    row["Description"] = item.Description ?? string.Empty;
                    row["Category"] = item.Category ?? string.Empty;
                    row["BrandID"] = item.BrandID;
                    row["CustomFields"] = item.CustomFields ?? string.Empty;

                    // Özel alanları ekle
                    if (!string.IsNullOrEmpty(item.CustomFields))
                    {
                        try
                        {
                            var customFields = JsonHelper.DeserializeCustomFields(item.CustomFields);
                            foreach (var field in customFields)
                            {
                                if (dataTable.Columns.Contains(field.Key))
                                {
                                    row[field.Key] = field.Value?.ToString() ?? string.Empty;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // JSON ayrıştırma hatası durumunda
                            Console.WriteLine($"JSON ayrıştırma hatası: {ex.Message}");
                        }
                    }

                    dataTable.Rows.Add(row);
                }

                // Grid'e veriyi bağla
                gridControlKatalog.DataSource = dataTable;

                // Grid görünümünü özelleştir
                gridViewKatalog.OptionsBehavior.Editable = false;
                gridViewKatalog.OptionsView.ShowGroupPanel = false;

                // Sütun başlıklarını ayarla
                gridViewKatalog.Columns["ItemID"].Visible = false;
                gridViewKatalog.Columns["BrandID"].Visible = false;
                gridViewKatalog.Columns["CustomFields"].Visible = false;

                gridViewKatalog.Columns["Barcode"].Caption = "Barkod";
                gridViewKatalog.Columns["Description"].Caption = "Açıklama";
                gridViewKatalog.Columns["Category"].Caption = "Kategori";

                // Özel sütunlar için başlıkları ayarla
                foreach (var key in customFieldKeys)
                {
                    if (gridViewKatalog.Columns[key] != null)
                    {
                        gridViewKatalog.Columns[key].Caption = key;
                    }
                }

                // Gösterilen kayıt sayısı ve toplam kayıt sayısı bilgisini göster
                string mesaj = $"{gridViewKatalog.RowCount} Kayıt Listelendi";
                if (_pageSize > 0 && _totalRecords > _pageSize)
                {
                    mesaj += $" (Toplam: {_totalRecords})";
                }

                XtraMessageBox.Show(mesaj, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Katalog verileri yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupCustomFieldColumns()
        {
            try
            {
                // Katalog öğelerinden benzersiz özel alan anahtarlarını al
                var customFields = _catalogRepository.GetUniqueCustomFieldKeys(_selectedBrandId);

                // Her özel alan için bir sütun ekle
                foreach (var field in customFields)
                {
                    // Sütunu ekle
                    DevExpress.XtraGrid.Columns.GridColumn column = gridViewKatalog.Columns.AddField($"CustomField_{field}");
                    column.Caption = field;
                    column.VisibleIndex = gridViewKatalog.Columns.Count;
                    column.UnboundType = DevExpress.Data.UnboundColumnType.String;

                    // Enable filtering for this column
                    column.OptionsFilter.AllowFilter = true;
                    column.OptionsFilter.AllowAutoFilter = true;
                }

                // Unbound sütunları doldurmak için event'i bağla
                gridViewKatalog.CustomUnboundColumnData += GridViewKatalog_CustomUnboundColumnData;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Özel alanlar yapılandırılırken hata oluştu: {ex.Message}", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GridViewKatalog_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName.StartsWith("CustomField_"))
            {
                GridView view = sender as GridView;
                var row = view.GetRow(e.ListSourceRowIndex) as CatalogItem;
                string fieldName = e.Column.FieldName.Replace("CustomField_", "");

                if (e.IsGetData) // When retrieving data to display
                {
                    if (row != null && !string.IsNullOrEmpty(row.CustomFields))
                    {
                        try
                        {
                            e.Value = JsonHelper.ExtractFieldFromJson(row.CustomFields, fieldName);
                        }
                        catch
                        {
                            e.Value = string.Empty;
                        }
                    }
                }
                else if (e.IsSetData) // When setting data (during filtering)
                {
                    // No implementation needed for setting data since we're using read-only grid
                }
            }
        }


        private void YeniKatalogYukle()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Dosyaları|*.xlsx;*.xls|Metin Dosyaları|*.txt;*.csv|Tüm Dosyalar|*.*";
                openFileDialog.Title = "Katalog Dosyasını Seçin";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string extension = System.IO.Path.GetExtension(filePath).ToLower();

                    try
                    {
                        // Dosya türüne göre işlem yap
                        DataTable sourceData;
                        string errorMessage;

                        if (extension == ".xlsx" || extension == ".xls")
                        {
                            sourceData = ExcelHelper.ReadExcelFile(filePath, out errorMessage);
                        }
                        else if (extension == ".txt" || extension == ".csv")
                        {
                            sourceData = TextFileHelper.ReadTextFile(filePath, out errorMessage);
                        }
                        else
                        {
                            XtraMessageBox.Show("Desteklenmeyen dosya formatı.", "Uyarı",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (sourceData == null)
                        {
                            XtraMessageBox.Show(errorMessage, "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Katalog verilerini önizleme ve eşleştirme formu göster
                        using (var previewForm = new KatalogOnizlemeForm(sourceData, _selectedBrandId, _selectedBrandName, _bulkImportService))
                        {

                            FlyoutProperties properties = new FlyoutProperties();
                            properties.Style = FlyoutStyle.Popup;
                            var result = FlyoutDialog.Show(MainMenu.ActiveForm, previewForm, properties);
                           
                            // burada ok ise yapıcaz.
                            LoadCatalogData();
                            XtraMessageBox.Show("Katalog başarıyla yüklendi.", "Bilgi",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Katalog yükleme sırasında hata oluştu: {ex.Message}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void windowsUIButtonPanel_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            //

            var caption = e.Button.Properties.Caption;
            if (caption == "Çıkış") (this.Parent as Form).DialogResult = DialogResult.No;
            else if (caption == "Katalog Yükle") YeniKatalogYukle();
            else if (caption == "Dışa Aktar") DisaAktar();
            else if (caption == "Listele") LoadCatalogData();
        }

        private void DisaAktar()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Dosyası (*.xlsx)|*.xlsx"; // Sadece .xlsx dosyası göster
                sfd.Title = "Excel Olarak Kaydet";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    gridControlKatalog.ExportToXlsx(sfd.FileName);
                    MessageBox.Show("Export başarıyla tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void radioGroupKayitSayisi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroupKayitSayisi.SelectedIndex >= 0)
            {
                // Seçilen değeri al
                var selectedItem = radioGroupKayitSayisi.Properties.Items[radioGroupKayitSayisi.SelectedIndex];
                if (selectedItem != null)
                {
                    _pageSize = Convert.ToInt32(selectedItem.Value);
                    // Listeyi yeniden yükle
                    LoadCatalogData();
                }
            }
        }
    }
}
