using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace mPower.OfferingsSystem
{
    public class ActionQueue: IActionQueue
    {
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private Task _task;

        public void Add(Action action)
        {
            _queue.Enqueue(action);
            if (_queue.Count == 1 && (_task == null || _task.IsCompleted))
            {
                _task = Task.Factory.StartNew(StartNext);
            }
        }

        private void StartNext()
        {
            Action action;
            _queue.TryDequeue(out action);
            action();
            if (_queue.Count > 0)
            {
                StartNext();
            }
        }

        public void Wait()
        {
            if (_task == null || _task.IsCompleted)
            {
                return;
            }
            _task.Wait();
        }
    }

    public interface IActionQueue
    {
        void Add(Action action);
    }

    [TestFixture]
    public class ActionQueueTest
    {
        [Test]
        public void single_test()
        {
            var actionQueue = new ActionQueue();
            var result = 0;
            var numbers = Enumerable.Range(1, 1000).ToList();
            var actions = numbers.Select((x) => new Action(() => result++)).ToList();
            foreach (var action in actions)
            {
                actionQueue.Add(action);
            }
            actionQueue.Wait();
            Assert.AreEqual(1000,result);
        }

        [Test]
        public void double_test()
        {
            var actionQueue = new ActionQueue();
            var result = 0;
            var numbers = Enumerable.Range(1, 1000).ToList();
            var actions = numbers.Select((x) => new Action(() => result++)).ToList();
            foreach (var action in actions)
            {
                actionQueue.Add(action);
            }
            actionQueue.Wait();
            foreach (var action in actions)
            {
                actionQueue.Add(action);
            }
            actionQueue.Wait();
            Assert.AreEqual(2000, result);
            
        }
    }
}