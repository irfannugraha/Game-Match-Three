using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Observer : MonoBehaviour
{
    public abstract void On_Notify(string value);
}

public abstract class Subject : MonoBehaviour
{
    private List<Observer> observerArray = new List<Observer>();

    public void Register_Observer(Observer observer){
        observerArray.Add(observer);
    }

    public void Notify(string value){
        foreach (var observer in observerArray)
        {
            observer.On_Notify(value);
        }
    }

}