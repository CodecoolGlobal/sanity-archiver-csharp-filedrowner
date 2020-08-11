using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanityArchiver.Application.Models
{
    /// <summary>
    /// Base of all Classes
    /// </summary>
    public abstract class BaseObject : PropertyNotifier
    {
        private IDictionary<string, object> _myValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Sets value in dict
        /// </summary>
        /// <param name="key">Key of the element if exist</param>
        /// <param name="value">Value we want to save</param>
        public void SetValue(string key, object value)
        {
            if (!_myValues.ContainsKey(key))
            {
                _myValues.Add(key, value);
            }
            else
            {
                _myValues[key] = value;
            }

            OnPropertyChanged(key);
        }

        /// <summary>
        /// Get value from Dictionary
        /// </summary>
        /// <typeparam name="T">Type param</typeparam>
        /// <param name="key">Key in the dictionary</param>
        /// <returns>Returns the value of the Key</returns>
        public T GetValue<T>(string key)
        {
            var value = GetValue(key);
            return (value is T) ? (T)value : default;
        }

        private object GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return _myValues.ContainsKey(key) ? _myValues[key] : null;
        }
    }
}
