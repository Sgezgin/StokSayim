using DevExpress.XtraBars;
using StokSayim.Data.Repositories;
using StokSayim.Data.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StokSayim
{
    public partial class MainMenu : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        private readonly BrandRepository _brandRepository;
        private readonly StoreRepository _storeRepository;
        private readonly CatalogService _catalogService;
        private readonly BulkImportService _bulkImportService;

        private UserControl _activeControl;


        public MainMenu(
            BrandRepository brandRepository,
            StoreRepository storeRepository,
            CatalogService catalogService,
            BulkImportService bulkImportService)
        {
            _brandRepository = brandRepository;
            _storeRepository = storeRepository;
            _catalogService = catalogService;
            _bulkImportService = bulkImportService;

            InitializeComponent();
        }

        private void aceKullanicilar_Click(object sender, EventArgs e)
        {

        }

        private void aceFirmalar_Click(object sender, EventArgs e)
        {

        }

        private void aceMarka_Click(object sender, EventArgs e)
        {
            if (_activeControl != null)
            {
                flContainer.Controls.Remove(_activeControl);
                _activeControl.Dispose();
            }

            flContainer.Controls.Clear();
            flContainer.Controls.Add(new Moduller.Marka.MarkaMain(_brandRepository) { Dock = DockStyle.Fill });
        }
    }
}
