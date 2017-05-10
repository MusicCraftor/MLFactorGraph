using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    interface ILayerNode
    {
        uint Id { get; }
        short Label { get; set; }
        Dictionary<int, object> Attribute { get; set; }
        MLFGraph Graph { get; }
    }
}
