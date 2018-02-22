
using System.Threading.Tasks;

namespace TiagoViegas.ProPresenterVmixBridge.Business.Interfaces
{
    public interface IBridgeBc
    {
        bool BridgeOn { get; }
        bool Connecting { get; }
        Task Bridge();
        Task Close();
    }
}
