using System;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;

namespace SOTA.DeviceEmulator.Views
{
    /// <summary>
    ///     Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();

            ( (INotifyCollectionChanged)LogsDataGrid.Items ).CollectionChanged += ScrollToTheEnd;
            AutoScrollCheckbox.Checked += ScrollToTheEnd;
        }

        private void ScrollToTheEnd(object sender, EventArgs e)
        {
            var isScrollCheckboxChecked = AutoScrollCheckbox.IsChecked ?? false;

            if (isScrollCheckboxChecked &&
                LogsDataGrid.Items.Count > 0 &&
                VisualTreeHelper.GetChild(LogsDataGrid, 0) is Decorator border &&
                border.Child is ScrollViewer scroll)
            {
                 scroll.ScrollToEnd();
            }
        }
    }
}