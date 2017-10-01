namespace ReactiveRedux.Example.Redux

module Types =

    type Events = 
        | Increment 
        | Decrement

    type State = { total : int }

module Reducers =
    
    open Types

    let reducer state event = 
        match event with
        | Increment -> { state with total = state.total + 2 }
        | Decrement -> { state with total = state.total - 1 }
