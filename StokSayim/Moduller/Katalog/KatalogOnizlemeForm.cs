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
using StokSayim.DTOs;
using StokSayim.Data.Services;
using DevExpress.XtraPrinting.Native;
using StokSayim.Helpers;

namespace StokSayim.Moduller.Katalog
{
    public partial class KatalogOnizlemeForm : DevExpress.XtraEditors.XtraUserControl
    {

        private DataTable _sourceData;
        private int _brandId;
        private string _brandName;
        private BulkImportService _bulkImportService;
        private List<ColumnMapping> _columnMappings;

        public KatalogOnizlemeForm(DataTable sourceData, int brandId, string brandName, BulkImportService bulkImportService)
        {
            InitializeComponent();

            _sourceData = sourceData;
            _brandId = brandId;
            _brandName = brandName;
            _bulkImportService = bulkImportService;
        }

        private void KatalogOnizlemeForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{_brandName} - Katalog Önizleme";

            // Veri önizleme grid'ini doldur
            gridControlOnizleme.DataSource = _sourceData;

            // Otomatik eşleştirmeleri oluştur
            if (_sourceData.Columns.Count > 0)
            {
                if (System.IO.Path.GetExtension(_sourceData.TableName).ToLower() == ".xlsx" ||
                    System.IO.Path.GetExtension(_sourceData.TableName).ToLower() == ".xls")
                {
                    _columnMappings = StokSayim.Helpers.ExcelHelper.GenerateColumnMappings(_sourceData);
                }
                else
                {
                    _columnMappings = TextFileHelper.GenerateColumnMappings(_sourceData);
                }

                // Eşleştirme grid'ini doldur
                gridControlMapping.DataSource = _columnMappings;

                // Repository oluştur (sütun eşleştirme için combobox)
                DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryComboBox =
                    new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();

                // Excel/TXT sütunlarını ekle
                foreach (DataColumn column in _sourceData.Columns)
                {
                    repositoryComboBox.Items.Add(column.ColumnName);
                }

                // Combobox'ı SourceColumn kolonuna bağla
                gridViewMapping.Columns["SourceColumn"].ColumnEdit = repositoryComboBox;

                // Grid görünümünü özelleştir
                gridViewMapping.Columns["DestinationField"].Caption = "Hedef Alan";
                gridViewMapping.Columns["SourceColumn"].Caption = "Kaynak Sütunu";
                gridViewMapping.Columns["IsCustomField"].Caption = "Özel Alan";
                gridViewMapping.Columns["IsRequired"].Caption = "Zorunlu";

                gridViewMapping.Columns["IsCustomField"].OptionsColumn.ReadOnly = true;
                gridViewMapping.Columns["IsRequired"].OptionsColumn.ReadOnly = true;
            }
        }


        private async void Yukle()
        {
            if (_columnMappings != null && _columnMappings.Count > 0)
            {
                try
                {
                    // İlerleme çubuğunu hazırla
                    progressBarControl1.Properties.Minimum = 0;
                    progressBarControl1.Properties.Maximum = 100;
                    progressBarControl1.Visible = true;

                    // İlerleme raporlama
                    var progress = new Progress<int>(percent => {
                        progressBarControl1.Position = percent;
                        progressBarControl1.Update();
                        Application.DoEvents();
                    });

                    // Katalog yükleme işlemini başlat
                    var result = await _bulkImportService.BulkImportCatalogAsync(
                        _sourceData, _brandId, _columnMappings, progress);

                    progressBarControl1.Visible = false;

                    if (result.Success)
                    {
                        XtraMessageBox.Show(
                            $"Katalog başarıyla yüklendi.\nToplam: {result.TotalCount} satır\n" +
                            $"Başarılı: {result.ImportedRows} satır\n" +
                            $"Boş Barkod: {result.FailedRows} satır\n" +
                            $"Süre: {result.ElapsedTime.TotalSeconds:N1} saniye",
                            "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        (this.Parent as Form).DialogResult = DialogResult.OK;


                       
                    }
                    else
                    {
                        XtraMessageBox.Show(
                            $"Katalog yüklenirken hata oluştu: {result.ErrorMessage}",
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    progressBarControl1.Visible = false;
                    XtraMessageBox.Show(
                        $"İşlem sırasında beklenmeyen bir hata oluştu: {ex.Message}",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Sütun eşleştirmeleri tanımlanmamış.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

     

        private void windowsUIButtonPanel_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var caption = e.Button.Properties.Caption;
            if (caption == "Iptal") (this.Parent as Form).DialogResult = DialogResult.Cancel;
            else if (caption == "Yükle") Yukle();
        }
    }
}
