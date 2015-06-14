using NLog.Config;

namespace NLog.Layouts
{
    /// <summary>
    /// JSON parameters dictionary.
    /// </summary>
    [NLogConfigurationItem]
    [ThreadAgnostic]
    public class JsonPropertiesRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPropertiesRenderer" /> class.
        /// </summary>
        public JsonPropertiesRenderer() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPropertiesRenderer" /> class.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        public JsonPropertiesRenderer(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        [RequiredParameter]
        public string Name { get; set; }
    }
}