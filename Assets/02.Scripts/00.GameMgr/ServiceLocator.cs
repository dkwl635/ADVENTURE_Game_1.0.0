using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    //서비스 등록
    public static void Register<T>(T service)
    {
        var type = typeof(T);
        if (!_services.ContainsKey(type))
        {
            _services.Add(type, service);
        }
        else
        {
            Debug.LogWarning($"[ServiceLocator] Service {type.Name} is already registered.");
        }
    }

    //서비스 해제
    public static void Unregister<T>()
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            _services.Remove(type);
        }
    }

    //서비스 등록 확인
    public static bool IsRegistered<T>()
    {
        return _services.ContainsKey(typeof(T));
    }


    //서비스 가져오기
    public static T Get<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        Debug.LogError($"[ServiceLocator] Service {type.Name} is not registered!");
        return default;
    }

    //초기화
    public static void Clear()
    {
        _services.Clear();
    }
}
