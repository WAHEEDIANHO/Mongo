using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mongo.MessageBus
{
    public interface IMessageBus
    {
        Task PubishMessge(object message, string topoc_queue_name);
    }
}
