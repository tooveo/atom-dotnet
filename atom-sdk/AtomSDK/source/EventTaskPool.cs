using System;

using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace ironsource {
	class EventTaskPool {
		private ConcurrentQueue<Action> events_;
		private bool isRunning_;

		private List<Thread> workers_;

		private int maxEvents_;

		public EventTaskPool(int maxThreads, int maxEvents) {
			maxEvents_ = maxEvents;
			events_ = new ConcurrentQueue<Action>();
			isRunning_ = true;

			workers_ = new List<Thread>();
			ThreadStart threadMethodHolder = new ThreadStart(this.TaskWorker);

			for (int index = 0; index < maxThreads; ++index) {
				Thread workerThread = new Thread(threadMethodHolder);
				workers_.Add(workerThread);

				workerThread.Start();
			}
		}

		~EventTaskPool() {
			foreach (Thread thread in workers_) {
				thread.Join();
			}

			isRunning_ = false;
		}

		private void TaskWorker() {
			while (isRunning_) {
				Action eventAction;
				if (!events_.TryDequeue(out eventAction)) {
					Thread.Sleep(25);
					continue;
				}

				eventAction();
			}
		}

		public void addEvent(Action eventAction) {
			if (events_.Count > maxEvents_) {
				return;
			}

			events_.Enqueue(eventAction);
		}
	}
}

