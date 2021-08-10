using System;
using System.Diagnostics.Tracing;
using System.Threading;

namespace Tessa.Performance.EventSources
{
    public sealed class CardRepositoryEventSource : EventSource
    {
        public static readonly CardRepositoryEventSource Log = new CardRepositoryEventSource();

        private const string EventSourceName = "Tessa.Performance.CardRepository";

        private long _totalNew;

        private long _currentNew;

        private long _failedNew;

        private IncrementingPollingCounter _newPerSecondCounter;

        private PollingCounter _totalNewCounter;

        private PollingCounter _currentNewCounter;

        private PollingCounter _failedNewCounter;

        private long _totalGet;

        private long _currentGet;

        private long _failedGet;

        private IncrementingPollingCounter _getPerSecondCounter;

        private PollingCounter _totalGetCounter;

        private PollingCounter _currentGetCounter;

        private PollingCounter _failedGetCounter;

        private long _totalStore;

        private long _currentStore;

        private long _failedStore;

        private IncrementingPollingCounter _storePerSecondCounter;

        private PollingCounter _totalStoreCounter;

        private PollingCounter _currentStoreCounter;

        private PollingCounter _failedStoreCounter;

        public CardRepositoryEventSource() : base(EventSourceName) { }

        protected override void OnEventCommand(EventCommandEventArgs command)
        {
            if (command.Command != EventCommand.Enable)
            {
                return;
            }

            _newPerSecondCounter = new IncrementingPollingCounter("card-new-per-second", this, () => Interlocked.Read(ref _totalNew))
            {
                DisplayName = "New requests",
                DisplayRateTimeScale = TimeSpan.FromSeconds(1),
            };

            _totalNewCounter = new PollingCounter("card-new-total", this, () => Interlocked.Read(ref _totalNew))
            {
                DisplayName = "Total new requests",
            };

            _currentNewCounter = new PollingCounter("card-new-current", this, () => Interlocked.Read(ref _currentGet))
            {
                DisplayName = "Current new requests",
            };

            _failedNewCounter = new PollingCounter("card-new-failed", this, () => Interlocked.Read(ref _failedNew))
            {
                DisplayName = "Failed new requests",
            };

            _getPerSecondCounter = new IncrementingPollingCounter("card-get-per-second", this, () => Interlocked.Read(ref _totalGet))
            {
                DisplayName = "Get requests",
                DisplayRateTimeScale = TimeSpan.FromSeconds(1),
            };

            _currentGetCounter = new PollingCounter("card-get-total", this, () => Interlocked.Read(ref _totalGet))
            {
                DisplayName = "Total get requests",
            };

            _totalGetCounter = new PollingCounter("card-get-current", this, () => Interlocked.Read(ref _currentGet))
            {
                DisplayName = "Current get requests",
            };

            _failedGetCounter = new PollingCounter("card-get-failed", this, () => Interlocked.Read(ref _failedGet))
            {
                DisplayName = "Failed get requests",
            };

            _storePerSecondCounter = new IncrementingPollingCounter("card-store-per-second", this, () => Interlocked.Read(ref _totalStore))
            {
                DisplayName = "Store requests",
                DisplayRateTimeScale = TimeSpan.FromSeconds(1),
            };

            _totalStoreCounter = new PollingCounter("card-store-total", this, () => Interlocked.Read(ref _totalStore))
            {
                DisplayName = "Total store requests",
            };

            _currentStoreCounter = new PollingCounter("card-store-current", this, () => Interlocked.Read(ref _currentStore))
            {
                DisplayName = "Current store requests",
            };

            _failedStoreCounter = new PollingCounter("card-store-failed", this, () => Interlocked.Read(ref _failedStore))
            {
                DisplayName = "Failed store requests",
            };
        }

        public void StartNew()
        {
            Interlocked.Increment(ref _totalNew);
            Interlocked.Increment(ref _currentNew);
        }

        public void EndNew()
        {
            Interlocked.Decrement(ref _currentNew);
        }

        public void FailNew()
        {
            Interlocked.Increment(ref _failedNew);
        }

        public void StartGet()
        {
            Interlocked.Increment(ref _totalGet);
            Interlocked.Increment(ref _currentGet);
        }
        
        public void EndGet()
        {
            Interlocked.Decrement(ref _currentGet);
        }

        public void FailGet()
        {
            Interlocked.Increment(ref _failedGet);
        }

        public void StartStore()
        {
            Interlocked.Increment(ref _totalStore);
            Interlocked.Increment(ref _currentStore);
        }

        public void EndStore()
        {
            Interlocked.Decrement(ref _currentStore);
        }

        public void FailStore()
        {
            Interlocked.Increment(ref _failedStore);
        }
    }
}
