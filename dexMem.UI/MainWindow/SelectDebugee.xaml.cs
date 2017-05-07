/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DexMem.Annotations;
using DexMem.Engine;

namespace DexMem
{
    /// <summary>
    /// Interaction logic for SelectDebugee.xaml
    /// </summary>
    public partial class SelectDebugee : INotifyPropertyChanged
    {
        private Debugee _selectedDebugee;

        public Debugee SelectedDebugee
        {
            get => _selectedDebugee;
            set
            {
                if (Equals(value, _selectedDebugee)) return;
                _selectedDebugee = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAttachEnabled));
            }
        }

        public ObservableCollection<Debugee> AvailableDebugees { get; set; }

        public bool IsAttachEnabled => _selectedDebugee != null;

        public SelectDebugee()
        {
            InitializeComponent();
            DataContext = this;
            AvailableDebugees = new ObservableCollection<Debugee>();
            Dispatcher.InvokeAsync(() =>OnRefreshClick(null, null));
        }

        private void OnAttachClick(object sender, RoutedEventArgs e)
        {
            // let caller handle this SelectedDebugee.Open();
            DialogResult = true;
            Close();
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            AvailableDebugees.Clear();

            // dont reassign, that will mess up bindings
            foreach (var debugee in Debugee.Available)
            {
                try
                {
                    // only show running userprocesses (images)
                    // by trying to access this it will throw an exception on things like idle and system, filtering em out
                    if (debugee.Process.MainModule.ModuleName.Length < 1)
                        continue;
                }
                catch (Win32Exception)
                {
                    continue;
                }
                catch (InvalidOperationException)
                {
                    // can happen when we iterate as a process exits
                    continue;
                }

                AvailableDebugees.Add(debugee);
            }
        }
    }
}
