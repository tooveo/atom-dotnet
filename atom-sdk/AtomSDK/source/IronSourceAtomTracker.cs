﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace ironsource {
    public class IronSourceAtomTracker {
        private int taskWorkersCount_ = 24;
        private int taskPoolSize_ = 10000;

        /// <summary>
        /// The flush interval in milliseconds
        /// </summary>
        private long flushInterval_ = 100000;

        private int bulkSize_ = 1000;

        /// <summary>
        /// The size of the bulk in bytes.
        /// </summary>
        private int bulkBytesSize_ = 64 * 1024;

        private IronSourceAtom api_;

        private bool isDebug_;
        private bool isFlushData_;


        private bool isRunWorker_ = true;
        private Thread eventWorkerThread_;

        private ConcurrentDictionary<string, string> streamData_;

        private IEventManager eventManager_;

        private EventTaskPool eventPool_;

        /// <summary>
        /// API Tracker constructor
        /// </summary>
        public IronSourceAtomTracker() {
            api_ = new IronSourceAtom();
            eventPool_ = new EventTaskPool(taskWorkersCount_, taskPoolSize_);

            eventManager_ = new QueueEventManager();
            streamData_ = new ConcurrentDictionary<string, string>();

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
        public void track(string stream, string data, string authKey = "") {
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
        public void flush() {
            isFlushData_ = true;
        }

        /// <summary>
        /// Events the worker.
        /// </summary>
        private void EventWorker() {
            long timerStartTime = Utils.GetCurrentMilliseconds();          
            long timerDeltaTime = 0;

            Dictionary<string, List<string>> eventsBuffer = new Dictionary<string, List<string>>();
            Dictionary<string, int> eventsSize = new Dictionary<string, int>();

            Action<string, string, List<string>> flushEvent = delegate(string stream, 
                                                            string authKey, 
                                                            List<string> events) {
                List<string> buffer = new List<string>(events);
                events.Clear();
                eventsSize[stream] = 0;
                timerDeltaTime = 0;

                eventPool_.addEvent(delegate() {            			
                        flushData(stream, authKey, buffer);
                    });
            };

            while (isRunWorker_) {
                foreach (var entry in streamData_) {
                    timerDeltaTime += Utils.GetCurrentMilliseconds() - timerStartTime;
                    timerStartTime = Utils.GetCurrentMilliseconds();

                    string streamName = entry.Key;

                    Event eventObject = eventManager_.getEvent(streamName);
                    if (eventObject == null) {
                        //Thread.Sleep(25);
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

                    if (eventsSize[streamName] >= bulkBytesSize_) {
                        flushEvent(streamName, streamData_[streamName], eventsBuffer[streamName]);
                    }

                    if (eventsBuffer[streamName].Count >= bulkSize_) {
                        flushEvent(streamName, streamData_[streamName], eventsBuffer[streamName]);
                    }

                    if (isFlushData_) {
                        flushEvent(streamName, streamData_[streamName], eventsBuffer[streamName]);
                    }

                    if (timerDeltaTime >= flushInterval_) {
                        timerDeltaTime = 0;
                        PrintLog("Timer");
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
        private void flushData(string stream, string authKey, List<string> data) {
            // data str 
            // send data
            while (true) {
                Response response = api_.PutEvents(stream, data, authKey);
                PrintLog("data: " + data + "; response: " + response.status);
                if (response.status < 500) {
                    break;
                }

                // fixme add Jitter
                Thread.Sleep(1000);

                PrintLog("Retry request: " + data);
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

