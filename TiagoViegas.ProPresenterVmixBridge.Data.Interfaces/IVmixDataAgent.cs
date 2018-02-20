using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiagoViegas.ProPresenterVmixBridge.Data.Interfaces
{
    public interface IVmixDataAgent
    {
        Task SendText(string text);
    }
}
