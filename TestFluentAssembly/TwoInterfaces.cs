using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFluentAssembly
{
    public interface IInterface1
    {
        IInterface2 Iface2 { get; }
    }

    public interface IInterface2
    {

    }
}
