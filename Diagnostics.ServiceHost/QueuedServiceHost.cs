using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.ServiceModel.Configuration;
using System.Messaging;

namespace Diagnostics.ServiceHost
{
    
    public class QueuedServiceHost<T> : System.ServiceModel.ServiceHost
    {

        protected override void OnOpening()
        {
            foreach (ServiceEndpoint endpoint in Description.Endpoints)
            {
                endpoint.VerifyQueue();
            }
            base.OnOpening();
        }
        protected override void OnClosing()
        {
            PurgeQueues();

            base.OnClosing();
        }
        [Conditional("DEBUG")]
        void PurgeQueues()
        {
            foreach (ServiceEndpoint endpoint in Description.Endpoints)
            {
                QueuedServiceHelper.PurgeQueue(endpoint);
            }
        }

    }

    public static class QueuedServiceHelper
    {
        public static void PurgeQueue(ServiceEndpoint endpoint)
        {
            if (endpoint.Binding is NetMsmqBinding)
            {
                string queueName = GetQueueFromUri(endpoint.Address.Uri);

                if (MessageQueue.Exists(queueName) == true)
                {
                    MessageQueue queue = new MessageQueue(queueName);
                    queue.Purge();
                }
            }
        }

        public static void VerifyQueue(this ServiceEndpoint endpoint)
        {
            if (endpoint.Binding is NetMsmqBinding)
            {
                string queue = GetQueueFromUri(endpoint.Address.Uri);
                if (MessageQueue.Exists(queue) == false)
                {
                    MessageQueue.Create(queue, true);
                }
            }
        }

        static string GetQueueFromUri(Uri uri)
        {
            string queue = String.Empty;

            Debug.Assert(uri.Segments.Length == 3 || uri.Segments.Length == 2);
            if (uri.Segments[1] == @"private/")
            {
                queue = @".\private$\" + uri.Segments[2];
            }
            else
            {
                queue = uri.Host;
                foreach (string segment in uri.Segments)
                {
                    if (segment == "/")
                    {
                        continue;
                    }
                    string localSegment = segment;
                    if (segment[segment.Length - 1] == '/')
                    {
                        localSegment = segment.Remove(segment.Length - 1);
                    }
                    queue += @"\";
                    queue += localSegment;
                }
            }
            return queue;

        }

    }
}
