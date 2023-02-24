using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Global
{
    public class ConfirmPopUp : MonoBehaviour
    {
        public UnityEvent OnConfirm;
        public UnityEvent OnCancel;

        public void InvokeOnConfirm()
        {
            OnConfirm?.Invoke();
        }

        public void InvokeOnCancel()
        {
            OnCancel?.Invoke();
        }
    }

}
