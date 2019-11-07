﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DBAutomatorLibrary
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnNameAttribute : Attribute
    {
        public readonly string ColumnName;

        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }


    }
}
