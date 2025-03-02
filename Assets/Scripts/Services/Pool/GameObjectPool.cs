using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoolService.Pool
{
    public class GameObjectPool
    {
        private Dictionary<Type, Queue<GameObject>> poolStorage = new Dictionary<Type, Queue<GameObject>>();
        public void Prepare<T>(T type)
        {

        }

        public GameObject Get<T>() where T : MonoBehaviour
        {

            return null;
        }

        public void Return(GameObject gameObject)
        {

        }
    }

    public interface IPool
    {
        public void Prepare<T>(T type);
        public GameObject Get<T>() where T : MonoBehaviour;
        public void Return(GameObject gameObject);
    }
}
