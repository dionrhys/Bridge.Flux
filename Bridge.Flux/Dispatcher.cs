using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge.Flux
{
	// TODO: Documentation
	public sealed class Dispatcher : IDispatcher
	{
		private readonly Dictionary<IDispatchToken, Action<IDispatcherAction>> _callbacks = new Dictionary<IDispatchToken, Action<IDispatcherAction>>();
		private readonly HashSet<IDispatchToken> _executingCallbacks = new HashSet<IDispatchToken>();
		private readonly HashSet<IDispatchToken> _finishedCallbacks = new HashSet<IDispatchToken>();
		private bool _isDispatching = false;
		private IDispatcherAction _currentAction;

		/// <summary>
		/// Dispatches an action that will be sent to all callbacks registered with this dispatcher.
		/// </summary>
		/// <param name="action">The action to dispatch; may not be null.</param>
		/// <remarks>
		/// This method cannot be called while a dispatch is in progress.
		/// </remarks>
		public void Dispatch(IDispatcherAction action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			if (_isDispatching)
				throw new InvalidOperationException("Cannot dispatch while already dispatching.");

			_isDispatching = true;
			_currentAction = action;
			_executingCallbacks.Clear();
			_finishedCallbacks.Clear();

			foreach (var callback in _callbacks)
			{
				// Skip over callbacks that have already been called (by an earlier callback that used WaitFor)
				if (_finishedCallbacks.Contains(callback.Key))
					continue;

				// TODO: Assert _executingCallbacks should NEVER have this callback already in its set because reasons (explain)
				_executingCallbacks.Add(callback.Key);
				callback.Value(_currentAction);
				_executingCallbacks.Remove(callback.Key);
				_finishedCallbacks.Add(callback.Key);
			}

			_isDispatching = false;
			_currentAction = null;
			_finishedCallbacks.Clear();
		}

		/// <summary>
		/// Registers a callback to receive actions dispatched through this dispatcher.
		/// A token is returned to allow other callbacks to wait for this specific callback to be executed before another during a dispatch.
		/// </summary>
		/// <param name="callback">The callback; may not be null.</param>
		/// <returns>A token to allow this callback to be unregistered or waited upon.</returns>
		/// <remarks>
		/// This method cannot be called while a dispatch is in progress.
		/// </remarks>
		public IDispatchToken Register(Action<IDispatcherAction> callback)
		{
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			if (_isDispatching)
				throw new InvalidOperationException("Cannot register a dispatch token while dispatching.");

			var token = new DispatchToken();
			_callbacks.Add(token, callback);
			return token;
		}

		/// <summary>
		/// Unregisters the callback associated with the given token.
		/// </summary>
		/// <param name="token">The dispatch token to unregister; may not be null.</param>
		/// <remarks>
		/// This method cannot be called while a dispatch is in progress.
		/// </remarks>
		public void Unregister(IDispatchToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));
			if (!_callbacks.ContainsKey(token))
				throw new ArgumentException("", nameof(token));

			if (_isDispatching)
				throw new InvalidOperationException("Cannot unregister a dispatch token while dispatching.");

			// TODO: FB Flux throws on a token that isn't registered but I've gone the nice approach here of not erroring if that's the case (since the operation is idempotent)
			// Is there value in throwing if the token isn't registered? Just to enforce consumers be strict about their token usage?
			_callbacks.Remove(token);
		}

		/// <summary>
		/// Waits for the callbacks associated with the given tokens to be called first during a dispatch operation.
		/// </summary>
		/// <param name="tokens">The tokens to wait on; may not be null.</param>
		/// <remarks>
		/// This method can only be called while a dispatch is in progress.
		/// </remarks>
		public void WaitFor(IEnumerable<IDispatchToken> tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));
			if (!tokens.All(token => _callbacks.ContainsKey(token)))
				throw new ArgumentException("All given tokens must be registered with this dispatcher.", nameof(tokens));

			if (!_isDispatching)
				throw new InvalidOperationException("Can only call WaitFor while dispatching.");

			// Ensure there isn't a circular dependency of tokens waiting on each other
			// This check is done after the _isDispatching check because the state of _executingCallbacks is undefined if not currently dispatching
			// TODO: Does this logic hold firm even DURING looping over the tokens in the foreach after this check? Need to visualise the state better.
			if (tokens.Any(token => _executingCallbacks.Contains(token)))
				throw new ArgumentException("None of the tokens can have its callback already executing.", nameof(tokens));

			foreach (var token in tokens)
			{
				// Skip over callbacks that have already been called
				if (_finishedCallbacks.Contains(token))
					continue;

				_executingCallbacks.Add(token);
				_callbacks[token](_currentAction);
				_executingCallbacks.Remove(token);
				_finishedCallbacks.Add(token);
			}
		}

		/// <summary>
		/// Waits for the callbacks associated with the given tokens to be called first during a dispatch operation.
		/// </summary>
		/// <param name="tokens">The tokens to wait on; may not be null.</param>
		/// <remarks>
		/// This method can only be called while a dispatch is in progress.
		/// </remarks>
		public void WaitFor(params IDispatchToken[] tokens)
		{
			WaitFor((IEnumerable<IDispatchToken>)tokens);
		}

		private sealed class DispatchToken : IDispatchToken
		{
			// No need for any book-keeping here - dispatch tokens will differ based on reference equality
		}
	}
}
