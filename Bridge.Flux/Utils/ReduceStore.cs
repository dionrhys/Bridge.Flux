using System.Collections.Generic;

namespace Bridge.Flux.Utils
{
	// TODO: Class documentation (why use this over Store, etc.)
	public abstract class ReduceStore<TState> : Store
	{
		/// <summary>
		/// Constructs the store, setting the initial state to the one returned by GetInitialState and registering with the provided dispatcher.
		/// </summary>
		/// <param name="dispatcher">The dispatcher to register with to receive actions.</param>
		public ReduceStore(IDispatcher dispatcher) : base(dispatcher)
		{
			State = GetInitialState();
		}
		
		/// <summary>
		/// Gets the current state of the store.
		/// </summary>
		public TState State { get; private set; }

		/// <summary>
		/// Gets the initial state of the store.
		/// </summary>
		/// <returns>Initial state of the store.</returns>
		protected abstract TState GetInitialState();

		// TODO: FB Flux passes State as the first parameter to Reduce. While I like the idea that the method should rely solely on the previous and the given action,
		// we can't enforce that with class inheritance. It would seem redundant to pass the previous state when the derived class has access to State already.
		// It would be ideal if the Reduce method could enforced to be an explicitly pure function, but the only way I see of doing that is to have this class'
		// constructor accept a Func<TState, IDispatcherAction, TState> Reduce delegate in its constructor (which seems lamer than implementing an abstract method).

		/// <summary>
		/// Reduces an incoming dispatcher action to a new state for the store.
		/// This method should return the existing state if nothing has changed.
		/// </summary>
		/// <param name="action">The incoming dispatcher action.</param>
		/// <returns>New state for the store after applying the dispatcher action.</returns>
		protected abstract TState Reduce(IDispatcherAction action);

		/// <summary>
		/// Compares if two states are equal. The default behaviour uses <see cref="EqualityComparer{TState}.Default"/>, which should suffice for immutable types.
		/// </summary>
		/// <param name="state1">The first state.</param>
		/// <param name="state2">The second state.</param>
		/// <returns>True if both states are equal; false otherwise.</returns>
		protected virtual bool AreStatesEqual(TState state1, TState state2)
		{
			return EqualityComparer<TState>.Default.Equals(state1, state2);
		}

		protected override void HandleAction(IDispatcherAction action)
		{
			// The Reduce method is free to return any valid value for the State type, this includes null if the state is a reference type.
			var newState = Reduce(action);

			if (!AreStatesEqual(State, newState))
			{
				State = newState;
				OnChange();
			}
		}
	}
}
