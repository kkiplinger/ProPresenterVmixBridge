using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiagoViegas.ProPresenterVmixBridge.Business.Interfaces
{
    public interface IBridgeBc
    {
        bool BridgeOn { get; }
        void Bridge();
        void Close();
    }
}
