using System;
using System.Runtime.Serialization;

namespace Dvrp.Ucc.Commons.Exceptions;

/// <summary>
/// 
/// </summary>
public class ParsingNullException : ArgumentNullException
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="parameterName"></param>
	public ParsingNullException(string parameterName): base(parameterName, "Empty object; TODO: It should be thrown in proper place with a proper exception.")
	{
	}

	/// <summary>
	/// This constructor is needed for serialization.
	/// </summary>
	/// <param name="info"></param>
	/// <param name="context"></param>
	protected ParsingNullException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}