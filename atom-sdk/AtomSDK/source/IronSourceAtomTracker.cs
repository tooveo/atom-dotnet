using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace ironsource {
    /// <summary>
    /// Iron source atom tracker.
    /// </summary>
    public class IronSourceAtomTracker {
        private const int TASK_WORKERS_COUNT_ = 1;
        private const int TASK_POOL_SIZE_ = 500;

        /// <summary>
        /// The flush interval in milliseconds
        /// </summary>
        private long flushInterval_ = 1000;

        private int bulkLength_ = 500;

        /// <summary>
        /// The size of the bulk in bytes.
        /// </summary>
        private int bulkBytesSize_ = 64 * 1024;

        // Jitter time
        private double minTime_ = 1;
        private double maxTime_ = 10;

        private IronSourceAtom api_;

        private bool isDebug_;
        private bool isFlushData_;


        private bool isRunWorker_ = true;
        private Thread eventWorkerThread_;

        private ConcurrentDictionary<string, string> streamData_;

        private IEventManager eventManager_;
        private EventTaskPool eventPool_;
        private Random random_;

        /// <summary>
        /// API Tracker constructor
        /// </summary>
        /// <param name="taskWorkersCount">
        /// <see cref="int"/> task workers count
        /// </param>
        /// <param name="taskPoolSize">
        /// <see cref="int"/> task pool size
        /// </param>
        public IronSourceAtomTracker(int taskWorkersCount=TASK_WORKERS_COUNT_, int taskPoolSize=TASK_POOL_SIZE_) {
            api_ = new IronSourceAtom();
            eventPool_ = new EventTaskPool(taskWorkersCount, taskPoolSize);

            eventManager_ = new QueueEventManager();
            streamData_ = new ConcurrentDictionary<string, string>();

            random_ = new Random();

            ThreadStart threadMethodHolder = new ThreadStart(this.EventWorker);
            eventWorkerThread_ = new Thread(threadMethodHolder);
            eventWorkerThread_.Start();
        }

        /// <summary>
        /// Clear craeted IronSourceCoroutineHandler
        /// </summary>       
        public void Stop() {
            isRunWorker_ = false;
            eventPool_.Stop();
        }

        /// <summary>
        /// Sets the event manager.
        /// </summary>
        /// <param name="eventManager">Event manager.</param>
        public void SetEventManager(IEventManager eventManager) {
            eventManager_ = eventManager;
        }

        /// <summary>
        /// Enabling print debug information
        /// </summary>
        /// <param name="isDebug">
        /// If set to <c>true</c> is debug.
        /// </param>
        public void EnableDebug(bool isDebug) {
            isDebug_ = isDebug;

            api_.EnableDebug(isDebug);
        }

        /// <summary>
        /// Set Auth Key for stream
        /// </summary>  
        /// <param name="authKey">
        /// <see cref="string"/> for secret key of stream.
        /// </param>
        public virtual void SetAuth(string authKey) {
            api_.SetAuth(authKey);
        }

        /// <summary>
        /// Set endpoint for send data
        /// </summary>
        /// <param name="endpoint">
        /// <see cref="string"/> for address of server
        /// </param>
        public void SetEndpoint(string endpoint) {
            api_.SetEndpoint(endpoint);
        }

        /// <summary>
        /// Set Bulk data count
        /// </summary>
        /// <param name="bulkSize">
        /// <see cref="int"/> Count of event for flush
        /// </param>
        public void SetBulkSize(int bulkSize) {
            bulkSize_ = bulkSize;
        }

        /// <summary>
        /// Set Bult data bytes size
        /// </summary>
        /// <param name="bulkBytesSize">
        /// <see cref="int"/> Size in bytes
        /// </param>
        public void SetBulkBytesSize(int bulkBytesSize) {
            bulkBytesSize_ = bulkBytesSize;
        }

        /// <summary>
        /// Set intervals for flushing data
        /// </summary>
        /// <param name="flushInterval">
        /// <see cref="float"/> Intervals in seconds
        /// </param>
        public void SetFlushInterval(long flushInterval) {
            flushInterval_ = flushInterval;
        }

        /// <summary>
        /// Track data to server
        /// </summary>
        /// <param name="stream">
        /// <see cref="string"/> Name of the stream
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> Info for sending
        /// </param>
        /// <param name="authKey">
        /// <see cref="string"/> Secret token for stream
        /// </param>
        public void Track(string stream, string data, string authKey = "") {
            if (authKey.Length == 0) {
                authKey = api_.GetAuth();
            }

            if (!streamData_.ContainsKey(stream)) {
                streamData_.TryAdd(stream, authKey);
            }

            eventManager_.addEvent(new Event(stream, data, authKey));           
        }

        /// <summary>
        /// Flush all data to server
        /// </summary>
        public void Flush() {
            isFlushData_ = true;
        }
       
        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <returns>The duration.</returns>
        /// <param name="attempt">Attempt.</param>
        private double GetDuration(int attempt) {           
            double duration = minTime_ * Math.Pow(2, attempt);         
            duration = (random_.NextDouble() * (duration - minTime_)) + minTime_;

            if (duration > maxTime_) {
                duration = maxTime_;
            }

            return duration;
        }

        /// <summary>
        /// Events the worker.
        /// </summary>
        private void EventWorker() {
            Dictionary<string, long> timerStartTime = new Dictionary<string, long>();
            Dictionary<string, long> timerDeltaTime = new Dictionary<string, long>();

            // temporary buffers for hold event data per stream
            Dictionary<string, List<string>> eventsBuffer = new Dictionary<string, List<string>>();
            // buffers size storage
            Dictionary<string, int> eventsSize = new Dictionary<string, int>();

            Action<string, string, List<string>> flushEvent = delegate(string stream, 
                                                            string authKey, 
                                                            List<string> events) {
                List<string> buffer = new List<string>(events);
                events.Clear();
                eventsSize[stream] = 0;

                eventPool_.addEvent(delegate() {            			
                        FlushData(stream, authKey, buffer);
                    });
            };

            while (isRunWorker_) {
                foreach (var entry in streamData_) {
                    string streamName = entry.Key;
                    if (!timerStartTime.ContainsKey(streamName)) {
                        timerStartTime.Add(streamName, Utils.GetCurrentMilliseconds());
                    }

                    if (!timerDeltaTime.ContainsKey(streamName)) {
                        timerDeltaTime.Add(streamName, 0);
                    }

                    timerDeltaTime[streamName] += Utils.GetCurrentMilliseconds() - timerStartTime[streamName];
                    timerStartTime[streamName] = Utils.GetCurrentMilliseconds();

                    if (timerDeltaTime[streamName] >= flushInterval_) {
                        PrintLog("Timer for stream: " + streamName + "; delta timer: " + timerDeltaTime[streamName]);
                        timerDeltaTime[streamName] = 0;
                        if (eventsBuffer.ContainsKey(streamName)) {
                            if (eventsBuffer[streamName].Count > 0) {
                                flushEvent(streamName, streamData_[streamName], eventsBuffer[streamName]);
                            }
                        }
                    }

                    Event eventObject = eventManager_.getEvent(streamName);
                    if (eventObject == null) {
                        Thread.Sleep(25);
                        continue;
                    }

                    if (!eventsSize.ContainsKey(streamName)) {
                        eventsSize.Add(streamName, 0);
                    }

                    if (!eventsBuffer.ContainsKey(streamName)) {
                        eventsBuffer.Add(streamName, new List<string>());
                    }

                    eventsSize[streamName] += Encoding.Unicode.GetByteCount(eventObject.data_);
                    eventsBuffer[streamName].Add(eventObject.data_);

                    if (isFlushData_) {
                        flushEvent(streamName, streamData_[streamName], eventsBuffer[streamName]);
                    } else if (eventsSize[streamName] >= bulkBytesSize_) {
                        flushEvent(streamName, streamData_[streamName], eventsBuffer[streamName]);
                    } else if (eventsBuffer[streamName].Count >= bulkSize_) {
                        flushEvent(streamName, streamData_[streamName], eventsBuffer[streamName]);
                    }
                }

                if (isFlushData_) {
                    isFlushData_ = false;
                }
            }
        }

        /// <summary>
        /// Flushs the data.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="authKey">Auth key.</param>
        /// <param name="data">Data.</param>
        private void FlushData(string stream, string authKey, List<string> data) {
            // data str 
            // send data
            int attempt = 1;

            while (true) {
                Response response = api_.PutEvents(stream, data, authKey);
                PrintLog("data: " + data + "; response: " + response.status);
                if (response.status < 500 && response.status > 1) {
                    break;
                }

                int duration = (int)(GetDuration(attempt++) * 1000);
                Thread.Sleep(duration);

                PrintLog("Url: " + api_.GetEndpoint() + " Retry request: " + data);
            }
        }

        /// <summary>
        /// Prints the log.
        /// </summary>
        /// <param name="logData">Log data.</param>
        protected void PrintLog(string logData) {			
            Debug.WriteLineIf(isDebug_, logData);
        }
    }
}

