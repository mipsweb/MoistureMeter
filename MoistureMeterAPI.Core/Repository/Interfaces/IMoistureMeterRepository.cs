using MoistureMeterAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoistureMeterAPI.Core.Repository.Interfaces
{
    public interface IMoistureMeterRepository
    {
        public Task Insert(MoistureMeterReading reading);
    }
}
