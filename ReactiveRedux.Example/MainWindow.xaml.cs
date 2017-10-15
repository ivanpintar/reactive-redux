using ReactiveRedux.Example.Redux;
using System;
using System.Windows;
using System.Reactive.Linq;
using static ReactiveRedux.Example.Redux.Types;
using System.Reactive;
using System.Threading.Tasks;
using static ReactiveRedux.Redux;
using System.Collections.Generic;
using System.Threading;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace ReactiveRedux.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // subscribe to state changes
            Store.store.stateStream
                .Select(s => s.total.ToString())
                .Subscribe(UpdateView);

            var incrementStream =
                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Increment.Click += h, h => Increment.Click -= h)
                .Select(e => Events.Increment);

            // example of async actions using streams
            var decrementStream =
                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Decrement.Click += h, h => Decrement.Click -= h)
                .SelectMany(e =>
                {
                    Log(Store.store.getState.Invoke(null));
                    var first = Observable.Return(Events.Decrement);
                    var sec = DelayOneSecond().Select(e2 =>
                    {
                        Log(Store.store.getState.Invoke(null));
                        return Events.Increment;
                    });
                    return first.Merge(sec);
                });

            Store.store.addEventStream.Invoke(incrementStream);
            Store.store.addEventStream.Invoke(decrementStream);

        }

        private void UpdateView(string newTotal)
        {
            Dispatcher.BeginInvoke(new Action(() => Total.Text = newTotal));
        }

        private void Log(object o)
        {
            System.Diagnostics.Debug.WriteLine(o);
        }

        private IObservable<Unit> DelayOneSecond()
        {
            return Task.Delay(1000).ToObservable();
        }
    }
}
