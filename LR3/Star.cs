using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LR3
{
    public class ShapeData
    {
        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public List<Point> Points { get; set; }

        [DataMember]
        public System.Windows.Media.Color StrokeColor { get; set; }

        [DataMember]
        public System.Windows.Media.Color FillColor { get; set; }

        [DataMember]
        public double StrokeThickness { get; set; }
    }
}
