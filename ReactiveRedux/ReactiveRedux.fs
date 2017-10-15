namespace ReactiveRedux

open FSharp.Control.Reactive
open System.Reactive.Subjects
open System

module Redux =

    type Store<'s, 'e> = { dispatch : obj -> unit 
                           stateStream : IObservable<'s>
                           eventStream : IObservable<'e>
                           getState : unit -> 's
                           dispose : unit -> unit }
                           interface IDisposable with 
                                member this.Dispose() = this.dispose()
                           
    let createStore<'State, 'Event> (initialState:'State) reducer middleware =
        let stateSubject = new BehaviorSubject<'State>(initialState);
        let eventSubject = new Subject<'Event>();
        let getState = fun () -> stateSubject.Value;
        
        let createNewState event = 
            let oldState = stateSubject.Value
            reducer oldState event 
            |> stateSubject.OnNext         
        
        let dispatch (event:obj) = 
            match middleware with
            | None -> 
                match event with
                | :? 'Event as e -> eventSubject.OnNext e
                | _ -> ()
            | Some mw -> mw getState eventSubject.OnNext event

        let eventObserver = Observable.subscribe createNewState eventSubject    
        
        let dispose () =
            stateSubject.Dispose()
            eventSubject.Dispose()
            eventObserver.Dispose()

        { dispatch = dispatch
          stateStream = Observable.asObservable stateSubject
          eventStream = Observable.asObservable eventSubject
          getState = getState
          dispose = dispose }
          
    let thunkMiddleware<'State, 'Event> =
        fun getState dispatch (eventCreator:obj) -> 
            match eventCreator with
            | :? ((unit -> 'State) -> ('Event -> unit) -> unit) as ec ->
                ec getState dispatch
            | :? ((unit -> 'State) -> ('Event -> unit) -> Async<unit>) as ec ->
                async { do! ec getState dispatch } |> Async.Start
            | :? 'Event as e -> dispatch e
            | _ -> ()
