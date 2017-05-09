using System;
using System.Collections.Generic;

namespace Bridge.Flux
{
	// TODO: Documentation
	public interface IDispatcher
	{
		/// <summary>
		/// Dispatches an action that will be sent to all callbacks registered with this dispatcher.
		/// </summary>
		/// <param name="action">The action to dispatch; may not be null.</param>
		/// <remarks>
		/// This method cannot be called while a dispatch is in progress.
		/// </remarks>
		void Dispatch(IDispatcherAction action);

		/// <summary>
		/// Registers a callback to receive actions dispatched through this dispatcher.
		/// A token is returned to allow other callbacks to wait for this specific callback to be executed before another during a dispatch.
		/// </summary>
		/// <param name="callback">The callback; may not be null.</param>
		/// <returns>A token to allow this callback to be unregistered or waited upon.</returns>
		/// <remarks>
		/// This method cannot be called while a dispatch is in progress.
		/// </remarks>
		IDispatchToken Register(Action<IDispatcherAction> callback);

		/// <summary>
		/// Unregisters the callback associated with the given token.
		/// </summary>
		/// <param name="token">The dispatch token to unregister; may not be null.</param>
		/// <remarks>
		/// This method cannot be called while a dispatch is in progress.
		/// </remarks>
		void Unregister(IDispatchToken token);

		/// <summary>
		/// Waits for the callbacks associated with the given tokens to be called first during a dispatch operation.
		/// </summary>
		/// <param name="tokens">The tokens to wait on; may not be null.</param>
		/// <remarks>
		/// This method can only be called while a dispatch is in progress.
		/// </remarks>
		void WaitFor(IEnumerable<IDispatchToken> tokens);

		/// <summary>
		/// Waits for the callbacks associated with the given tokens to be called first during a dispatch operation.
		/// </summary>
		/// <param name="tokens">The tokens to wait on; may not be null.</param>
		/// <remarks>
		/// This method can only be called while a dispatch is in progress.
		/// </remarks>
		void WaitFor(params IDispatchToken[] tokens);
	}
}
