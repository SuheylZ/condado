/*The purpose of the class is to improve the performace of the GAL_Engine
 * Designed By Muzamil H;
 * Date 19 Aug 2014
 * futher improvments are required.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SignalR_Engine.GAL_Engine
{
    public interface IHasChanges
    {
        bool HasNotification { get; set; }
        void ApplyData(object item);
        Guid? AgentKey { get; }
        // bool Equals(object obj);
    }

    public class AgentChangeTracker<T> : IList<T> where T : IHasChanges
    {
        List<T> _list = new List<T>();
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            ((IHasChanges)item).HasNotification = true;
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public int Count { get { return _list.Count(s => ((IHasChanges)s).HasNotification); } }

        public bool IsReadOnly { get; private set; }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IHasChanges)item).HasNotification = true;
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        internal void AddOrUpdate(T item)
        {
            if (_list.Contains(item))
            {
                var @default = _list.FirstOrDefault(s => s.Equals(item));
                var trackedItem = (IHasChanges)@default;
                if (trackedItem != null)
                    trackedItem.ApplyData(item);
            }
            else
            {
                ((IHasChanges)item).HasNotification = true;
                _list.Add(item);
            }
        }
    }


    internal static class Extensions
    {
        internal static AgentChangeTracker<T> ApplyData<T>(this AgentChangeTracker<T> tracker, List<T> list) where T : IHasChanges
        {
            foreach (var item in list)
            {
                tracker.AddOrUpdate(item);
            }
            return tracker;
        }

        internal static List<string> GetAgnetsToNotify<T>(this AgentChangeTracker<T> tracker) where T : IHasChanges
        {
            return tracker.Where(s => s.HasNotification).Select(s => s.AgentKey.ToString()).ToList();
        }
    }

   
}

