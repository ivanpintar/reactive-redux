namespace ReactiveRedux

open System
open Redux

module ReduxC =
    
    type Store<'s, 'e> = { Dispatch : Action<'e> 
                           StateStream : IObservable<'s> 
                           EventStream : IObservable<'e>
                           GetState : Func<'s>
                           dispose : unit -> unit }
                           interface IDisposable with
                                member this.Dispose() = this.dispose()

    let CreateStore<'State, 'Event> initialState (reducer : Func<'State, 'Event, 'State>) =
        let reducerFunc = fun state action -> reducer.Invoke(state, action)    
        let store = createStore initialState reducerFunc

        { Dispatch = new Action<'Event>(store.dispatch)
          StateStream = store.stateStream
          EventStream = store.eventStream
          GetState = new Func<'State>(fun () -> store.getState())
          dispose = store.dispose }
