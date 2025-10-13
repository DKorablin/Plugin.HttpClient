using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Plugin.HttpClient.Project
{
	/// <summary>Collection of items for testing in a project</summary>
	[Serializable]
	[DesignTimeVisible(false)]
	public class HttpProjectItemCollection : ISerializable, IEnumerable<HttpProjectItem>
	{
		#region Fields
		/// <summary>Remote server requests for testing</summary>
		[NonSerialized]
		private readonly List<HttpProjectItem> _items;

		[NonSerialized]
		private HttpProject _project; // Back reference to owning project
		#endregion Fields

		#region Properties
		/// <summary>Project to which this collection belongs</summary>
		public HttpProject Project
		{
			get => this._project;
			set
			{
				this._project = value;
				foreach(HttpProjectItem item in this._items)
					item.Items.Project = value;
			}
		}

		/// <summary>Number of test items in the collection</summary>
		public Int32 Count => this._items.Count;
		#endregion Properties

		#region Methods
		/// <summary>Create empty collection</summary>
		public HttpProjectItemCollection()
			=> this._items = new List<HttpProjectItem>();

		/// <summary>Create collection specifying the project it belongs to</summary>
		/// <param name="project">Project that owns the collection</param>
		public HttpProjectItemCollection(HttpProject project)
			: this()
			=> this.Project = project;

		/// <summary>Serialization constructor</summary>
		/// <param name="info">Serialization info</param>
		/// <param name="context">Stream</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		private HttpProjectItemCollection(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach(SerializationEntry entry in info)
				switch(entry.Name)
				{
				case "Items":
				case "I":
					this._items = new List<HttpProjectItem>((HttpProjectItem[])entry.Value);
					break;
				default:
					throw new NotSupportedException($"Entry {entry.Name} not supported");
				}
		}

		/// <summary>Write collection items to serialized stream under a specific identifier</summary>
		/// <param name="info">Serialization info</param>
		/// <param name="context">Stream</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(this._items.Count > 0)
				info.AddValue("I", this._items.ToArray());
		}

		/// <summary>Add new address for stress test</summary>
		/// <param name="address">Resource URL to test</param>
		/// <returns>Project item</returns>
		public HttpProjectItem Add(String address)
		{
			if(String.IsNullOrEmpty(address))
				throw new ArgumentNullException(nameof(address));

			HttpProjectItem result = new HttpProjectItem();
			this.Add(result);
			result.Address = address;//HACK: Project is set only in the this.Add(HttpProjectItem) method

			return result;
		}

		/// <summary>Add item to current node queue</summary>
		/// <param name="item">Test item to add to current level queue</param>
		public void Add(HttpProjectItem item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			item.Items.Project = this.Project;
			this._items.Add(item);
		}

		/// <summary>Insert item at a specific position in the queue</summary>
		/// <param name="item">Item to insert</param>
		/// <param name="itemIndex">Queue index</param>
		public void Insert(HttpProjectItem item, Int32 itemIndex)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));
			if(itemIndex < 0)
				itemIndex = 0;

			item.Items.Project = this.Project;
			this._items.Insert(itemIndex, item);
		}

		/// <summary>Insert item before or after another item</summary>
		/// <param name="itemToInsert">The item to insert</param>
		/// <param name="target">The target item before or after item will be inserted</param>
		/// <param name="isAfter">Insert before target item or after target item</param>
		/// <exception cref="ArgumentNullException">item or target is null</exception>
		/// <exception cref="ArgumentException">target item not found</exception>
		public void Insert(HttpProjectItem itemToInsert, HttpProjectItem target, Boolean isAfter)
		{
			_ = itemToInsert ?? throw new ArgumentNullException(nameof(itemToInsert));
			_ = target ?? throw new ArgumentNullException(nameof(target));

			Int32 index = this._items.IndexOf(target);
			if(index == -1)
				throw new ArgumentException($"Item {target.Address} not found in the list of Items", nameof(target));

			if(isAfter)
				this._items.Insert(index + 1, itemToInsert);
			else
				this._items.Insert(index, itemToInsert);
		}

		/// <summary>Check if item exists in collection</summary>
		/// <param name="item">Item to check presence of</param>
		/// <returns>Item present in collection</returns>
		public Boolean Contains(HttpProjectItem item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			return this._items.Contains(item);
		}

		/// <summary>Remove project item</summary>
		/// <param name="item">Project item to remove</param>
		/// <returns>Result of removal from list</returns>
		public Boolean Remove(HttpProjectItem item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			if(item.Items.Project != this.Project)
				throw new InvalidOperationException("This element does not belongs to this project");

			Boolean result = this._items.Remove(item);
			if(!result)
			{
				HttpProjectItem parent = this.GetParent(item);
				if(parent != null)
					result = parent.Items.Remove(item);
			}

			return result;
		}

		/// <summary>Move item to a specific position in the list</summary>
		/// <param name="newParentItem">New parent item to move under (null to move to root)</param>
		/// <param name="item">Project item to move</param>
		/// <param name="index">Index to move to</param>
		/// <exception cref="ArgumentNullException"><c>item</c> is null</exception>
		/// <exception cref="NotImplementedException">Moving node between different projects is not implemented</exception>
		public void Move(HttpProjectItem newParentItem, HttpProjectItem item, Int32 index)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			//TODO: If movable item is from another process then removal from collection will not work
			//TODO: Verify movability of child elements
			this.Remove(item);

			if(newParentItem == null)
				this.Insert(item, index);
			else
				newParentItem.Items.Insert(item, index);
		}

		/// <summary>Get parent test item</summary>
		/// <param name="child">Child test whose parent to find</param>
		/// <returns>Parent test or null if not found</returns>
		public HttpProjectItem GetParent(HttpProjectItem child)
		{
			_ = child ?? throw new ArgumentNullException(nameof(child));

			foreach(HttpProjectItem item in this._items)
			{
				HttpProjectItem result = GetParent(item, child);
				if(result != null)
					return result;
			}
			return null;
		}

		/// <summary>Recursively search for parent item</summary>
		/// <param name="parent">Candidate parent returned if it is the parent</param>
		/// <param name="child">Child item</param>
		/// <returns>Parent test or null if not found</returns>
		private static HttpProjectItem GetParent(HttpProjectItem parent, HttpProjectItem child)
		{
			foreach(HttpProjectItem item in parent.Items)
				if(item == child)
					return parent;
				else
				{
					HttpProjectItem result = GetParent(item, child);
					if(result != null)
						return result;
				}
			return null;
		}

		/// <summary>Enumerate all nodes and all child nodes</summary>
		/// <returns>All nodes in hierarchy</returns>
		public IEnumerable<HttpProjectItem> EnumerateItems()
		{
			foreach(HttpProjectItem item in this._items)
			{
				yield return item;

				foreach(HttpProjectItem childItem in item.Items.EnumerateItems())
					yield return childItem;
			}
		}

		/// <summary>Search parent project item by child item.</summary>
		/// <param name="child">Child item which parent to find</param>
		/// <returns>Found parent item or null</returns>
		/// <exception cref="ArgumentNullException">Child item is null</exception>
		public HttpProjectItem FindParentByReference(HttpProjectItem child)
		{
			_ = child ?? throw new ArgumentNullException(nameof(child));

			return this.Find(p => p.Items.Contains(child)).FirstOrDefault();
		}

		/// <summary>Search exact match all items in the project tree</summary>
		/// <param name="search">Search template project item</param>
		/// <param name="isRelativeSearch">Search host by relative part but not absolute</param>
		/// <returns>All found items that contains the item with same properties</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public IEnumerable<HttpProjectItem> Find(HttpProjectItem search, Boolean isRelativeSearch = false)
		{
			_ = search ?? throw new ArgumentNullException(nameof(search));

			search.Items.Project = this._project;
			search.Address = search.Address;//Apply current project templates to address

			return this.Find(p => p.Equals(search, isRelativeSearch));
		}

		public IEnumerable<HttpProjectItem> Find(Func<HttpProjectItem, Boolean> callback, IEnumerable<HttpProjectItem> items = null)
		{
			if(items == null)
				items = this;

			foreach(HttpProjectItem item in items)
			{
				if(callback(item))
					yield return item;

				foreach(HttpProjectItem subItem in this.Find(callback, item.Items))
					yield return subItem;
			}
		}
 
		/// <summary>Returns generic enumerator over root-level project items.</summary>
		public IEnumerator<HttpProjectItem> GetEnumerator()
			=> this._items.GetEnumerator();

		/// <summary>Returns non-generic enumerator over root-level project items.</summary>
		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();

		#endregion Methods
	}
}