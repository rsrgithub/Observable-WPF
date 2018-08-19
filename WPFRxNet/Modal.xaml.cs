using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFRxNet
{
    /// <summary>
    /// Interaction logic for Modal.xaml
    /// </summary>
    public partial class Modal : Window
    {
        public Modal()
        {
            InitializeComponent();
            SetupDrag();
        }

        private void SetupDrag()
        {
            var target = Border;

            var mouseDownObs = Observable.FromEventPattern<MouseEventArgs>(target, "MouseLeftButtonDown")
                .Select(evt =>
                {
                    var mousePos = evt.EventArgs.GetPosition(target);
                    return mousePos;
                });

            var mouseUpObs = Observable.FromEventPattern<MouseEventArgs>(this, "MouseLeftButtonUp");

            var mouseLeaveObs = Observable.FromEventPattern<MouseEventArgs>(this, "MouseLeave");

            var mouseMoveObs = Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
                .Select(evt =>
                {
                    var mousePos = evt.EventArgs.GetPosition(this);
                    return mousePos;
                });


            var drag = mouseDownObs
                .CombineLatest(mouseMoveObs, (p1, p2) => new { p1, p2 })
                .TakeUntil(mouseUpObs.Merge(mouseLeaveObs))
                .DistinctUntilChanged()
                .Repeat();

            drag.Subscribe(paired =>
            {
                Console.WriteLine($"paired----{paired}");
                var x = paired.p2.X - paired.p1.X;
                var y = paired.p2.Y - paired.p1.Y;
                Canvas.SetLeft(target, x);
                Canvas.SetTop(target, y);
            });
        }
    }
}
