using UnityEngine;

namespace Game.Services
{
    public interface IResourcesService
    {
        Material MaterialBlock { get; }
        Material MaterialVoxels { get; }
    }
}
