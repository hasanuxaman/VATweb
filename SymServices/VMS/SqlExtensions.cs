﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.VMS
{
    public static class SqlExtensions
    {
        public static void AddWithValueAndNullHandle(this SqlParameterCollection collection, string parameterName, object value)
        {
            if (value == null)
            {
                collection.AddWithValue(parameterName, DBNull.Value);
            }
            else
            {
                collection.AddWithValue(parameterName, value);
            }
        }
        
    }
}
