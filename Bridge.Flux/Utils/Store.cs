using System;

namespace Bridge.Flux.Utils
{
	// TODO: Class documentation
	public abstract class Store
	{
		/// <summary>
		/// Constructs the store, registering with the provided dispatcher.
		/// </summary>
		/// <param name="dispatcher">The dispatcher to register with to receive actions.</param>
		public Store(IDispatcher dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException(nameof(dispatcher));

			dispatcher.Register(HandleAction);
		}

		/// <summary>
		/// Provides an event that will be raised when the State of the store changes.
		/// </summary>
		public event Action Change;

		/// <summary>
		/// Handles an incoming dispatcher action.
		/// This method should call OnChange if the state of the store has changed after handling the action.
		/// </summary>
		/// <param name="action">The dispatcher action to handle.</param>
		protected abstract void HandleAction(IDispatcherAction action);

		/// <summary>
		/// Triggers the Change event.
		/// </summary>
		protected void OnChange()
		{
			Change?.Invoke();
		}
	}
}
