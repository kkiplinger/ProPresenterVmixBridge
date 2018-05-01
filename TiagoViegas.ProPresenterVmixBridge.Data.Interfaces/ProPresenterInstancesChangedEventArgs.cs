using System;
using System.Collections.Generic;
using TiagoViegas.ProPresenterVmixBridge.Entities;

namespace TiagoViegas.ProPresenterVmixBridge.Data.Interfaces
{
    public class ProPresenterInstancesChangedEventArgs : EventArgs
    {
        public IEnumerable<ProPresenterInstance> Instances { get; set; }
    }
}
