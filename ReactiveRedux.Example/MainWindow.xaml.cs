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

            // create a stream of increment events from clicking the increment button
            var incrementStream =
                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Increment.Click += h, h => Increment.Click -= h)
                .Select(e => Events.Increment);
            
            // create a stream of events and async actions when clicking the decrement button
            var decrementStream =
                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Decrement.Click += h, h => Decrement.Click -= h)
                .SelectMany(e =>
                {
                    // when clicked immediately decrement the counter
                    var first = Observable.Return(Events.Decrement);

                    // after one second decrement again
                    var second = DelayOneSecond().Select(e2 =>
                    {
                        return Events.Decrement;
                    });

                    // merge the two streams
                    return first.Merge(second);
                });

            // add the two streams of events to the store
            Store.store.addEventStream.Invoke(incrementStream);
            Store.store.addEventStream.Invoke(decrementStream);
        }

        private void UpdateView(string newTotal)
        {
            Dispatcher.BeginInvoke(new Action(() => Total.Text = newTotal));
        }
        
        private IObservable<Unit> DelayOneSecond()
        {
            return Task.Delay(1000).ToObservable();
        }
    }
}
