using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game.BulletSystem
{
    public class Bullet : MonoBehaviour
    {
        private BulletTypes _bulletType = BulletTypes.None;

        public BulletTypes BulletTypes { get { return _bulletType; } }
    }

    public enum BulletTypes
    {
        None = 0,
        Player = 1,
        Enemy = 2,
    }
}