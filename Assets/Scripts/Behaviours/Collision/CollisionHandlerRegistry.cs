using System.Collections.Generic;
using UnityEngine;

public static class CollisionHandlerRegistry
{
    private static Dictionary<int, ICollisionHandler> handlers = new Dictionary<int, ICollisionHandler>();

    public static void RegisterHandler(int instanceID, ICollisionHandler handler)
    {
        handlers[instanceID] = handler;
    }

    public static void UnregisterHandler(int instanceID)
    {
        handlers.Remove(instanceID);
    }

    public static ICollisionHandler GetHandler(int instanceID)
    {
        handlers.TryGetValue(instanceID, out ICollisionHandler handler);
        return handler;
    }
}