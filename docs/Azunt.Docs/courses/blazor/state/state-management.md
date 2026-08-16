# State management

State can live in a component, a scoped service, browser storage, or a persistent data store.

## Component state

Use local component state for values that only matter to that component instance.

## Shared state

A scoped service is a simple option when several components need the same in-memory state.

### Persistence

Use a durable store when state must survive a browser refresh or application restart.
