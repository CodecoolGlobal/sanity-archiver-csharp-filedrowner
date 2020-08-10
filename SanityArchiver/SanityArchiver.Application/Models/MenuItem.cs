using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanityArchiver.Application.Models
{
    /// <summary>
    /// MenuItem class
    /// </summary>
    public class MenuItem
    {
        private ObservableCollection<MenuItem> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        public MenuItem()
        {
            _items = new ObservableCollection<MenuItem>();
        }

        /// <summary>
        /// Gets or sets title Property
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets items
        /// </summary>
        public ObservableCollection<MenuItem> Items
        {
            get
            {
                return _items;
            }
        }

        /// <summary>
        /// Adds to the Items list of the entity
        /// </summary>
        /// <param name="item">Item u want to add</param>
        public void AddToItems(MenuItem item)
        {
            _items.Add(item);
        }
    }
}
