/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System.Windows;

namespace DexMem.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindowTabbed.xaml
    /// </summary>
    public partial class MainWindowTabbed : Window
    {
        // bit of a hack but we should only have one main window
        public static MainWindowViewModel ViewModel { get; set; }
        public MainWindowTabbed()
        {
            InitializeComponent();
            DataContext = ViewModel = new MainWindowViewModel(this);
        }
    }
}
