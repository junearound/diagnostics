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

namespace Diagnostics.Services.Manager
{
   // [InstanceProviderBehavior]//when the InstanceContextMode is SingleCall the IInstanceProvider is not invoked to create the instance
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
       ConcurrencyMode = ConcurrencyMode.Multiple, 
        UseSynchronizationContext = false, 
        IncludeExceptionDetailInFaults = true)]
    public class DiagnosticsManager : IDiagnosticsManager, IMessageStorage
    {
        private readonly IDiagnosticsRepository _repository;
        private static readonly List<IDiagnosticsManagerCallback> subscribers = new List<IDiagnosticsManagerCallback>();
        private static readonly List<Subscriber> _subscribers = new List<Subscriber>();
        private static readonly object _sycnRoot = new object();

        
        public DiagnosticsManager(IDiagnosticsRepository repository)
        {
            _repository = repository; 
        }
        #region IDiagnosticsManager Members
        public async Task<bool> Subscribe(string filter)
        {
            bool success = false;
            var callback = this.CurrentCallback;
            lock (_sycnRoot)
            {
                try
                {
                    if (!_subscribers.Any(s => s.Callback == callback))
                    {
                        _subscribers.Add(new Subscriber() { Filter = filter, Callback = callback });
                        Console.WriteLine($"Добавлена подписка для канала : {callback.GetHashCode()}");
                        success = true;
                    }

                }
                catch (Exception ex)
                {
                    _subscribers.RemoveAll(x => x.Callback == callback);
                    Console.WriteLine($"Subscribe error: {ex.Message}");
                }
            }
            return success;
        }

        public async Task<bool> Unsubscribe() 
        {
            bool success = false;
            var callback = this.CurrentCallback;
            lock (_sycnRoot)
            {
                try
                {
                    if (_subscribers.Any(s => s.Callback == callback))
                    {
                        _subscribers.RemoveAll(x => x.Callback == callback);
                        Console.WriteLine("Удалена подписка для канала : {callback.GetHashCode()}");
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unsubscribe error: {ex.Message}");
                }
            }
            return success;
        }
        public async Task ChangeFilter(string newFilter)//<bool>
        {
            var callback = this.CurrentCallback;
            lock (_sycnRoot)
            {
                try
                {
                    foreach (var subscriber in _subscribers.Where(s => s.Callback == callback))
                    {
                        subscriber.Filter = newFilter;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ChangeFilter error: {ex.Message}");
                }
            }
        }

        
       

        
        public async Task UpdateMessage(Contracts.DiagnosticsMessage message)
        {
            if (message != null)
            {
                try
                {
                    //await Task.Run(() => 
                    //messageChanel.UpdateMessage(selectedMessage)
                    //);
                    await ChangeMessage(message);
                    var callback = this.CurrentCallback;
                    //Console.WriteLine($"Try to save: {message.ToString()}");
                    lock (_sycnRoot)
                    {
                        for (int i = _subscribers.Count - 1; i >= 0; i--)
                        {
                            var state = ((ICommunicationObject)_subscribers[i].Callback).State;//IChannel
                            if (((ICommunicationObject)_subscribers[i].Callback).State != CommunicationState.Opened)
                            {
                                Console.WriteLine($"Удален канал, находящийся в закрытом состоянии : {_subscribers[i].Callback.GetHashCode()}");
                                _subscribers.RemoveAt(i);
                                continue;
                            }

                            try
                            {
                                if (_subscribers[i].Callback != this.CurrentCallback)
                                {
                                    if (IsFilterSuitable(_subscribers[i].Filter, message))
                                    {
                                          _subscribers[i].Callback.OnUpdateMessage(message);//await
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Возникло исключение при изменении сообщения для канала: {_subscribers[i].Callback.GetHashCode()}");
                                Console.WriteLine($"Описание: {ex.Message}");
                                
                                _subscribers.RemoveAt(i);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UpdateMessage error: Тип:{ex.GetType()} Описание:{ex.Message}");
                }
            }
        }
        #endregion
        #region IMessageStorage Members
        public async Task SaveMessage(Contracts.DiagnosticsMessage message)
        {

            if (message != null)
            {
                
                await InsertMessage(message);
                
                lock (_sycnRoot)
                {
                    //if (_repository != null)
                    //{
                    //    _repository.Insert(message);
                    //}
                  
                    for (int i = _subscribers.Count - 1; i >= 0; i--)
                    {
                        if (((ICommunicationObject)_subscribers[i].Callback).State != CommunicationState.Opened)
                        {
                            var state = ((ICommunicationObject)_subscribers[i].Callback).State;
                            Console.WriteLine("Detected Non-Open Callback Channel: {0}", _subscribers[i].Callback.GetHashCode());
                            _subscribers.RemoveAt(i);
                            continue;
                        }
                        try
                        {
                            if (IsFilterSuitable(_subscribers[i].Filter, message))
                            {
                                _subscribers[i].Callback.OnNewMessage(message);//await
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Возникло исключение при сохранении сообщения для канала: {_subscribers[i].Callback.GetHashCode()}");
                            Console.WriteLine($"CreateMessage error: Тип:{ex.GetType()} Описание:{ex.Message}");
                            _subscribers.RemoveAt(i);
                        }
                    }
                }
            }
        }
        #endregion

        public async Task InsertMessage(Contracts.DiagnosticsMessage message)
        {
            Console.WriteLine($"Попытка сохранения сообщения: {message.ToString()}");
            if (message != null)
            {
                lock (_sycnRoot)
                {
                    try
                    {
                        if (_repository != null)
                        {
                            _repository.Insert(message);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"InsertMessage err: Тип:{ex.GetType()} Описание:{ex.Message}");
                    }
                }
            }
        }
        public async Task ChangeMessage(Contracts.DiagnosticsMessage message)
        {
            if (message != null)
            {
                lock (_sycnRoot)
                {
                    try
                    {
                        if (_repository != null)
                        {
                            _repository.Update(message);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ChangeMessage err: Тип:{ex.GetType()} Описание:{ex.Message}");
                    }
                }
            }
        }
        private static bool IsFilterSuitable(string filter, Contracts.DiagnosticsMessage message)
        {
            if (string.IsNullOrEmpty(filter))
                return false;
            if (filter == "All")//TODO
                return true;
            SeverityEnum severity;
            if (Enum.TryParse(filter, true, out severity))
                return message.Severity == severity;

            return false;
        }
        public IDiagnosticsManagerCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IDiagnosticsManagerCallback>();
            }
        }
    }
}
