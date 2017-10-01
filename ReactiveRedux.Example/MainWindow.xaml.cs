using ReactiveRedux.Example.Redux;
using System;
using System.Windows;
using System.Reactive.Linq;
using static ReactiveRedux.Example.Redux.Types;
using System.Reactive;
using System.Threading.Tasks;

namespace ReactiveRedux.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReduxC.Store<State, Events> _store;

        public MainWindow()
        {
            InitializeComponent();

            // create initial store and state
            var state = new State(0);
            _store = ReduxC.CreateStore<State, Events>(state, Reducers.reducer);

            // subscribe to state changes
            var incrementStream = _store.StateSubject
                .Select(s => s.total.ToString())
                .Subscribe(UpdateView);

            // really simple "middleware" injection
            _store.EventSubject
                .Where(action => action.IsIncrement)
                .SelectMany(AsyncSideEffect)
                .Subscribe();
        }

        private async Task<Unit> AsyncSideEffect(Events action)
        {
            return await Task.Run(async () =>
            {
                await Task.Delay(2000);
                _store.Dispatch(Events.Decrement);
                return Unit.Default;
            });
        }

        private void UpdateView(string newTotal)
        {
            Dispatcher.BeginInvoke(new Action(() => Total.Text = newTotal));
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            _store.Dispatch(Events.Increment);
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            _store.Dispatch(Events.Decrement);
        }
    }
}
