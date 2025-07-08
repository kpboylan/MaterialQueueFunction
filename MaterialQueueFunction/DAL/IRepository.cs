using MaterialQueueFunction.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialQueueFunction.DAL
{
    public interface IRepository
    {
        Task AddMaterialAsync(Material material);
    }
}
