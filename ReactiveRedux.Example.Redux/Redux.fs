namespace ReactiveRedux.Example.Redux

module Types =

    type Events = 
        | Increment 
        | Decrement

    type State = { total : int }

    let incThenDec (getState:(unit -> State)) dispatch = async {
        dispatch Events.Increment
        do! Async.Sleep(1000);
        dispatch Events.Increment
        do! Async.Sleep(1000);
        dispatch Events.Decrement
    }

module Reducers =    
    open Types

    let reducer state event = 
        match event with
        | Increment -> { state with total = state.total + 1 }
        | Decrement -> { state with total = state.total - 1 }

module Store =
    open Types
    open Reducers
    open ReactiveRedux.Redux

    let middleware = thunkMiddleware<State, Events>

    let store = createStore { total = 10 } reducer (Some middleware)

    let dispatchIncrement () = store.dispatch (box Increment)    
    let dispatchDecrement () = store.dispatch (box Decrement)
    let dispatchIncThenDec () = store.dispatch (box incThenDec)
