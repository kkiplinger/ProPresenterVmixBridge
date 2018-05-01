using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Entities;

namespace TiagoViegas.ProPresenterVmixBridge.Data.Interfaces
{
    public interface IProPresenterDataAgent
    {
        IReadOnlyCollection<ProPresenterInstance> Instances { get; }
        bool Connected { get; }
        bool Connecting { get; }
        Task Connect(string instanceName, CancellationToken cancellationToken);
        Task Close();
        void Listen(Action<ProPresenterNewSlideMessage> action);
        void StopListen();
        void LookForProPresenter();
        event EventHandler<ProPresenterInstancesChangedEventArgs> OnProPresenterInstancesChanged;
        event EventHandler OnConnected;
        event EventHandler OnDisconnected;
    }
}
