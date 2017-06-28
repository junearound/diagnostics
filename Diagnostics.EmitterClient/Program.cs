using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Diagnostics.EmitterClient
{
 
    class Program
    {

        static void Main()
        {
            try
            {
                int timeoutSeconds = 2;
                Console.Write("Отправка сообщений начата");
                Console.Write("\nНажмите клавишу Esc для остановки отправки\n\n");
                CreateSession(timeoutSeconds);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("\nОШИБКА:");
                Console.WriteLine(e);
            }
            Console.ReadKey();

        }

        private static void CreateSession(int timeoutSeconds)
        {
            var tokenSource = new CancellationTokenSource();
            var mre = new ManualResetEventSlim();
            var task = Task.Run(async () => await StartSendingSessionAsync(timeoutSeconds, tokenSource.Token), tokenSource.Token);

            task.ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => Console.WriteLine("\nЗадача была отменена"), TaskContinuationOptions.OnlyOnCanceled);
            task.ContinueWith(t => mre.Set());

            Task.Run(() =>
            {
                while (!tokenSource.Token.IsCancellationRequested)
                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
                        tokenSource.Cancel();
            });

            mre.Wait();

            tokenSource.Cancel();
            Console.Write("\nНажмите любую клавишу для выхода\n");
        }




        private async static Task StartSendingSessionAsync(int timeoutSeconds, CancellationToken token)
        {
            var newToken = new CancellationTokenSource();
            token.Register(newToken.Cancel);
            Guid sessionId = Guid.NewGuid();
            var client = new EmitterClient("BasicHttpBinding_IDiagnosticsDispatcher");
            while (!token.IsCancellationRequested) 
            {
                try
                {
                    await SendMessageAsync(client, sessionId, newToken.Token);                                                                 
                    token.ThrowIfCancellationRequested();                                              
                    await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), token); 
                }
                catch (OperationCanceledException)
                {
                    newToken.Cancel();
                    Console.WriteLine("Отправка отменена");
                    break;
                }
            }
            if (client != null && client.State != CommunicationState.Closed)
                client.Close();
        }
        private static async Task SendMessageAsync(EmitterClient client, Guid sessionId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                string text = RandomDataHelper.RandomString(9);
                DiagnosticsMessage message = new DiagnosticsMessage() {
                    SourceId = sessionId,
                    Severity = RandomDataHelper.RandomSeverity(),
                    Text = text,
                    Uid = Guid.NewGuid() };
                string msgDescription = message.ToString();
                Console.WriteLine($"Отправка сообщения: {msgDescription};");
                await client.PushMessageAsync(message); 
                
                Console.WriteLine("Сообщение отправлено");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine("Невозможно подключиться к серверу: " + ex.Message + "/n ");
                client.Abort();
                throw new OperationCanceledException(token);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("Время ожидания истекло: " + ex.Message + "/n ");
                client.Abort();
                throw new OperationCanceledException(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке сообщения: " + ex.Message + "/n " + ex.StackTrace);
                client.Abort();
                throw new OperationCanceledException(token);
            }
        }


        
    }
}

