using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;

namespace Plugin.HttpClient.Project
{
	[Serializable]
	[XmlRoot("dictionary")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		public SerializableDictionary()
			: base() { }

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

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(this.Count > 0)
				info.AddValue("I", this.ToArray());
		}

		public System.Xml.Schema.XmlSchema GetSchema()
			=> null;

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