using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public abstract class NewBehaviourScript : MonoBehaviour
    {
        private bool isActive = false;

        //Create a gameobject that can be pooled (activated and deactivated when needed)
        public bool IsActive 
        {
            get { return isActive; } private set { isActive = value; gameObject.SetActive(isActive); }
        }

        public abstract void New();
        public abstract void Free();

        public virtual void Activate()
        { 
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }
    }
}