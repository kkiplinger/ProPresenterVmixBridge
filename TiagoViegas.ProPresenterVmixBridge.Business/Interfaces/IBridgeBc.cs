
using System;
using System.Threading.Tasks;

namespace TiagoViegas.ProPresenterVmixBridge.Business.Interfaces
{
    public interface IBridgeBc
    {
        bool BridgeOn { get; }
        bool Connecting { get; }
        void OnConnection(Action action);
        Task Bridge();
        Task Close();
    }
}
