/*
 * dexMem 
 * Dexter Haslem <dmh@fastmail.com> 2017
 * see the LICENSE file for licensing details
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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
        private string _searchText;
        private ICollectionView _collectionView;

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

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                UpdateSearchFilter();
            }
        }

        private void UpdateSearchFilter()
        {
            // refresh doesnt work, reassign filter. amazing
            //_collectionView.Refresh();
            _collectionView.Filter = null;
            _collectionView.Filter = SearchFilter;
        }

        public ObservableCollection<Debugee> AvailableDebugees { get; set; }

        public bool IsAttachEnabled => _selectedDebugee != null;

        public SelectDebugee()
        {
            InitializeComponent();
            DataContext = this;
            AvailableDebugees = new ObservableCollection<Debugee>();
            Dispatcher.InvokeAsync(() =>OnRefreshClick(null, null));
            _collectionView = CollectionViewSource.GetDefaultView(listView.Items);
            //_collectionView.Filter = SearchFilter;
        }

        private bool SearchFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var debugee = obj as Debugee;
            if (debugee == null)
                return true;

            var search = SearchText.ToLowerInvariant().Trim();
            return debugee.Name.ToLowerInvariant().Contains(search);
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
            //foreach (var debugee in Debugee.Available)
            // this will be slow due to all the exceptions being thrown so multithreaded
            var debugees = new List<Debugee>();
            Parallel.ForEach(Debugee.Available, debugee =>
            {
                try
                {
                    // only show running userprocesses (images)
                    // by trying to access this it will throw an exception on things like idle and system, filtering em out
                    if (debugee.Process.MainModule.ModuleName.Length < 1)
                        return;
                }
                catch (Win32Exception)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    // can happen when we iterate as a process exits
                    return;
                }

                lock (debugees)
                    debugees.Add(debugee);
            });

            // have to do this on main thread
            debugees.ForEach(AvailableDebugees.Add);
        }
    }
}
