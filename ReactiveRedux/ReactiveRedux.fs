namespace ReactiveRedux

open FSharp.Control.Reactive
open System.Reactive.Subjects
open System.Reactive.Linq
open System

module Redux =

    type Store<'S, 'E> = { addEventStream : IObservable<'E> -> IDisposable 
                           stateStream : IObservable<'S>
                           getState : unit -> 'S
                           dispose : unit -> unit }
                           interface IDisposable with 
                                member this.Dispose() = this.dispose()
                           
    let createStore<'S, 'E> (initialState:'S) (reducer:'E -> 'S -> 'S) =
        let stateSubject = new BehaviorSubject<'S>(initialState);
        let getState () = stateSubject.Value;
        
        let createNewState event = 
            getState ()
            |> reducer event 
            |> stateSubject.OnNext         
        
        let addEventStream = Observable.subscribe createNewState            
                 
        { addEventStream = addEventStream
          stateStream = Observable.asObservable stateSubject
          getState = getState
          dispose = stateSubject.Dispose }          