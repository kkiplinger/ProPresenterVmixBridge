using System;
using System.Threading;
using System.Threading.Tasks;
using TiagoViegas.ProPresenterVmixBridge.Entities;

namespace TiagoViegas.ProPresenterVmixBridge.Data.Interfaces
{
    public interface IProPresenterDataAgent
    {
        bool Connected { get; }
        bool Connecting { get; }
        Task Connect(CancellationToken cancellationToken);
        Task Close();
        void Listen(Action<ProPresenterNewSlideMessage> action);
        void StopListen();
        void LookForProPresenter();
    }
}
