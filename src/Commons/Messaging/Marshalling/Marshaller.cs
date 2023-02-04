using System;
using System.Text;
using System.Text.Json;
using Dvrp.Ucc.Commons.Exceptions;
using Dvrp.Ucc.Commons.Messaging.Marshalling.Base;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.Commons.Messaging.Marshalling
{
	/// <summary>
	/// Class converting raw byte data to messages and inverse.
	/// </summary>
	public class Marshaller : IMarshaller<Message>
	{
		/// <summary>
		/// Convert raw bytes of data to messages.
		/// </summary>
		/// <param name="rawData">Messages data to convert.</param>
		/// <returns>Messages returned</returns>
		public Message[] Deserialize(byte[] rawData) => JsonSerializer.Deserialize<Message[]>(rawData) ?? throw new ParsingNullException(nameof(rawData));

		/// <summary>
		/// Convert messages to raw bytes
		/// </summary>
		/// <param name="messages">List of messages to convert</param>
		/// <returns>Raw bytes after conversion</returns>
		/// <exception cref="ArgumentNullException">Throws when argument messages is null.</exception>
		public byte[] Serialize(params Message[] messages)
		{
			var a = JsonSerializer.Serialize(messages);
			return Encoding.UTF8.GetBytes(a);
		}
	}
}