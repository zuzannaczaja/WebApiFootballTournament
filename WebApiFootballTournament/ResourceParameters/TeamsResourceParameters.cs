using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiFootballTournament.ResourceParameters
{
    public class TeamsResourceParameters
    {
        const int maxPageSize = 8;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 8;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; }
    }
}
