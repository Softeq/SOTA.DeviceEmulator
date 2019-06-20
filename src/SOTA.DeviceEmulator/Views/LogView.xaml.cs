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
        }

        private void ScrollToTheEnd(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (LogsDataGrid.Items.Count > 0)
            {
                if (VisualTreeHelper.GetChild(LogsDataGrid, 0) is Decorator border)
                {
                    if (border.Child is ScrollViewer scroll)
                    {
                        scroll.ScrollToEnd();
                    }
                }
            }
        }
    }
}