using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls.Primitives;

namespace Metroist
{
    public class NoParalaxTitleLayer : PanningTitleLayer
    {
        protected override double PanRate
        {
            get { return 1d; }
        }
    }

    public class NoParalaxBackgroundLayer : PanningBackgroundLayer
    {
        protected override double PanRate
        {
            get { return 1d; }
        }
    }
}
