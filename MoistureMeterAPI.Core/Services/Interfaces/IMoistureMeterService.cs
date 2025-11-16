using MoistureMeterAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoistureMeterAPI.Core.Services.Interfaces
{
    public interface IMoistureMeterService
    {
        public Task Insert(MoistureMeterReading reading);
    }
}
