﻿namespace Vit.Core.Util.ComponentModel.Model
{
    /// <summary>
    /// description
    /// </summary>
    public class SsDescriptionAttribute : System.Attribute
    {
        public string Value { get; set; }

        public SsDescriptionAttribute(string Value = null)
        {
            this.Value = Value;
        }
    }
}
