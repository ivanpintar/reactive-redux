namespace ReactiveRedux

open System.Reactive.Subjects
open System
open Redux

module ReduxC =
    
    type Store<'s, 'e>= { Dispatch : Action<'e> 
                          StateSubject : Subject<'s> 
                          EventSubject : Subject<'e>
                          GetLastState : Func<'s>
                          ActionObserver : IDisposable}

    let CreateStore<'State, 'Event> initialState (reducer : Func<'State, 'Event, 'State>) =
        let reducer' = fun state action -> reducer.Invoke(state, action)    
        let store = createStore initialState reducer'
        { Dispatch = new Action<'Event>(store.dispatch)
          StateSubject = store.stateSubject
          EventSubject = store.eventSubject
          GetLastState = new Func<'State>(fun () -> store.getLastState())
          ActionObserver = store.actionObserver }
