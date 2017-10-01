namespace ReactiveRedux

open FSharp.Control.Reactive
open System.Reactive.Subjects
open System

module Redux =

    type Store<'s, 'e> = { dispatch : 'e -> unit 
                           stateSubject : Subject<'s> 
                           eventSubject : Subject<'e>
                           getLastState : unit -> 's
                           actionObserver : IDisposable}
                           
    let createStore<'State, 'Event> (initialState:'State) reducer =
        let stateSubject = new Subject<'State>();
        let eventSubject = new Subject<'Event>();
        
        let mutable state = initialState
        let getNewState event = 
            state <- reducer state event
            stateSubject.OnNext state
        
        let getLastState = fun () -> state
        let dispatch = eventSubject.OnNext

        let actionObserver = Observable.subscribe getNewState eventSubject

        { dispatch = dispatch
          stateSubject = stateSubject
          eventSubject = eventSubject
          getLastState = getLastState
          actionObserver = actionObserver }