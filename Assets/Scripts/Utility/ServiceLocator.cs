using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static ServiceLocator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Register<T>(T service)
    {
        Type type = typeof(T);
        if (!services.ContainsKey(type))
        {
            services.Add(type, service);
        }
    }

    public void UnRegister<T>(T service)
    {
        Type type = typeof(T);
        if (services.ContainsKey(type))
        { 
            services.Remove(type);
        }
    }

    public T GetService<T>()
    {
        Type type = typeof(T);
        if (services.TryGetValue(type, out object service))
        {
            return (T)service;
        }
        throw new Exception("Service not registered.");
    }
}
