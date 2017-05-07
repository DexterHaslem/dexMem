using System.Windows;

namespace DexMem.DexStatusBar
{
    public partial class DexStatusBar
    {
        public DexStatusBarViewModel ViewModel
        {
            get => (DexStatusBarViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(DexStatusBarViewModel), typeof(DexStatusBar), new PropertyMetadata(null));

        public DexStatusBar()
        {
            InitializeComponent();
            DataContext = ViewModel = new DexStatusBarViewModel();
        }
    }
}