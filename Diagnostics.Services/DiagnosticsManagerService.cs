using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Diagnostics.DataAccess;

namespace Diagnostics.Services
{
    //[ServiceBehavior(
    //    InstanceContextMode = InstanceContextMode.Single,
    //    ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DiagnosticsManagerService : IDiagnosticsManager, IMessageStorage
    {
        private readonly DiagnosticsRepository _repository;

        // Dictionary<Guid, Subscriber> clients = new Dictionary<Guid, Subscriber>();
        private static readonly List<IDiagnosticsManagerCallback> subscribers = new List<IDiagnosticsManagerCallback>();
        private static readonly List<Subscriber> _subscribers = new List<Subscriber>();
        private static readonly object _sycnRoot = new object();

        //Contracts.DiagnosticsMessage[] Messages = null;//ConcurrentDictionary
        public DiagnosticsManagerService()
        {
            
            _repository = new DiagnosticsRepository(new DiagnosticsDBContext());
            _repository.InsertOrUpdate(new Contracts.DiagnosticsMessage()
            { SourceId = new Guid(), Severity = SeverityEnum.Statistics, Text = "dfgdfgdfgdfgdfg" });
            _repository.InsertOrUpdate(new Contracts.DiagnosticsMessage()
            { SourceId = new Guid(), Severity = SeverityEnum.Statistics, Text = "777777" });
        }

        public async Task<bool> Subscribe(string filter)
        {
            Console.WriteLine($"Filter: {filter}");
            bool success = false;
            try
            {
                // var ctxId = Guid.NewGuid(); 
               
                var callback = OperationContext.Current.GetCallbackChannel<IDiagnosticsManagerCallback>();
                lock (_sycnRoot)
                {
                    if (!_subscribers.Any(s => s.Callback == callback))
                    {
                        _subscribers.Add(new Subscriber() { Filter = filter, Callback = callback });
                        Console.WriteLine("Subscription added");
                        success = true;


                    }
                     

                }
                //Contracts.DiagnosticsMessage m = new Contracts.DiagnosticsMessage() { SourceId = new Guid(), Severity = SeverityEnum.Statistics, Text = "dfgdfgdfgdfgdfg" };
                //await callback.OnNewMessage(m);
                //while (((IChannel)callback).State == CommunicationState.Opened)
                //{
                //    Contracts.DiagnosticsMessage m = new Contracts.DiagnosticsMessage() { SourceId = new Guid(), Severity = SeverityEnum.Statistics, Text = "dfgdfgdfgdfgdfg" };
                //    await callback.OnNewMessage(m);

                //    await Task.Delay(1000);
                //}

            }
            catch (Exception ex)
            {
                //      catch
                //{
                //    clients.Remove(key);
                Console.WriteLine($"Subscribe err: {ex.Message}");
            }
            return success;
        }

        //private async void TaskCallback(object callback)
        //{
        //    IDiagnosticsManagerCallback messageCallback = callback as IDiagnosticsManagerCallback;

        //    for (int i = 0; i < 10; i++)
        //    {
        //        messageCallback.OnNewMessage("message " + i.ToString());
        //        await Task.Delay(1000);
        //    }
        //}

        public async Task<bool> Unsubscribe()//<bool>
        {
            bool success = false;
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IDiagnosticsManagerCallback>();
                lock (_sycnRoot)
                {
                    _subscribers.RemoveAll(x => x.Callback == callback);//callback.GetHashCode()
                    Console.WriteLine("Subscription removed");
                    success = true;
                    //var setToRemove = new HashSet<Subscriber>(_subscribers);
                    //_subscribers.RemoveAll(x => setToRemove.Contains(x));
                    //for (int i = _callbackChannels.Count - 1; i >= 0; i--)
                    //if (_subscribers.Contains<Subscriber>(s=> ))

                    //if (subscribers.Contains((callback))
                    //    subscribers.Remove(callback);
                }
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unsubscribe err: {ex.Message}");
            }
            return success;
        }


        public async Task ChangeFilter(string newFilter)//<bool>
        {
            Console.WriteLine($"New Filter: {newFilter}");
            try
            {
                lock (_sycnRoot)
                {
                    var callback = OperationContext.Current.GetCallbackChannel<IDiagnosticsManagerCallback>();
                    foreach (var item in _subscribers.Where(s => s.Callback == callback))
                    {
                        item.Filter = newFilter;
                    }

                }
                //Console.WriteLine("Removed Callback Channel: {0}", callback.GetHashCode());
                await Task.Delay(1000);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChangeFilter err: {ex.Message}");
            }
        }

        //public async Task AddMessage(Contracts.DiagnosticsMessage message)
        //{
        //    //return await Task.FromResult(() => { });
        //    await Task.Delay(100);
        //}
        public async Task UpdateMessage(Contracts.DiagnosticsMessage message)
        {
            //return await Task.FromResult(() => { });
            await Task.Delay(100);
        }

     
        public async Task SaveMessage(Contracts.DiagnosticsMessage message)
        {
           
            if (message != null)
            {
                Console.WriteLine($"Try to save: {message.ToString()}");
                lock (_sycnRoot)
                {
                    if (_repository != null)
                    {
                        _repository.InsertOrUpdate(message);
                    }
                    //return await Task.FromResult(() => { });
                    for (int i = _subscribers.Count - 1; i >= 0; i--)
                    {
                        if (((ICommunicationObject)_subscribers[i].Callback).State != CommunicationState.Opened)
                        {
                            Console.WriteLine("Detected Non-Open Callback Channel: {0}", _subscribers[i].Callback.GetHashCode());
                            _subscribers.RemoveAt(i);
                            continue;
                        }

                        try
                        {
                           // _subscribers[i].Callback.SendUpdatedList(_groceryList);
                            _subscribers[i].Callback.OnNewMessage(message);
                            Console.WriteLine("Pushed Updated List on Callback Channel: {0}", _subscribers[i].Callback.GetHashCode());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Service threw exception while communicating on Callback Channel: {0}", _subscribers[i].Callback.GetHashCode());
                            Console.WriteLine("Exception Type: {0} Description: {1}", ex.GetType(), ex.Message);
                            _subscribers.RemoveAt(i);
                        }
                    }
                }
            }
        }

    }
}
