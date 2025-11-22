using System;
using System.Collections.Generic;
using System.Text;

namespace MoistureMeterAPI.Core.Models
{
    public class PaginationResult<T> where T : class
    {
        public long Rows { get; set; }
        public List<T>? Result { get; set; }
    }
}
