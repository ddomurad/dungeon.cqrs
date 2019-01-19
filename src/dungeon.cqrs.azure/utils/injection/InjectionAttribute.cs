using System;
using System.Collections.Concurrent;
using Microsoft.Azure.WebJobs.Description;

namespace dungeon.cqrs.azure.utils.injection
{
    [Binding]
    public class DungeonInjectAttribute : Attribute
    {}
}
