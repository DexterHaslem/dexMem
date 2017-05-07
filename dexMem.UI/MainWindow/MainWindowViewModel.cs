using System;
using System.Windows;
using System.Windows.Input;
using DexMem.Engine;

namespace DexMem.MainWindow
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private MainWindowTabbed _owner;
        private bool _isTabControlEnabled;
        private string _attachButtonContents;
        private Debugee _debugee;

        public bool IsTabControlEnabled
        {
            get => _isTabControlEnabled;
            set
            {
                if (value == _isTabControlEnabled) return;
                _isTabControlEnabled = value;
                OnPropertyChanged();
            }
        }

        public DexStatusBar.DexStatusBarViewModel StatusBarViewModel { get; set; }

        public string AttachButtonContents => _debugee != null ? "Detach" : "Attach";

        // dont return a new routed command everytime
        private ICommand _attachCommand;
        public ICommand AttachCommand => _attachCommand ??
                                         (_attachCommand = new RoutedCommand("Attach", typeof(MainWindowViewModel)));

        public MainWindowViewModel(MainWindowTabbed owner)
        {
            _owner = owner;
            owner.CommandBindings.Add(new CommandBinding(AttachCommand, OnAttachDetachExecuted));
        }

        private void OnAttachDetachExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_debugee != null)
            {
                _debugee.Close();
                _debugee = null;
            }
            else
            {
                var selectDebugee = new SelectDebugee {Owner = _owner};
                if (selectDebugee.ShowDialog() == true)
                {
                    _debugee = selectDebugee.SelectedDebugee;
                    try
                    {
                        _debugee.Open();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message);
                        _debugee = null;
                    }
                }
            }

            OnPropertyChanged(nameof(AttachButtonContents));
            IsTabControlEnabled = _debugee != null;
        }
    }
}