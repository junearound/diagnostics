using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ServiceModel;
using Diagnostics.Contracts;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;


namespace Diagnostics.TerminalClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Diagnostics.Contracts.IDiagnosticsManager _channel;
        private DiagnosticsCallbackHandler _clientCallback;
        private ObservableCollection<DiagnosticsMessage> _messages = new ObservableCollection<DiagnosticsMessage>();
        ChannelFactory<IDiagnosticsManager> _channelFactory;

        public MainWindow()
        {
            InitializeComponent();
            SetStartState();
            InitializeChannel();
        }
  
        private void InitializeChannel()
        {
            try
            {
                CloseChannel();
                if (_channelFactory == null)
                {
                    _clientCallback = new DiagnosticsCallbackHandler();
                    var context = new InstanceContext(_clientCallback); 
                    _channelFactory = new DuplexChannelFactory<Diagnostics.Contracts.IDiagnosticsManager>(context, "WSDualHttpBinding_IDiagnosticsManager");
                    _clientCallback.MessageAdded += (s, message) =>
                    {
                        Dispatcher.Invoke(() => UpdateMessagesList(message));
                    };
                    _clientCallback.MessageUpdated += (s, message) =>
                    {
                        Dispatcher.Invoke(() => UpdateMessagesList(message));
                    };
                }
                _channel = _channelFactory.CreateChannel();
                ((ICommunicationObject)_channel).Faulted += new EventHandler(Channel_Faulted);
                ((ICommunicationObject)_channel).Opened += new EventHandler(Channel_Opened);
                ((ICommunicationObject)_channel).Closed += new EventHandler(Channel_Closed);
            }
            catch (Exception ex)
            {
                CloseChannel();
                txtInfo.Text = "Не удалось подключиться к серверу";
                MessageBox.Show(ex.Message);
            }
        }
        

        public void StartLongOperation()
        {//block buttons
         // Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => this.progressBar.Value = 50));
            btnSubscribe.IsEnabled = false;
            btnUnsubscribe.IsEnabled = false;
            severityFilterList.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;//block buttons
                                               //make datatable grey
                                               //not clickable
        }
        public void StopLongOperation()
        {
            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Visibility.Hidden;
            severityFilterList.IsEnabled = true;
            btnSubscribe.IsEnabled = true;
            btnUnsubscribe.IsEnabled = true;
           
        }
        private void SetSubscribedState(bool resetAll)
        {
            btnSubscribe.IsEnabled = false;
            btnUnsubscribe.IsEnabled = true;
            severityFilterList.IsEnabled = true;
            if (!resetAll) return;
       
            txtInfo.Text = "Прием сообщений начат";
            
            if (resultDataGrid.RowStyle == null)
            {
                EventSetter RowDoubleClickSetter = new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                     new MouseButtonEventHandler(Row_DoubleClick));
                Style rowStyle = new Style(typeof(DataGridRow));
                rowStyle.Setters.Add(RowDoubleClickSetter);
                resultDataGrid.RowStyle = rowStyle;
                resultDataGrid.RowStyle.Setters.Add(RowDoubleClickSetter);
            }
        }
        private void SetUnsubscribedState()
        {
            btnSubscribe.IsEnabled = true;
            btnUnsubscribe.IsEnabled = false;
            txtInfo.Text = "Прием сообщений остановлен";
        }
        private void SetStartState()
        {
            txtInfo.Text = "Данные еще не получены";
            List<string> filtersList = Enum.GetValues(typeof(SeverityEnum)).Cast<SeverityEnum>().Select(v => v.ToString()).ToList();
            filtersList.Add("All");
            severityFilterList.ItemsSource = filtersList;
            severityFilterList.SelectedValue = "All";
            resultDataGrid.IsReadOnly = true;
        }
 
        void  Channel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() =>
                {
                    txtInfo.Text = "Closed";
                    HandleChannel();
                });
                return;
            }
            HandleChannel();
        }
        void  Channel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() =>
                {
                    txtInfo.Text = "Opened";
                    HandleChannel();
                });
                return;
            }
            HandleChannel();
        }
        void  Channel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() =>
                {
                    txtInfo.Text = "Faulted";
                    HandleChannel();
                });
                return;
            }
            HandleChannel();
           
        }
 
        private void HandleChannel()
        {
            if (_channel != null)
            {
                switch (((ICommunicationObject)_channel).State)
                {
                    case CommunicationState.Closed:
                        txtInfo.Text = "Соединение закрыто";
                        SetUnsubscribedState();
                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        CloseChannel();
                        txtInfo.Text = "Ошибка соединения";
                        SetUnsubscribedState();
                        break;
                    case CommunicationState.Opened:
                        txtInfo.Text = "Opened";
                        break;
                    case CommunicationState.Opening:
                        break;
                    default:
                        break;
                }
            }

        }
        private async void btnUnsubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartLongOperation();
                if(Subscribed)
                {
                    bool success = await _channel.Unsubscribe();
                    CloseChannel();
                    Dispatcher.Invoke(() =>
                    {
                        StopLongOperation();
                        if (success)
                            SetUnsubscribedState();
                    });
                }
            }
            catch (Exception ex)
            {
                CloseChannel();
                MessageBox.Show($"{ex.Message}:              {ex.StackTrace}");
            }
        }
        private async void btnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 if (_channel == null)
                    InitializeChannel();
                string filter = (string)severityFilterList.SelectedItem;
                if(_channel!=null)
                {
                    StartLongOperation();
                   
                   bool success = await _channel.Subscribe(filter);
                    Dispatcher.Invoke(() => {
                        StopLongOperation();
                        if (success)
                            SetSubscribedState(true);
                    });
                    
                }
            }
            catch (Exception ex)
            {
                CloseChannel();
                MessageBox.Show($"{ex.Message}:                  {ex.StackTrace}");
            }
          
   

        }

       
        public void UpdateMessagesList(DiagnosticsMessage message)
        {
            if (message == null)
                return;
     
            if (_messages.Any(m => m.Uid == message.Uid))
            {
                var old = _messages.FirstOrDefault(m => m.Uid == message.Uid);
               int index =  _messages.IndexOf(old);
                if (index != -1)
                {
                    _messages[index] = message;
                }
            }
            else
                _messages.Add(message);

            resultDataGrid.Items.Refresh();
        }
        private async void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!Subscribed || resultDataGrid.SelectedItem == null)
                return;
            DiagnosticsMessage selectedMessage = resultDataGrid.SelectedItem as DiagnosticsMessage;
            try
            {
                 if(_channel != null && !selectedMessage.IsRead)
                {
                    StartLongOperation();
                    selectedMessage.IsRead = true;
                    await _channel.UpdateMessage(selectedMessage);//.ConfigureAwait(true);  

                    Dispatcher.Invoke(() => { txtInfo.Text = "Сообщение обновлено";
                        StopLongOperation();
                        SetSubscribedState(false); });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}:              {ex.StackTrace}");
            }
        }
        private async void SeverityFilter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (!Subscribed || comboBox.SelectedItem == null || _channel == null)
                return;
            string filter = (string)comboBox.SelectedItem;
            DiagnosticsMessage selectedMessage = resultDataGrid.SelectedItem as DiagnosticsMessage;
            try
            {
                if (_channel != null)
                {
                    StartLongOperation();
                    await _channel.ChangeFilter(filter);
                    Dispatcher.Invoke(() =>
                    {
                        txtInfo.Text = "Фильтр обновлен";
                        StopLongOperation();
                        SetSubscribedState(false);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}:              {ex.StackTrace}");
            }
        }
        
        private async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
       
            if (_channel!=null&&((ICommunicationObject)_channel).State == CommunicationState.Opened)
            {
                await _channel.Unsubscribe();
            }
            CloseChannel();
            CloseChannelFactory();
        }

        private void CloseChannel()
        {
            try
            {
                if (_channel != null)
                {
                    ICommunicationObject commObj = null;
                    if (_channel is ICommunicationObject)
                    {
                        commObj = (ICommunicationObject)_channel;
                        DisposeCommunicationObject(commObj);
                    }
                    _channel = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CloseChannelFactory()
        {
            try
            {
                if (_channelFactory != null)
                {
                    DisposeCommunicationObject(_channelFactory);
                    _channelFactory = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static void DisposeCommunicationObject(ICommunicationObject communicationObject)
        {
            if (communicationObject != null)
            {
                if (communicationObject.State == CommunicationState.Faulted)
                    communicationObject.Abort();
                else
                    try
                    {
                        communicationObject.Close();
                    }
                    catch
                    {
                        communicationObject.Abort();
                        throw;
                    }
                    //finally
                    //{
                    //    ((IDisposable)communicationObject).Dispose();
                    //}
            }
        }
        public ObservableCollection<DiagnosticsMessage> DiagnosticsMessages
        {
            get
            {
                return this._messages;
            }
        }
        public bool Subscribed
        {
            get
            {
                if (_channel != null && ((ICommunicationObject)_channel).State == CommunicationState.Opened)
                {
                    return true;
                }
                return false;
            }
        }
    }
}

 
