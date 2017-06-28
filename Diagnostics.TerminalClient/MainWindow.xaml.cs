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
        private DiagnosticsCallbackHandler clientCallback;
        private ObservableCollection<DiagnosticsMessage> _messages= new ObservableCollection<DiagnosticsMessage>();
        private bool subscribed = false;
        private const string ServiceEndpointUri = "http://localhost:17121/service";
        private DiagnosticsManagerClient _proxy;

        public MainWindow()
        {
            InitializeComponent();
            InitClient();
            
        }

        private void InitializeClient()
        {
            //if (_proxy != null)
            //{
            //    try
            //    {
            //        _proxy.Close();
            //    }
            //    catch
            //    {
            //        _proxy.Abort();
            //    }
            //    finally
            //    {
            //        _proxy = null;
            //    }
            //}
            //clientCallback = new DiagnosticsCallbackHandler();
           
            //clientCallback.MessageAdded += (s, message) =>
            //{
            //    Dispatcher.Invoke(() => UpdateMessagesList(message));
            //};
            //clientCallback.MessageUpdated += (s, message) =>
            //{
            //    Dispatcher.Invoke(() => UpdateMessagesList(message));
            //};

            //var instanceContext = new InstanceContext(clientCallback);
            //var dualHttpBinding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
            //var endpointAddress = new EndpointAddress(ServiceEndpointUri);
            //_proxy = new DiagnosticsManagerClient(instanceContext, dualHttpBinding, endpointAddress);



        //    _proxy.Open();
            //_proxy.Connect();
            //if (client.InnerChannel.State != System.ServiceModel.CommunicationState.Faulted)
            //{
            //    // call service - everything's fine
            //}
            //else
            //{
            //    // channel faulted - re-create your client and then try again
            //}
        }
        private void InitializeChannel()
        {
            try
            {
                var binding = new WSDualHttpBinding();
                binding.OpenTimeout = new TimeSpan(0, 10, 0);
                binding.CloseTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);//clientBaseAddress="http://localhost:17122/"
                var address = new EndpointAddress("http://localhost:17121/service");//WSDualHttpBinding_IDiagnosticsManager
                clientCallback = new DiagnosticsCallbackHandler();
                var context = new InstanceContext(clientCallback);//todo error
               // var factory = new DuplexChannelFactory<Diagnostics.Contracts.IDiagnosticsManager>(context, "WSDualHttpBinding_IDiagnosticsManager");
                      var factory = new DuplexChannelFactory<Diagnostics.Contracts.IDiagnosticsManager>(context, binding, address);
                //var channelFactory = new ChannelFactory<IDiagnosticsManager>(
                //                    "WSDualHttpBinding_IDiagnosticsManager"  
                //                    );
                _channel = factory.CreateChannel();
                clientCallback.MessageAdded += (s, message) =>
                {
                    Dispatcher.Invoke(() => UpdateMessagesList(message));
                };
                clientCallback.MessageUpdated += (s, message) =>
                {
                    Dispatcher.Invoke(() => UpdateMessagesList(message));
                };
            }
            catch (Exception ex)
            {
                //if (((IChannel)callback).State == CommunicationState.Opened)
                MessageBox.Show(ex.Message);
                // messageChanel = null;//todo dispose
            }
        }

        private void InitClient()
        {
            try
            {
                
                //InitializeClient();
                InitializeChannel();
                txtInfo.Text = "Данные еще не получены....";
                List<string> filtersList = Enum.GetValues(typeof(SeverityEnum)).Cast<SeverityEnum>().Select(v => v.ToString()).ToList();
                filtersList.Add("All");
                severityFilterList.ItemsSource = filtersList;
                severityFilterList.SelectedValue = "All";
                resultDataGrid.IsReadOnly = true;
            }
            catch (Exception ex) {
                //if (((IChannel)callback).State == CommunicationState.Opened)
                txtInfo.Text = "Не удалось подключиться к серверу";
                MessageBox.Show(ex.Message);
               // messageChanel = null;//todo dispose
            }
            // dispose if err
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
            //          grid.SelectionChanged += (obj, e) =>
            //Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            //  grid.UnselectAll()));
        }
        private void SetSubscribedState(bool resetAll)
        {
            btnSubscribe.IsEnabled = false;
            btnUnsubscribe.IsEnabled = true;
            severityFilterList.IsEnabled = true;
            if (!resetAll) return;
            subscribed = true;
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
            //else if (!resultDataGrid.RowStyle.Setters.Contains(RowDoubleClickSetter))
            //    resultDataGrid.RowStyle.Setters.Add(RowDoubleClickSetter);

        }
        private void SetUnsubscribedState()
        {
            subscribed = false;
            btnSubscribe.IsEnabled = true;
            btnUnsubscribe.IsEnabled = false;
            txtInfo.Text = "Прием сообщений остановлен";
        }
        private delegate void FaultedInvoker();
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));//TODO async
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Opened(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvoker(HandleProxy));
                return;
            }
            HandleProxy();
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
        void Channel_Faulted(object sender, EventArgs e)
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
        private void HandleProxy()
        {
            if (_proxy != null)
            {
                switch (this._proxy.State)
                {
                    case CommunicationState.Closed:
                        _proxy = null;
                        txtInfo.Text = "Closed";
                        //chatListBoxMsgs.Items.Clear();
                        //chatListBoxNames.Items.Clear();
                        //loginLabelStatus.Content = "Disconnected";
                        //ShowChat(false);
                        //ShowLogin(true);
                        //loginButtonConnect.IsEnabled = true;
                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        _proxy.Abort();
                        txtInfo.Text = "Faulted";
                        _proxy = null;
                        //chatListBoxMsgs.Items.Clear();
                        //chatListBoxNames.Items.Clear();
                        //ShowChat(false);
                        //ShowLogin(true);
                        //loginLabelStatus.Content = "Disconnected";
                        //loginButtonConnect.IsEnabled = true;
                        break;
                    case CommunicationState.Opened:
                        txtInfo.Text = "Opened";
                        //ShowLogin(false);
                        //ShowChat(true);

                        //chatLabelCurrentStatus.Content = "online";
                        //chatLabelCurrentUName.Content = this.localClient.Name;

                        //Dictionary<int, Image> images = GetImages();
                        //Image img = images[loginComboBoxImgs.SelectedIndex];
                        //chatCurrentImage.Source = img.Source;
                        break;
                    case CommunicationState.Opening:
                        break;
                    default:
                        break;
                }
            }

        }
        private void HandleChannel()
        {
            if (_channel != null)
            {
                switch (((ICommunicationObject)_channel).State)
                {
                    case CommunicationState.Closed:
                        _channel = null;
                        txtInfo.Text = "Closed";
                        SetUnsubscribedState();
                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        //_channel.Abort();
                        txtInfo.Text = "Faulted";
                        _channel = null;
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
                if(_channel!=null)//if (_proxy != null)
                {

                    bool success = await _channel.Unsubscribe();
                      //  Task.Run(() =>
                      //_channel.Unsubscribe()
                      //);
                      // bool success = await   _proxy.Unsubscribe();
                      // _proxy = null;
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
                MessageBox.Show($"{ex.Message}:              {ex.StackTrace}");
            }
        }
        private async void btnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_channel == null)// if (_proxy == null)
                    InitClient();
                string filter = (string)severityFilterList.SelectedItem;
                //if (_proxy != null)
                if(_channel!=null)
                {
                    StartLongOperation();
                    // _proxy.Open();
                    ((ICommunicationObject)_channel).Faulted += new EventHandler(Channel_Faulted);
                    ((ICommunicationObject)_channel).Opened += new EventHandler(Channel_Opened);
                    ((ICommunicationObject)_channel).Closed += new EventHandler(Channel_Closed);
                    bool success = await _channel.Subscribe(filter);
                    //((ICommunicationObject)_channel).ConnectionRecovered += new EventHandler(Channel_Closed);
                    //callbackSyncProxy.Ping();
                    // _proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
                    // _proxy.InnerDuplexChannel.Opened += new EventHandler(InnerDuplexChannel_Opened);
                    // _proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                    // bool success = await _proxy.Subscribe(filter);
                    Dispatcher.Invoke(() => {
                        StopLongOperation();
                        if (success)
                            SetSubscribedState(true);
                    });
                    
                }
            }
            catch (TimeoutException ex) {
                MessageBox.Show(ex.Message);//eeee
            }
            catch (Exception ex)
            {
               // subscribed = false;
               //TODO dispose
                MessageBox.Show($"{ex.Message}:                  {ex.StackTrace}");
            }
          
   

        }

       
        public void UpdateMessagesList(DiagnosticsMessage message)
        {
            if (message == null) return;
            txtInfo.Text = message.Text;
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
            if (!subscribed || resultDataGrid.SelectedItem == null)
                return;
            DiagnosticsMessage selectedMessage = resultDataGrid.SelectedItem as DiagnosticsMessage;
            try
            {
                if(_channel != null && !selectedMessage.IsRead)//if (_proxy != null&&!selectedMessage.IsRead)
                {
                    StartLongOperation();
                    selectedMessage.IsRead = true;
                    //await _proxy.UpdateMessage(selectedMessage);
                    await _channel.UpdateMessage(selectedMessage);//.ConfigureAwait(true);  
                    //await Task.Run(() => 
                    //messageChanel.UpdateMessage(selectedMessage)
                    //);
                    Dispatcher.Invoke(() => { txtInfo.Text = "Сообщение обновлено";
                        StopLongOperation();
                        SetSubscribedState(false); });
                }
            }
            catch (TimeoutException timeProblem)
            {
                MessageBox.Show($"timeout");
                //Console.WriteLine("The service operation timed out. " + timeProblem.Message);
                // Console.ReadLine();
            }
            catch (CommunicationException commProblem)
            {
                MessageBox.Show($"comm");
                //  Console.WriteLine("There was a communication problem. " + commProblem.Message);
                // Console.ReadLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}:              {ex.StackTrace}");
            }
        }
        private async  void SeverityFilter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (!subscribed || comboBox.SelectedItem == null|| _channel==null)
                return;
                string filter = (string)comboBox.SelectedItem;
                DiagnosticsMessage selectedMessage = resultDataGrid.SelectedItem as DiagnosticsMessage;
                try
                {
                    if( _channel!=null)//if (_proxy != null)
                    {
                        StartLongOperation();
                       // await  _proxy.ChangeFilter(filter);
                    await _channel.ChangeFilter(filter);
                    Dispatcher.Invoke(() => {
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
            // protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
            // //private void Window_Unloaded(object sender, RoutedEventArgs e)
  //          if (subscribed&& _proxy != null)
 //           {
  //              try
  //              {
                    //if (_proxy.State == CommunicationState.Opened)
  //                  await _proxy.Unsubscribe();
  //                  _proxy.Close();
   //             }
   //             catch
   //             {
   //                 _proxy.Abort();
   //             }
               // bool success = await  _proxy.Unsubscribe();
    //        }
            //   if (host != null)
            //  messageChanel
            //  MessageBox.Show("Closing called");

            //if (this.Dispatcher.Is.isDataDirty)
            //{
            //    string msg = "Data is dirty. Close without saving?";
            //    MessageBoxResult result =
            //      MessageBox.Show(
            //        msg,
            //        "Data App",
            //        MessageBoxButton.YesNo,
            //        MessageBoxImage.Warning);
            //    if (result == MessageBoxResult.No)
            //    {
            //        // If user doesn't want to close, cancel closure
            //        e.Cancel = true;
            //    }
            //}
        }


        public ObservableCollection<DiagnosticsMessage> DiagnosticsMessages
        {
            get
            {
                return this._messages;
            }
        }

    }
}



