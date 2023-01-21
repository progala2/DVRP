namespace _15pl04.Ucc.Commons.Logging
{
    /// <summary>
    /// Allows to log messages with a different level.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs TRACE level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        void Trace(string s);
        /// <summary>
        /// Logs DEBUG level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        void Debug(string s);
        /// <summary>
        /// Logs INFO level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        void Info(string? s);
        /// <summary>
        /// Logs WARN level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        void Warn(string s);
        /// <summary>
        /// Logs ERROR level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        void Error(string? s);
    }
}