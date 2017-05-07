/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System;
using System.Windows;
using System.Windows.Input;
using DexMem.Engine;

namespace DexMem.MainWindow
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private readonly MainWindowTabbed _owner;
        private bool _isTabControlEnabled;
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

        public string DebugeeInfoStatusText
        {
            get => _debugeeInfoStatusText;
            set
            {
                if (value == _debugeeInfoStatusText) return;
                _debugeeInfoStatusText = value;
                OnPropertyChanged();
            }
        }

        public string AttachButtonContents => _debugee != null ? "Detach" : "Attach";

        // dont return a new routed command everytime
        private ICommand _attachCommand;

        private string _debugeeInfoStatusText;

        public ICommand AttachCommand => _attachCommand ??
                                         (_attachCommand = new RoutedCommand("Attach", typeof(MainWindowViewModel)));

        public MainWindowViewModel(MainWindowTabbed owner)
        {
            _owner = owner;
            owner.CommandBindings.Add(new CommandBinding(AttachCommand, OnAttachDetachExecuted));
            DebugeeInfoStatusText = "Detached";
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

            UpdateDebugeeText(_debugee);
        }

        private void UpdateDebugeeText(Debugee debugee)
        {
            DebugeeInfoStatusText = debugee != null ? $"Attached [{debugee.Process.Id}]" : "Detached";
        }
    }
}