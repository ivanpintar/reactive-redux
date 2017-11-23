namespace ReactiveRedux.Example.Redux

module Types =

    type Events = 
        | Increment 
        | Decrement

    type State = { total : int }

module Reducers =    
    open Types

    let reducer event state = 
        match event with
        | Increment -> { state with total = state.total + 1 }
        | Decrement -> { state with total = state.total - 1 }

module Store =
    open Types
    open Reducers
    open ReactiveRedux.Redux
    
    let store = createStore<State, Events> { total = 10 } reducer
