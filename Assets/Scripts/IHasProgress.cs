using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress
{
    public event EventHandler<OnProgressChangedEvenArgs> OnProgressChanged;
    public class OnProgressChangedEvenArgs : EventArgs {
        public float progressNormalized;
    }
}
