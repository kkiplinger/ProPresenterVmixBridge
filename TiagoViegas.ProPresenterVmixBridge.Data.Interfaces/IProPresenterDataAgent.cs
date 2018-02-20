using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace TiagoViegas.ProPresenterVmixBridge.Data.Interfaces
{
    public interface IProPresenterDataAgent
    {
        bool Connected { get; }
        bool Connecting { get; }
        void Connect();
        void Close();
        void Listen(EventHandler<MessageEventArgs> onMessage);
    }
}
