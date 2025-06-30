using Internal.Extensions;
using System;
using System.Threading;
using UnityEngine;

namespace Simplito.Utils.Observables
{
    internal static class ObservableExtensions
    {
        private static readonly Action<Exception>
            DefaultOnError = static exception => Debug.LogException(exception);

        private static readonly Action DefaultOnCompleted = static () => { };

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onChange = null,
            Action<Exception> onError = null, Action onCompleted = null)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable));
            if (onChange == null && onError == null && onCompleted == null)
                throw new InvalidOperationException("All callbacks cannot be null");

            return observable.Subscribe(new CallbackObserver<T>(onChange, onError, onCompleted));
        }

        private class CallbackObserver<T> : IObserver<T>
        {
            private static readonly Action<T> DefaultOnChange = static _ => { };

            private readonly Action<T> onChange;
            private readonly Action onCompleted;
            private readonly Action<Exception> onError;

            public CallbackObserver(Action<T> onChange, Action<Exception> onError, Action onCompleted)
            {
                this.onChange = onChange ?? DefaultOnChange;
                this.onError = onError ?? DefaultOnError;
                this.onCompleted = onCompleted ?? DefaultOnCompleted;
            }

            public void OnCompleted()
            {
                onCompleted.Invoke();
            }

            public void OnError(Exception error)
            {
                onError.Invoke(error);
            }

            public void OnNext(T value)
            {
                onChange.Invoke(value);
            }
        }

        public static IObservable<T> ObserveOn<T>(this IObservable<T> observable, SynchronizationContext context)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable));
            return new ObserveOnDispatcher<T>(observable, context);
        }

        private class ObserveOnDispatcher<T> : IObservable<T>
        {
            public ObserveOnDispatcher(IObservable<T> wrapped,
                SynchronizationContext context)
            {
                Context = context;
                Wrapped = wrapped;
            }

            private IObservable<T> Wrapped { get; }
            private SynchronizationContext Context { get; }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException(nameof(observer));
                return new Subscription(this, observer);
            }

            private class Subscription : IDisposable, IObserver<T>
            {
                public Subscription(ObserveOnDispatcher<T> parent, IObserver<T> wrappedObserver)
                {
                    Parent = parent;
                    WrappedObserver = wrappedObserver;
                    WrappedSubscription = Parent.Wrapped.Subscribe(this);
                }

                private ObserveOnDispatcher<T> Parent { get; }
                private IObserver<T> WrappedObserver { get; }
                private IDisposable WrappedSubscription { get; set; }

                public void Dispose()
                {
                    WrappedSubscription?.Dispose();
                    WrappedSubscription = null;
                }

                public async void OnCompleted()
                {
                    await Parent.Context;
                    try
                    {
                        WrappedObserver.OnCompleted();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }

                public async void OnError(Exception error)
                {
                    await Parent.Context;
                    try
                    {
                        WrappedObserver.OnError(error);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }

                public async void OnNext(T value)
                {
                    await Parent.Context;
                    try
                    {
                        WrappedObserver.OnNext(value);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }
        }
    }
}