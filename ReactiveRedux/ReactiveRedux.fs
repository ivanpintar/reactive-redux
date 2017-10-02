namespace ReactiveRedux

open FSharp.Control.Reactive
open System.Reactive.Subjects
open System

module Redux =

    type Store<'s, 'e> = { dispatch : 'e -> unit 
                           stateStream : IObservable<'s>
                           eventStream : IObservable<'e>
                           getState : unit -> 's
                           dispose : unit -> unit }
                           interface IDisposable with 
                                member this.Dispose() = this.dispose()
                           
    let createStore<'State, 'Event> (initialState:'State) reducer =
        let stateSubject = new BehaviorSubject<'State>(initialState);
        let eventSubject = new Subject<'Event>();
        
        let createNewState event = 
            let oldState = stateSubject.Value
            reducer oldState event 
            |> stateSubject.OnNext         
        
        let dispatch = eventSubject.OnNext

        let eventObserver = Observable.subscribe createNewState eventSubject    
        
        let dispose () =
            stateSubject.Dispose()
            eventSubject.Dispose()
            eventObserver.Dispose()

        { dispatch = dispatch
          stateStream = Observable.asObservable stateSubject
          eventStream = Observable.asObservable eventSubject
          getState = fun () -> stateSubject.Value
          dispose = dispose }
