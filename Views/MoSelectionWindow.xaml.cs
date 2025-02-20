using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace dapdon.Views
{
    public partial class MoSelectionWindow : Window
    {
        public List<string> MoNoList { get; set; }

        private string _selectedMoNo;
        public string SelectedMoNo
        {
            get { return _selectedMoNo; }
            set { _selectedMoNo = value; }
        }

        public MoSelectionWindow(List<string> moNos)
        {
            InitializeComponent();
            MoNoList = moNos.Distinct().ToList();
            DataContext = this;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Không cần FindName, SelectedMoNo đã có binding
            DialogResult = true;
            Close();
        }
    }
}
