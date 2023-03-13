﻿using System.Collections.Immutable;
using Server.Models;

namespace Server.Grains;

public interface IUserGrains : IGrainWithGuidKey
{
    Task SetAsync(Users users);

    //Task ClearAsync();

    Task<Users> GetAsync(Guid guid);

    //Task<List<Users>> GetAllAsync();
}
