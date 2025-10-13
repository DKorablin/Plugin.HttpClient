using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;

namespace Plugin.HttpClient.Project
{
	/// <summary>
	/// Serializable dictionary implementation that supports:
	/// 1. Binary / runtime serialization (ISerializable support via base Dictionary).
	/// 2. Explicit XML serialization through IXmlSerializable (custom key/value element layout).
	/// This avoids the need for additional wrapper classes when persisting dictionaries to XML.
	/// </summary>
	/// <typeparam name="TKey">Key type.</typeparam>
	/// <typeparam name="TValue">Value type.</typeparam>
	[Serializable]
	[XmlRoot("dictionary")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		/// <summary>Default constructor – creates an empty dictionary.</summary>
		public SerializableDictionary()
			: base() { }

		/// <summary>
		/// Deserialization constructor used during binary/runtime serialization.
		/// Reconstructs dictionary items from the SerializationInfo payload.
		/// </summary>
		/// <param name="info">Serialization info containing stored entries.</param>
		/// <param name="context">Streaming context.</param>
		/// <exception cref="NotSupportedException">Thrown when an unexpected entry name is encountered.</exception>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		private SerializableDictionary(SerializationInfo info, StreamingContext context)
		{
			foreach(SerializationEntry entry in info)
				switch(entry.Name)
				{
				case "I":
					KeyValuePair<TKey, TValue>[] items = (KeyValuePair<TKey, TValue>[])entry.Value;
					foreach(var item in items)
						base.Add(item.Key, item.Value);
					break;
				default:
					throw new NotSupportedException($"Entry {entry.Name} not supported");
				}
		}

		/// <summary>Adds dictionary content to the SerializationInfo for binary/runtime serialization.</summary>
		/// <param name="info">Serialization info to populate.</param>
		/// <param name="context">Streaming context.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(this.Count > 0)
				info.AddValue("I", this.ToArray());
		}

		/// <summary>Returns XML schema (not used). Always null because schema generation is skipped.</summary>
		/// <returns>Always null.</returns>
		public System.Xml.Schema.XmlSchema GetSchema()
			=> null;

		/// <summary>
		/// Reads dictionary items from XML. Expected format:
		/// &lt;dictionary&gt;
		///   &lt;item>&lt;key&gt;...&lt;/key&gt;&lt;value&gt;...&lt;/value&gt;&lt;/item&gt;
		///   ...
		/// &lt;/dictionary&gt;
		/// </summary>
		/// <param name="reader">XML reader positioned at the start of the dictionary element.</param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

			Boolean wasEmpty = reader.IsEmptyElement;

			if(wasEmpty)
				return;

			while(reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");

				reader.ReadStartElement("key");
				TKey key = (TKey)keySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement("value");
				TValue value = (TValue)valueSerializer.Deserialize(reader);
				reader.ReadEndElement();
				this.Add(key, value);

				reader.ReadEndElement();
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		/// <summary>Writes dictionary entries to XML using the custom &lt;item&gt;&lt;key/&gt;&lt;value/&gt;&lt;/item&gt; layout.</summary>
		/// <param name="writer">XML writer to output the dictionary.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

			foreach(TKey key in this.Keys)
			{
				writer.WriteStartElement("item");
				writer.WriteStartElement("key");
				keySerializer.Serialize(writer, key);
				writer.WriteEndElement();
				writer.WriteStartElement("value");
				TValue value = this[key];
				valueSerializer.Serialize(writer, value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}
	}
}