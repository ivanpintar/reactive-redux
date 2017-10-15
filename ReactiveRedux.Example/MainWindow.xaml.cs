using ReactiveRedux.Example.Redux;
using System;
using System.Windows;
using System.Reactive.Linq;
using static ReactiveRedux.Example.Redux.Types;
using System.Reactive;
using System.Threading.Tasks;
using static ReactiveRedux.Redux;

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
            var incrementStream = Store.store.stateStream
                .Select(s => s.total.ToString())
                .Subscribe(UpdateView);

            // really simple "middleware" injection
            //Store.store.eventStream
            //    .Where(action => action.IsIncrement)
            //    .SelectMany(AsyncSideEffect)
            //    .Subscribe();
        }

        private async Task<Unit> AsyncSideEffect(Events action)
        {
            return await Task.Run(async () =>
            {
                await Task.Delay(2000);
                Store.store.dispatch.Invoke(Events.Decrement);
                return Unit.Default;
            });
        }

        private void UpdateView(string newTotal)
        {
            Dispatcher.BeginInvoke(new Action(() => Total.Text = newTotal));
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            Store.dispatchIncThenDec();
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            Store.dispatchDecrement();
        }
    }
}
