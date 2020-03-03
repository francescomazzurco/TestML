using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestML
{
    class Model
    {
        public float[] Features { get; set; }

        public float Label { get; set; }

        public Model() { }

        public Model(IEnumerable<float> values)
        {
            // label is the first column
            Label = values.First();
            Features = values.Skip(1).ToArray();
        }
    }
}
