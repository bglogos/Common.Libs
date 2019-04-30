namespace DapperDataClient.Models
{
    /// <summary>
    /// Summarized information for creating new data parameter.
    /// </summary>
    public class DataParameterInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is structured.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is structured; otherwise, <c>false</c>.
        /// </value>
        public bool IsStructured { get; set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public string DataType { get; set; }
    }
}
