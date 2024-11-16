using Plugin.HttpClient.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Plugin.HttpClient.Project
{
	/// <summary>Коллекция элементов для тестирования в проекте</summary>
	[Serializable]
	[DesignTimeVisible(false)]
	public class HttpProjectItemCollection : ISerializable, IEnumerable<HttpProjectItem>
	{
		#region Fields
		/// <summary>Запросы удалённого сервера для тестирования</summary>
		[NonSerialized]
		private readonly List<HttpProjectItem> _items;

		[NonSerialized]
		private HttpProject _project;
		#endregion Fields

		#region Properties
		/// <summary>Проект к которому относится эта коллекция</summary>
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

		/// <summary>Количество элементов тестирования в коллекции</summary>
		public Int32 Count => this._items.Count;
		#endregion Properties

		#region Methods
		/// <summary>Создание пустого конструктора с указанием пустого массива коллекции</summary>
		public HttpProjectItemCollection()
			=> this._items = new List<HttpProjectItem>();

		/// <summary>Создание коллекции с указанием проекта к которому принадлежит коллекция</summary>
		/// <param name="project">Проект к которому принадлежит коллекция</param>
		public HttpProjectItemCollection(HttpProject project)
			: this()
			=> this.Project = project;

		/// <summary>Сериализационный конструктор</summary>
		/// <param name="info">Информция по сериализации</param>
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

		/// <summary>Запись в сериализованный поток элементов коллекции под определённым идентификатором</summary>
		/// <param name="info">Информация по сериализации</param>
		/// <param name="context">Stream</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(this._items.Count > 0)
				info.AddValue("I", this._items.ToArray());
		}

		/// <summary>Добавить новый адрес для стресстеста</summary>
		/// <param name="address">Ссылка на ресурс для проверки</param>
		/// <returns>Элемент проекта</returns>
		public HttpProjectItem Add(String address)
		{
			if(String.IsNullOrEmpty(address))
				throw new ArgumentNullException(nameof(address));

			HttpProjectItem result = new HttpProjectItem();
			this.Add(result);
			result.Address = address;//HACK: Project проставляется только в методе this.Add(HttpProjectItem)

			return result;
		}

		/// <summary>Добавить элемент в очередь текущего узла</summary>
		/// <param name="item">Элемент теста для добавления в очередь текущего уровня</param>
		public void Add(HttpProjectItem item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			item.Items.Project = this.Project;
			this._items.Add(item);
		}

		/// <summary>Добавить элемент в определённое место очереди</summary>
		/// <param name="item">Элемент для добавления в определённое место очереди</param>
		/// <param name="itemIndex">Индекс очереди</param>
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
				throw new ArgumentException($"Item {target.Address} not found");

			if(isAfter)
				this._items.Insert(index + 1, itemToInsert);
			else
				this._items.Insert(index, itemToInsert);
		}

		/// <summary>Проверка на существование элемента в коллекции</summary>
		/// <param name="item">Проверка на присутсвия этого элемента в коллекции</param>
		/// <returns>Элемент присутсвует в коллекции</returns>
		public Boolean Contains(HttpProjectItem item)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			return this._items.Contains(item);
		}

		/// <summary>Удалить элемент проекта</summary>
		/// <param name="item">Элемент проекта для удаления</param>
		/// <returns>Результат удаления элемента из массива</returns>
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

		/// <summary>Передвинуть элемент на определённую позицию в списке</summary>
		/// <param name="newParentItem">Новый родитель для передвигания элемента</param>
		/// <param name="item">Элемент проекта для передвигания</param>
		/// <param name="index">Индекс куда передвинуть элемент</param>
		/// <exception cref="ArgumentNullException"><c>item</c> равен null</exception>
		/// <exception cref="NotImplementedException">Движение узла из одного проекта в другой - не реализовано</exception>
		public void Move(HttpProjectItem newParentItem, HttpProjectItem item, Int32 index)
		{
			_ = item ?? throw new ArgumentNullException(nameof(item));

			//TODO: Эсли переносимый элемент из другого процесса, то удалить его из коллекции не получится
			//TODO: Проверить переносимость дочерних элементов элемента
			this.Remove(item);

			if(newParentItem == null)
				this.Insert(item, index);
			else
				newParentItem.Items.Insert(item, index);
		}

		/// <summary>Получить родительский тест</summary>
		/// <param name="child">Дочерний тест родителя которого необходимо найти</param>
		/// <returns>Родительский тест или null, если родитель не найден</returns>
		public HttpProjectItem GetParent(HttpProjectItem child)
		{
			_ = child ?? throw new ArgumentNullException(nameof(child));

			foreach(HttpProjectItem item in this._items)
			{
				HttpProjectItem result = this.GetParent(item, child);
				if(result != null)
					return result;
			}
			return null;
		}

		/// <summary>Рекурсивно поискать родительский элемент</summary>
		/// <param name="parent">Родитель, который возвращается если он им является</param>
		/// <param name="child">Дочерний элемент</param>
		/// <returns>Родительский тест или null, если родитель не найден</returns>
		private HttpProjectItem GetParent(HttpProjectItem parent, HttpProjectItem child)
		{
			foreach(HttpProjectItem item in parent.Items)
				if(item == child)
					return parent;
				else
				{
					HttpProjectItem result = item.Items.GetParent(item, child);
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

		/// <summary>Search exact match all items in the project tree</summary>
		/// <param name="search">Search template project item</param>
		/// <param name="isRelativeSearch">Search host by relative part but not absolute</param>
		/// <returns>All found items that contains the item with same properties</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public IEnumerable<HttpProjectItem> Find(HttpProjectItem search, Boolean isRelativeSearch = false)
		{
			_ = search ?? throw new ArgumentNullException(nameof(search));

			search.Items.Project = this._project;
			search.Address = search.Address;//Применяю шаблоны текущего проекта к адресу

			return this.Find(p => p.Equals(search, isRelativeSearch));
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

		public IEnumerator<HttpProjectItem> GetEnumerator()
			=> this._items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
		#endregion Methods
	}
}