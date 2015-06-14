// 
// Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using NLog.Internal;

namespace NLog.Layouts
{
    using Config;
    using LayoutRenderers.Wrappers;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A specialized layout that renders JSON-formatted events.
    /// </summary>
    [Layout("JsonLayout")]
    [ThreadAgnostic]
    [AppDomainFixedOutput]
    public class JsonLayout : Layout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonLayout"/> class.
        /// </summary>
        public JsonLayout()
        {
            this.Attributes = new List<JsonAttribute>();
        }

        /// <summary>
        /// Gets the array of attributes' configurations.
        /// </summary>
        /// <docgen category='CSV Options' order='10' />
        [ArrayParameter(typeof(JsonAttribute), "attribute")]
        public IList<JsonAttribute> Attributes { get; private set; }

        /// <summary>
        /// Gets the JsonParameters objects.
        /// </summary>
        /// <docgen category='CSV Options' order='10' />
        public JsonPropertiesRenderer PropertiesRenderer { get; set; }

        /// <summary>
        /// Formats the log event as a JSON document for writing.
        /// </summary>
        /// <param name="logEvent">The log event to be formatted.</param>
        /// <returns>A JSON string representation of the log event.</returns>
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            var jsonWrapper = new JsonEncodeLayoutRendererWrapper();
            var sb = new StringBuilder();
            sb.Append("{ ");
            bool first = true;

            foreach (var col in this.Attributes)
            {
                jsonWrapper.Inner = col.Layout;
                string text = jsonWrapper.Render(logEvent);
                
                if (!string.IsNullOrEmpty(text))
                {
                    if (!first) 
                    {
                        sb.Append(", ");
                    }

                    first = false;

                    sb.AppendFormat("\"{0}\": \"{1}\"", col.Name, text);
                }
            }

            if (PropertiesRenderer != null)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;

                var parametersText = BuildJsonParametersObject(logEvent);
                sb.AppendFormat("\"{0}\": {1}", PropertiesRenderer.Name, parametersText);
            }

            sb.Append(" }");

            return sb.ToString();
        }

        private static string BuildJsonParametersObject(LogEventInfo logEvent)
        {
            var parametersText = new StringBuilder();
            parametersText.Append("{");

            var firstProperty = true;
            foreach (var property in logEvent.Properties)
            {
                if (!firstProperty)
                {
                    parametersText.Append(", ");
                }
                firstProperty = false;

                string valueText = JsonHelper.Escape(property.Value.ToString());
                parametersText.AppendFormat("\"{0}\": \"{1}\"", property.Key, valueText);
            }
            parametersText.Append("}");
            
            return parametersText.ToString();
        }
    }
}