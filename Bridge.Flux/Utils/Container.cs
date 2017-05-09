using Bridge.React;
using System;
using System.Collections.Generic;

namespace Bridge.Flux.Utils
{
	/// <summary>
	/// A Flux Container is a React Component that sits atop a hierarchy of child components and manages coordination
	/// by sending UI actions to the dispatcher and retrieving state from stores that it depends on.
	/// </summary>
	/// <typeparam name="TProps">The type of this container's Props.</typeparam>
	/// <typeparam name="TState">The type of this container's State.</typeparam>
	public abstract class Container<TProps, TState> : Component<TProps, TState>
	{
		/// <summary>
		/// Constructs the container, assigning the initial component props.
		/// </summary>
		/// <param name="props">The props to assign to this container component.</param>
		public Container(TProps props)
			: base(props) { }

		/// <summary>
		/// Calculates the current state of the container.
		/// The implementation should only derive its state from the state of its stores.
		/// </summary>
		/// <returns>The container's state.</returns>
		protected abstract TState CalculateState();

		/// <summary>
		/// Returns the stores that this container is dependent on.
		/// The container will subscribe to the change events of these stores when mounted, and unsubscribe when unmounted.
		/// </summary>
		/// <returns>The container's stores.</returns>
		protected abstract IEnumerable<Store> GetStores();

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

		// Note: I've sealed the lifecycle methods here to avoid inadvertently forgetting to call required base methods in derived classes. This is just an
		// experiment; if it turns out derived classes can actually make good use of these lifecycle methods (I can't envision a scenario at the moment),
		// they can just be unsealed (a non-breaking change).

		protected sealed override void ComponentWillMount()
		{
			var stores = GetStores();
			if (stores == null)
				throw new Exception("GetStores cannot return null.");

			foreach (var store in stores)
				store.Change += StoreChanged;
		}

		protected sealed override void ComponentWillUnmount()
		{
			var stores = GetStores();
			if (stores == null)
				throw new Exception("GetStores cannot return null.");

			foreach (var store in stores)
				store.Change -= StoreChanged;
		}

		protected sealed override TState GetInitialState()
		{
			return CalculateState();
		}

		private void StoreChanged()
		{
			// The CalculateState method is free to return any valid value for the State type, this includes null if the state is a reference type.
			var newState = CalculateState();

			if (!AreStatesEqual(state, newState))
				SetState(newState);
		}
	}
}
