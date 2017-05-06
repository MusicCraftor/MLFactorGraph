using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ResearchUtils;
using System.Collections;

namespace MLFactorGraph
{
    public class Dataset
    {
        public Dataset(DatabaseConnection dataSource, bool enableCache = false)
        {
            this.DataSource = dataSource;
            Refresh();

            this.FetchMethods = new Dictionary<Type, object>();

            this.CacheEnabled = enableCache;
            this.Cache = new DataCache();
        }
        
        public bool Refresh()
        {
            this.Available = this.DataSource.TestConnection();
            return this.Available;
        }

        public void EnableCache()
        {
            CacheEnabled = true;
        }
        public void DisableCache()
        {
            CacheEnabled = false;
            ClearCache();
        }
        public void ClearCache()
        {
            this.Cache.Clear();
        }

        public delegate T FetchMethod<T>(DatabaseConnection dataSource, ArrayList info);

        protected void RegisterMethod<T>(FetchMethod<T> method)
            where T : class, new()
        {
            FetchMethods[typeof(T)] = method;
        }

        public T InquiryData<T>(ArrayList info)
            where T : class, new()
        {
            if (!FetchMethods.ContainsKey(typeof(T)))
            {
                return null;
            }

            FetchMethod<T> method = FetchMethods[typeof(T)] as FetchMethod<T>;
            return InquiryDataWithMethod(info, method);
        }
        public T InquiryDataWithMethod<T>(ArrayList info, FetchMethod<T> method)
            where T : class, new()
        {
            if (!this.Available)
            {
                return null;
            }

            if (this.CacheEnabled)
            {
                object cachedValue = Cache.GetCachedObject(method, info);
                if (cachedValue != null && cachedValue is T)
                {
                    return cachedValue as T;
                }
            }

            object result = method(DataSource, info);
            if (this.CacheEnabled)
            {
                Cache.CacheObject(method, info, result);
            }

            return result as T;
        }

        DatabaseConnection DataSource;
        public bool Available { get; private set; }
        Dictionary<Type, object> FetchMethods;

        bool CacheEnabled;
        DataCache Cache;
    }

    class DataCache
    {
        public DataCache()
        {
            Storage = new Dictionary<object, Dictionary<ArrayList, object>>();
        }

        public void Clear()
        {
            Storage.Clear();
        }

        public object GetCachedObject(object key, ArrayList dependency)
        {
            if (key == null)
                return null;
            if (!IsDependable(dependency))
                return null;
            if (!Storage.ContainsKey(key))
                return null;

            KeyValuePair<ArrayList, object>? record = FindDependency(dependency, Storage[key]);
            if (record != null)
            {
                return record.Value.Value;
            }
            return null;
        }
        public bool CacheObject(object key, ArrayList dependency, object value)
        {
            if (!IsDependable(dependency))
                return false;

            if (!Storage.ContainsKey(key))
            {
                Storage.Add(key, new Dictionary<ArrayList, object>());
            }

            KeyValuePair<ArrayList, object>? record = FindDependency(dependency, Storage[key]);
            if (record == null)
            {
                Storage[key].Add(dependency, value);
            }
            else
            {
                Storage[key][record.Value.Key] = value;
            }

            return true;
        }

        bool IsDependable(ArrayList dependency)
        {
            if (dependency == null)
                return true;

            foreach (object obj in dependency)
            {
                if (!obj.GetType().IsValueType)
                {
                    return false;
                }
            }
            return true;
        }
        KeyValuePair<ArrayList, object>? FindDependency(ArrayList dependency, Dictionary<ArrayList, object> cacheBlock)
        {
            foreach (KeyValuePair<ArrayList, object> cacheRecord in cacheBlock)
            {
                if (IsValidObject(dependency, cacheRecord.Key))
                {
                    return cacheRecord;
                }
            }
            return null;
        }
        bool IsValidObject(ArrayList dependency, ArrayList cachedDependency)
        {
            if (dependency == null && cachedDependency == null)
            {
                return true;
            }
            else if (dependency == null || cachedDependency == null)
            {
                return false;
            }

            if (dependency.Count != cachedDependency.Count)
            {
                return false;
            }

            for (int i = 0; i < dependency.Count; i++)
            {
                if (!Object.Equals(dependency[i], cachedDependency[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /*
         * Dictionary<Key, Dictionary<Dependency, CachedObject>>
         */
        Dictionary<object, Dictionary<ArrayList, object>> Storage;
    }
}
