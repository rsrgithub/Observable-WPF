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
            var mousedownObs = Observable.FromEventPattern<MouseEventArgs>(Image, "MouseLeftButtonDown")
                .Select(evt => evt.EventArgs.GetPosition(Image));
            var mouseupObs = Observable.FromEventPattern<MouseEventArgs>(this, "MouseLeftButtonUp");
            var mousemoveObs = Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
                .Select(evt => evt.EventArgs.GetPosition(this));

            //var dragObs = mousedownObs.Merge((start) =>
            //{

            //});

            //var drag = mousedownObs.Select(start => mousemoveObs.TakeUntil(mouseupObs).Select(move => new
            //{
            //    X = move.X - start.X,
            //    Y = move.Y - start.Y
            //}));

            //var drag = mousedownObs.CombineLatest(mousemoveObs.TakeUntil(mouseupObs), (start, move) =>
            //{
            //    return new
            //    {
            //        X = move.X - start.X,
            //        Y = move.Y - start.Y
            //    };
            //});

            //var drag =
            //    from mouseDownPosition in mousedownObs
            //    from mouseMovePosition in mousemoveObs.StartWith(mouseDownPosition)
            //        .TakeUntil(mouseupObs)
            //    select mouseMovePosition;

            var drag = from start in mousedownObs
                       from move in mousemoveObs.TakeUntil(mouseupObs)
                       select
                           new
                           {
                               X = move.X - start.X,
                               Y = move.Y - start.Y
                           };
            drag.Subscribe(value =>
            {
                Canvas.SetLeft(Image, value.X);
                Canvas.SetTop(Image, value.Y);
            });
        }
    }
}
